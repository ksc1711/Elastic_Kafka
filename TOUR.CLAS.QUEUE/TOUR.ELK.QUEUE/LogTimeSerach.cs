using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TOUR.CLAS.QUEUE.Model;
using System.Configuration;

namespace TOUR.CLAS.QUEUE
{
    class LogTimeSerach
    {
        string logTitle = ConfigurationManager.AppSettings["LOG_TYPE"].ToString();

        /// <summary>
        /// TODO :  지정된 로그파일을 검색하여 지정된 시간으로부터 지정된 시간안에 쌓인 로그들을 queue에 저장합니다.
        /// </summary>
        public void TimeSerarch(string client, string filePath)
        {
            int count = 0;
            string strColumn= "";
            int timeGap = 0;
            // 시간안에 포함된 로그 라인
            string oneLogLine = "";
            // 시간안에 포함된 로그 라인
            bool sameLog = false;
            //로그 타입
            string logType = "";

            //실제 로그가 저장된 시간
            //DateTime logSaveTime = DateTime.Now;
            DateTime logSaveTime = new DateTime(2016, 11, 09, 08, 15, 00);

            // 현재시간
            //DateTime now = DateTime.Now;
            DateTime now = new DateTime(2016, 11, 09, 08, 15, 00);

           

            //시간 찾아주는 정규식 
            Regex reg = new Regex(@"\d{4,4}-\d{2,2}-\d{2,2} \d{2,2}:\d{2,2}:\d{2,2}");

            Console.WriteLine("Read file : " + filePath);

            try
            {
                var readAll = File.ReadAllLines(filePath, Encoding.Default);
                int intCurRows = readAll.Length;
                
                if(intCurRows > 0)
                {
                    foreach (var line in readAll)
                    {
                        MatchCollection match = reg.Matches(line);
                        if(match.Count != 0)
                        {
                            // TODO : 로그에서 시간 부분의 라인을 가져온다.
                            string matchTime = match[0].ToString();
                            logSaveTime = Convert.ToDateTime(matchTime);

                            // TODO : 시간차이를 구함 지금 시간의 10분전보다 작으면 Enqueue
                            TimeSpan ts = now.Subtract(logSaveTime);
                            timeGap = Convert.ToInt32(Math.Ceiling(ts.TotalMinutes));

                            // TODO :  지정한 시간 내의 로그 내용이 아니면 저장하지 않는다.
                            sameLog = false;

                            if (timeGap <= 10 && timeGap > 0)
                            {
                                // TODO :  Log 라인 저장
                                strColumn = line;

                                // 날짜 형식
                                sameLog = true;
                                if (count == 0)
                                {
                                    oneLogLine = strColumn;
                                    count++;
                                    continue;
                                }

                                // TODO : 로그 타입 알아내기 어떻게 하면 좋을까 
                                var splitData = logTitle.Split('|');

                                foreach (var item in splitData)
                                {
                                    if(strColumn.Contains(item))
                                    {
                                        logType = item;
                                    }
                                    
                                }

                                // TODO :  로그 카운터 
                                SetSendQueue(client, filePath, logType, count, oneLogLine, logSaveTime);
                                count++;
                                oneLogLine = strColumn;

                            }

                        }
                        // TODO : 지정된 시간안에 시간이 아닌 그냥 내용의 로그만 기록한다. 
                        else
                        {
                            if (sameLog)
                            {
                                oneLogLine += strColumn;
                                //Console.WriteLine(strColumn);
                            }
                        }
                    }
                
                }
                if (count != 0) SetSendQueue(client, filePath, logType, count, oneLogLine, logSaveTime);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// log파일 모델을 생성하여 queue에 enqueue하는 메소드입니다.
        /// </summary>
        /// <param name="filePath"> log파일 경로 입니다.</param>
        /// <param name="count"> log파일별 index입니다.</param>
        /// <param name="sendLog">전송될 log message입니다. </param>
        public void SetSendQueue(string client, string filePath, string logType, int count, string sendLog, DateTime logTime)
        {
            // TODO : 전송 로그 모델
            LogMessage logMessage = new LogMessage(client, filePath, logType, count, sendLog, logTime);
            SendQueue.sendQ.Enqueue(logMessage);
        }
    }
}
