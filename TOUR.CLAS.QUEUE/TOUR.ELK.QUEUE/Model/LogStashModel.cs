using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOUR.CLAS.QUEUE.Model
{
    class LogStashModel
    {
        public LogStashModel(string client, string filePath, string logType, string message, DateTime logTime)
        {
            this.host = client;
            this.logType = logType;
            this.message = message;
            this.path = filePath;
            this.logSaveTime = logTime;
        }

        public string host { get; set; }
        public string logType { get; set; }
        public string path { get; set; }
        public string message { get; set; }
        public DateTime logSaveTime { get; set; }
    }
}
