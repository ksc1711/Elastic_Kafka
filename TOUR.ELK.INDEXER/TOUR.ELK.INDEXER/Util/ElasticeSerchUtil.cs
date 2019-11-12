using System;
using Nest;
using System.Configuration;
using System.Linq;
using TOUR.ELK.INDEXER.Model;

namespace TOUR.ELK.INDEXER.Util
{
    class ElasticeSerchUtil
    {
        public static Uri esNode = new Uri(ConfigurationManager.AppSettings["ELASTIC_HOST"].ToString());
        public static ConnectionSettings esConfig = new ConnectionSettings(esNode);
        public static ElasticClient esClient = new ElasticClient(esConfig);
        // Index이름 
        //string indexName = ConfigurationManager.AppSettings["INDEX_NAME"].ToString();

        public void CreateIndex(string indexName, Object model)
        {
            var index = esClient.Index(model, i => i
                .Index(indexName.ToLower())
                .Type(null)
                .Id(null)
            );
        }

        public void InsertDocument(string indexName, Object model)
        {
            esClient.Index(model, i => i.Index(indexName));
        }

        public void bulkIndexCreate(string indexName, Object modelList)
        {
            var descriptor = new BulkDescriptor();

                descriptor.Index<ElasticDataModel>(op => op
                    .Document(modelList as ElasticDataModel)
                );

            var result = esClient.Bulk(descriptor);

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
