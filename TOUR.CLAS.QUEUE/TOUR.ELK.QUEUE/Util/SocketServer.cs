using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;
using System.Threading;

namespace TOUR.CLAS.QUEUE
{
    class SocketServer
    {
        TcpListener _server = null;
        NetworkStream _stream = null;
        Thread _thread = null;
        string recvMessage = ConfigurationManager.AppSettings["SEND_OK"].ToString();
        string dirPath = ConfigurationManager.AppSettings["SEARCH_DRIVE"].ToString();


        public SocketServer()
        {
            _server = new TcpListener(AppEnvironment.Get.TcpEndPoint);
        }

        public void Start()
        {

            _server.Start();

            FileSerach fileSerach = new FileSerach();

            while (true)
            {
                Socket client = null;
                IPAddress clientIp = null;

                try
                {
                    client = _server.AcceptSocket();
                    clientIp = ((IPEndPoint)client.RemoteEndPoint).Address;
                    Console.WriteLine("ACCEPTED : {0}", clientIp);
                }
                catch (Exception) { client = null; }

                _thread = new Thread(() =>
                {
                    try
                    {
                        if (client != null)
                        {
                            _stream = new NetworkStream(client);

                            using (var reader = new StreamReader(_stream, System.Text.Encoding.UTF8))
                            {
                                while (client.Connected)
                                {
                                    var res = reader.ReadLine();

                                    if (!String.IsNullOrEmpty(res))
                                    {
                                        string[] commands = res.Split(' ');

                                        Console.Write("COMMAND {0} : {1}", clientIp, commands[0]);

                                        if(commands[0].Equals(recvMessage))
                                        {
                                            fileSerach.DirSearch(clientIp.ToString(), dirPath);

                                            // TODO : 디큐하여 데이터를 DB에 저장하는 함수
                                            SendLog sendLog = new SendLog();
                                            sendLog.SendMessage();
                                        }
                                    }
                                    else
                                    {
                                        break;
                                    }

                                    Thread.Sleep(10);
                                }
                            }
                        }
                    }
                    catch (IOException) { }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception : {0}", ex.Message);
                    }
                    finally
                    {
                        if (client != null)
                        {
                            client.Dispose();
                            client.Close();
                        }

                        if (_stream != null) { _stream.Flush(); }

                        Console.WriteLine("DISCONNECTED : {0}", clientIp);
                    }
                });

                _thread.IsBackground = true;
                _thread.Start();
            }
        }

        public void Stop()
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream.Close();
            }

            if (_server != null) _server.Stop();
            if (_thread != null) _thread.Abort();
        }
    }
}
