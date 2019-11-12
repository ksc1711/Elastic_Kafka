using System;
using System.Configuration;
using System.Text.RegularExpressions;


namespace TOUR.MIS
{
    class Program
    {
        

        static void Main(string[] args)
        {

            string serachDriver = ConfigurationManager.AppSettings["SEARCH_DRIVER"].ToString();

            FileUtil fileUtil = new FileUtil();

            fileUtil.DirSearch(serachDriver);
        }
    }
}
