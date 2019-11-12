using System;
using System.Collections.Generic;
using System.Net.Json;
using System.Net;
using System.Configuration;
using System.Text;
using System.IO;

namespace TOUR.CLAS.QUEUE
{
    class JsonConverter
    {
        public string JsonConvert(string message)
        {
            string SMSKey = ConfigurationManager.AppSettings["SMSKey"].ToString();
            string SMSUrl = ConfigurationManager.AppSettings["SMSUrl"].ToString();

            JsonObjectCollection joc = new JsonObjectCollection();

            // 전송할 uri
            WebRequest request = WebRequest.Create(SMSUrl);

            // post 전송할 헤드 설정
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(joc.ToString());
            request.ContentType = " text/json";
            // 서버 키 입렵
            request.Headers.Add("openapikey", SMSKey);
            request.ContentLength = byteArray.Length;

            // stream으로 변환한뒤 json 전송
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();
            WebResponse response = request.GetResponse();
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            Console.WriteLine(responseFromServer);
            reader.Close();
            dataStream.Close();
            response.Close();

            return "";
        }

        public string JsonSend(string message)
        {
            return "";
        }

    }
}
