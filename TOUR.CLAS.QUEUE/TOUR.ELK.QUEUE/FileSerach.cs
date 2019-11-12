using System;
using System.IO;
using System.Linq;
using System.Configuration;

namespace TOUR.CLAS.QUEUE
{
    class FileSerach
    {
        
        // 규칙이 정해지면 지워질수도 있음
        string test = ConfigurationManager.AppSettings["TEST"].ToString();
        string notHyphen = ConfigurationManager.AppSettings["NOT_HYPHEN"].ToString();
        public void DirSearch(string client, string sDir)
        {
            try
            {
                LogTimeSerach logTimeSerach = new LogTimeSerach();

                //DateTime now = DateTime.Now;
                DateTime now = new DateTime(2016, 11, 09, 08, 15, 00);
                string nowDay = now.ToShortDateString();

                if (notHyphen.Equals("Y"))
                    nowDay = nowDay.Replace("-", "");

                //Console.WriteLine(nowDay);

                // TODO : 날짜에 해당하는 log파일 목록을 가져온다.
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    foreach (var f in Directory.GetFiles(d))
                    {
                        if (f.Contains(nowDay))
                        {
                            logTimeSerach.TimeSerarch(client, f);
                        }
                    }
                    DirSearch(client, d);
                }
            }
            catch (System.Exception excpt)
            {
                
                Console.WriteLine(excpt.Message);
            }
        }
    }
}
