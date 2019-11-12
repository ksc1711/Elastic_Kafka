using System;
using Nest;
using System.Configuration;

namespace TOUR.CLAS.QUEUE
{
    class ElasticUtil
    {

        public static Uri esNode = new Uri(ConfigurationManager.AppSettings["ELASTIC_HOST"].ToString());
        public static ConnectionSettings esConfig = new ConnectionSettings(esNode);
        public static ElasticClient esClient = new ElasticClient(esConfig);
        // Index이름 
        string indexName = ConfigurationManager.AppSettings["INDEX_NAME"].ToString();

        public void CreateIndex(string indexName, string indexType, Object model)
        {
            var index = esClient.Index(model, i => i
                .Index(indexName.ToLower())
                .Type(indexType.ToLower())
            );
        }

        public void InsertDocument( Object model)
        {
            esClient.Index(model, i => i.Index(indexName));
        }

        public bool DeleteIndex(string indexName)
        {
             bool index = esClient.DeleteIndex(indexName).Acknowledged;

             return index;
        }

        public bool ExistsIndex(string indexName)
        {
            bool index = esClient.IndexExists(indexName).Exists;

            return index;
        }

    }

}
