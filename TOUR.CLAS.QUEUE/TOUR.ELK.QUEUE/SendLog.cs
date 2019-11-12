using System;
using System.Net.Json;
using TOUR.CLAS.QUEUE.Model;
using System.Configuration;

namespace TOUR.CLAS.QUEUE
{
    class SendLog
    {

        public void SendMessage()
        {
            // ElasticSerach 객체 생성
            ElasticUtil elasticUtil = new ElasticUtil();

            if(SendQueue.sendQ.Count > 0)
            { 
                for(int i=0 ; i<= SendQueue.sendQ.Count ; i++)
                {

                    try
                    {
                        // TODO : 큐의 처음에 있는 개체의 정보를 가져온다.
                        LogMessage msg = SendQueue.sendQ.Peek();
                        msg.seq = i;

                        // ElasticSearch에 데이터 추가
                        elasticUtil.InsertDocument(msg);

                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine( ex.Message);
                        continue;
                    }
                    //Console.WriteLine(msg.logFilePath + " 파일의 " + msg.seq.ToString() + " 번째 전송 메시지 " + msg.logFileMessage);

                    SendQueue.sendQ.Dequeue();

                    
                }
            }
            else
            {
                Console.WriteLine("큐가 비었습니다.");
            }
        }


    }
}
