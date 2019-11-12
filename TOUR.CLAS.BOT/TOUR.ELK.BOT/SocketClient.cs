using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Configuration;

namespace TOUR.CLAS.BOT
{
    class SocketClient : IDisposable
    {
        TcpClient _client = null;

        public SocketClient() {

            string TcpTarget = ConfigurationManager.AppSettings["TCP"].ToString();
            int TcpPort = Convert.ToInt32(ConfigurationManager.AppSettings["TcpPort"]);

            _client = new TcpClient();
            _client.Connect(TcpTarget, TcpPort);
               
        }
        
        public void SendMessage(string message)
        {
            try
            {
                //LocalHost에 지정 포트로 TCP Connection을 생성하고 데이터를 송수신 하기
                //위한 스트림을 얻는다.
                NetworkStream writeStream = _client.GetStream();

                //보낼 데이터를 읽어 Default 형식의 바이트 스트림으로 변환
                string dataToSend = message;
                byte[] data = Encoding.Default.GetBytes(dataToSend);

                dataToSend += "\r\n";
                data = Encoding.Default.GetBytes(dataToSend);
                writeStream.Write(data, 0, data.Length);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
               
            }
        }

        public void Dispose() {
            if (_client != null) _client.Close();
        }
    }
}
