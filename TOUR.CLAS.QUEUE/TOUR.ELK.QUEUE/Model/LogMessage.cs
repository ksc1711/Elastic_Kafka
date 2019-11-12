using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOUR.CLAS.QUEUE.Model
{
    class LogMessage
    {
        public LogMessage(string client, string filePath, string logType, int count, string message, DateTime logTime)
        {
            this.clientAddress = client;
            this.seq = count;
            this.logType = logType;
            this.logFileMessage = message;
            this.logFilePath = filePath;
            this.logSaveTime = logTime;
        }
        
        public int seq { get; set; }
        public string clientAddress { get; set; }
        public string logType { get; set; }
        public string logFilePath { get; set; }
        public string logFileMessage { get; set; }
        public DateTime logSaveTime { get; set; }
    }
}
