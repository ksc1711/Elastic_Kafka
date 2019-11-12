using System;
using System.Text.RegularExpressions;
using TOUR.ELK.INDEXER.Model;
using System.Text;

namespace TOUR.ELK.INDEXER.Util
{
    class DataModelUtil
    {
        
        Regex _reg = null;
        NLogging _loger = null;

        public DataModelUtil() {

            _reg = new Regex(@"[Aa]lert|ALERT|[Tt]race|TRACE|[Dd]ebug|DEBUG|[Nn]otice|NOTICE|[Ii]nfo|INFO|[Ww]arn?(?:ing)?|WARN?(?:ING)?|[Ee]rr?(?:or)?|ERR?(?:OR)?|[Cc]rit?(?:ical)?|CRIT?(?:ICAL)?|[Ff]atal|FATAL|[Ss]evere|SEVERE|EMERG(?:ENCY)?|[Ee]merg(?:ency)?");
            _loger = new NLogging("ERROR");
        }

        // TODO : 데이터 가공
        public object DataModeling(byte[] dataValue, string status)
        {
            string logType = "";
            string filePath = "";
            string host = "";
            string logTime = "";
            string realMessage = "";
            //char[] separator = "".ToCharArray();
            ElasticDataModel model = null;

            string message = ConvertEncoding(dataValue);

            try
            {
                // rubydebug로 입력되면 무조건 있으나 혹시몰라 예외처리.
                if (message.Contains("=>"))
                {

                    string[] convertMessage = Regex.Split(message, ",\n"); //Replace("\"", "").Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    // TODO : Kafka에서 가져온 메시지를 가공하여 ElasticSerach에 저장 할 수 있는 DATA로 변형한다.
                    if(convertMessage.Length > 0)
                    {
                        int count = 0;
                        int indexNum = 0;

                        foreach (var data in convertMessage)
                        {
                            if(data.Length > 1)
                            {
                                indexNum = data.IndexOf("=>") + 2;

                            convertMessage[count] = data.Substring(indexNum, data.Length - indexNum);
                            }
                            count++;
                        }
                    }

                    // logtype을 알아낸다.
                    MatchCollection match = _reg.Matches(convertMessage[4]);
                    if (match.Count != 0)
                    {
                        logType = match[0].ToString();
                    }
                    else
                    {
                        logType = "INFO";
                    }

                    // Log 저장 시간을 가져온다.
                    //reg = new Regex(@"\d{4,4}[-/]\d{2,2}[-/]\d{2,2}...........");
                    //match = reg.Matches(convertMessage[4]);
                    logTime = convertMessage[1];

                    // 가공된 데이터 저장
                    // 파일경로 
                    filePath = convertMessage[0];
                    // host name
                    host = convertMessage[3].Replace("\"","");
                    // 가공된 메시지
                    realMessage = convertMessage[4].Replace("\\\\", "/");


                    // 데이터 가공을 완료한 뒤 가공한 데이터를 ElasticSerach에 전송
                    model = new ElasticDataModel(filePath, logType, host, realMessage, ConvertLogDate(logTime));
                    //ElsticSerachSend(model);

                }
                if(status == "bulk")
                {
                    return model;
                }
                else
                {
                    return model;
                }
                
            }
            catch (Exception ex)
            {
                _loger.Error(ex);
                return ex.Message;
            }
            
        }

        // TODO : 데이터 ElasticSerach로 전송
        

        // TODO : kafka에서 넘겨받은 데이터 Encoding 변경
        public string ConvertEncoding(byte[] dataValue)
        {
            int euckrCodepage = 51949;

            // 인코딩을 편리하게 해주기 위해서 인코딩클래스 변수를 만듭니다.
            Encoding utf8 = Encoding.UTF8;
            Encoding euckr = Encoding.GetEncoding(euckrCodepage);

            byte[] pbDest = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("euc-kr"), dataValue);
            dataValue = Encoding.Convert(Encoding.GetEncoding("euc-kr"), Encoding.UTF8, pbDest);
            char[] psUnicode = UTF8Encoding.UTF8.GetChars(dataValue);
            string strReceiveText = new string(psUnicode);
            //string strReceiveText = Encoding.GetEncoding(51949).GetString(dataValue);
            return strReceiveText;
        }

        // TODO : 문자열 날짜로 변경 과정에서 오류 발생시 오늘 날짜로 대처 (어차피 로그 쌓인날과 전송되는날은 동일할테니.)
        public DateTime ConvertLogDate(string logTime)
        {
            DateTime logDate;
            try
            {
                logDate = Convert.ToDateTime(logTime);
                return logDate;
            }
            catch (Exception ex)
            {
                _loger.Error(ex);
                logDate = DateTime.Now;
                return logDate;
            }
        }
                 
    }
}
