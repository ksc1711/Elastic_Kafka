using System;
using Nest;
using Elasticsearch.Net;

namespace TOUR.ELK.INDEXER.Model
{
    
    class ElasticDataModel
    {
        public ElasticDataModel(string path, string type, string host, string message,DateTime logTime)
        {
            this.filePath = path;
            this.logType = type;
            this.host = host;
            this.message = message;
            this.logSaveTime = logTime;
        }

        public string filePath { get; set; }

        public string logType { get; set; }


        public string host { get; set; }

        [Text(Index=false)]
        public string message { get; set; }

        public DateTime logSaveTime { get; set; }

    }


}
