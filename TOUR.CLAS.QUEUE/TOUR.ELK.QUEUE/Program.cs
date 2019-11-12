using System;
using System.Collections.Generic;
using System.Collections;
using System.Configuration;
using TOUR.CLAS.QUEUE.Model;

namespace TOUR.CLAS.QUEUE
{
    class Program
    {

        static void Main(string[] args)
        {

            ElasticUtil test = new ElasticUtil();
            LogStashModel logstash = new LogStashModel("http://192.168.28.174:9200", "D:/ComLog/Housing/WebService/HousingService_2016-07-04.log", "INFO", "\\xC5\\xEB\\xC7??\\xF6 \\xC1\\xF6\\xB5\\xB5 \\xC1\\xC2?", DateTime.Today);

            Console.WriteLine(test.ExistsIndex("test").ToString());
            
            //SocketServer socketServer = new SocketServer();
            //socketServer.Start();
            
            
        }
    }
}
