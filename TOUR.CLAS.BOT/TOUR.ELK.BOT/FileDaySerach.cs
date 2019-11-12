using System;
using System.IO;
using System.Configuration;

namespace TOUR.CLAS.BOT
{
    class FileDaySerach
    {
        string host = ConfigurationManager.AppSettings["FtpUri"].ToString();
        string userName = ConfigurationManager.AppSettings["FtpID"].ToString();
        string password = ConfigurationManager.AppSettings["FtpPW"].ToString();
        // 규칙이 정해지면 지워질수도 있음
        string test = ConfigurationManager.AppSettings["Test"].ToString();
        string notHyphen = ConfigurationManager.AppSettings["NotHyphen"].ToString();



        /// <summary>
        /// 지정된 Log파일 폴더를 검색하면서 지정한 날짜에 해당하는 파일들을 ftp로 전송
        /// </summary>
        /// <param name="sDir"> 지정된 Log파일 폴더</param>
        /// <returns></returns>
        public void DirSearch(string sDir)
        {
            // FTP Target 서버 객체 생성
            FtpUtil ftpUtil = new FtpUtil(host, userName, password);

            /*
            //서버 자신의 IP를 가져온다. (IP주소 폴더 생성시 사용)
            IPHostEntry ipHost = Dns.GetHostByName(Dns.GetHostName());
            string myip = ipHost.AddressList[0].ToString();
            ftpUtil.IsExtistDirectory(host + "/" + myip, myip);
            */

            try
            {
                DateTime now = DateTime.Now;
                string nowDay = "";
                // 당일 날짜 획득 및 가져오고 싶은 날짜 
                if(test.Equals("Y")) 
                    nowDay = "2016-08-25";
                else
                    nowDay = now.ToShortDateString();

                if (notHyphen.Equals("Y"))
                    nowDay = nowDay.Replace("-", "");

                //Console.WriteLine(nowDay);

                // 날짜에 해당하는 log파일 목록을 가져온다.
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d))
                    {
                        if (f.Contains(nowDay))
                        {
                            // FTP 서버에 해당 폴더가 있는지 검사하고 없을 시 생성하기 위하여 '/' 를 추가
                            ftpUtil.Upload(f, "/" + nowDay);
                            //Console.WriteLine("FTP UpLoad File : "+ f );

                        }
                    }
                    DirSearch(d);
                }


            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

    }
    

}
