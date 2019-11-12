using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Text;
using NLog;

namespace TOUR.ELK.INDEXER
{
    class NLogging
    {
        
        private string _loggingType;


        public NLogging(string loggingType)
        {
            _loggingType = loggingType;
        }

        public void Debug(string message, string section = "")
        {
            var loggingType = (string.IsNullOrWhiteSpace(section)) ? _loggingType : _loggingType + "." + section;
            var logger = LogManager.GetLogger(loggingType);
            var logEvent = new LogEventInfo(LogLevel.Debug, loggingType, message);

            logger.Log(logEvent);
        }

        public void Info(string message, string section = "")
        {
            var loggingType = (string.IsNullOrWhiteSpace(section)) ? _loggingType : _loggingType + "." + section;
            var logger = LogManager.GetLogger(loggingType);
            var logEvent = new LogEventInfo(LogLevel.Info, loggingType, message);

            logger.Log(logEvent);
        }

        public void Error(Exception e)
        {
            var loggingType = _loggingType;
            var logger = LogManager.GetLogger(loggingType);
            var logEvent = new LogEventInfo();

            logEvent.Level = LogLevel.Error;
            logEvent.LoggerName = loggingType;
            logEvent.Exception = e;
            logEvent.Message = e.ToString();

            logger.Log(logEvent);
        }

        

        #region 정규식 테스트 

        public string regTest()
        {
            var readAll = File.ReadAllLines(@"D:\StagingComLog\allDatetime.txt", Encoding.Default);
            int intCurRows = readAll.Length;

            if (intCurRows > 0)
            {
                foreach (var line in readAll)
                {
                    //Regex reg = new Regex("([0-9]{4}).*([0-9]{2}).*([0-9]{2})..");
                    Regex reg = new Regex(@"\d{4,4}[-/]\d{2,2}[-/]\d{2,2}...........");
                    MatchCollection match = reg.Matches(line.ToString());
                    if (match.Count != 0)
                    {
                        Console.WriteLine("match test = " + match[0].ToString());
                        try
                        {
                            DateTime dt = Convert.ToDateTime(match[0].ToString());
                            Console.WriteLine("Convert Ok = " + dt.ToString());
                        }
                        catch
                        {
                            Console.WriteLine("Convert Fail = " + match[0].ToString());
                            DateTime dt = DateTime.Today;
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine(line + "매치실패");
                    }
                }
            }

            return "";
        }
        #endregion

    }
}
