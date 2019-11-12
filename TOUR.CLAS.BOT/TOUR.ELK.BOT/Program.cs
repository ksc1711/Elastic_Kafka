using System;
using System.Configuration;
using System.Timers;

namespace TOUR.CLAS.BOT
{
    class Watchers
    {
        static SocketClient sc = null;

        static void Main(string[] args)
        {
            try
            {
                int sendTime = Convert.ToInt32(ConfigurationManager.AppSettings["SendTime"]);

                sc = new SocketClient();

                /*
                // 지정한 시간 마다  Log 전송 
                Timer timer = new Timer();
                timer.Interval = 1000 * sendTime;
                timer.Elapsed += new ElapsedEventHandler(SendLog);
                timer.Start();
                */

                /*
                string sendMessage = ConfigurationManager.AppSettings["SendOK"].ToString();
                SocketClient sc = new SocketClient();
                sc.SendMessage(sendMessage);
                */
                SendLog(); // 테스트 용 함수 

                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();

                sc.Dispose();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 타이머가 정해진 시간마다 로그파일 검색하여 FTP 전송하는 함수.
        /// </summary>
        /// <returns></returns>
        static void SendLog(object sender, ElapsedEventArgs e)
        {

            string sendMessage = ConfigurationManager.AppSettings["SendOK"].ToString();

            // Path : ComLog
            string dirPath = ConfigurationManager.AppSettings["SearchDrive"].ToString();

            FileDaySerach fileDaySerach = new FileDaySerach();
            fileDaySerach.DirSearch(dirPath);

            sc.SendMessage(sendMessage);
        }

        #region 테스트용 함수
        static void SendLog()
        {
            string sendMessage = ConfigurationManager.AppSettings["SendOK"].ToString();

            // Path : ComLog
            string dirPath = ConfigurationManager.AppSettings["SearchDrive"].ToString();

            FileDaySerach fileDaySerach = new FileDaySerach();
            fileDaySerach.DirSearch(dirPath);

            sc.SendMessage(sendMessage);
        }
        #endregion
    }
}
