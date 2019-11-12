using System;
using KafkaNet;
using KafkaNet.Model;
using System.Threading.Tasks;
using System.Configuration;
using TOUR.ELK.INDEXER.Model;
using System.Collections.Generic;

namespace TOUR.ELK.INDEXER.Util
{
    class KafkaUtil
    {
        readonly string _topicName;
        readonly NLogging _loger;
        DataModelUtil dataUtil = new DataModelUtil();
        List<ElasticDataModel> dataList = new List<ElasticDataModel>();
        ElasticeSerchUtil elasticSerach = new ElasticeSerchUtil();

        public KafkaUtil()
        {
            _topicName = ConfigurationManager.AppSettings["TOPIC_NAME"].ToString();
            _loger = new NLogging("ERROR");
        }

        public void Run()
        {
            var options = new KafkaOptions(new Uri(ConfigurationManager.AppSettings["KAFKA_HOST"].ToString()))
            {
                Log = new ConsoleLog()
            };


            // kafka에서 데이터를 받아온다.
            Task.Factory.StartNew(() =>
            {
                var consumer = new Consumer(new ConsumerOptions(_topicName, new BrokerRouter(options)) { Log = new ConsoleLog() });

                int count = 0;

                _loger.Info(DateTime.Now.ToString(), "");
                foreach (var data in consumer.Consume())
                {

                    try
                    {
                        // kafka에서 가져온 메시지를 가공하여 가져온다.
                        //bulk 미사용
                        var model = dataUtil.DataModeling(data.Value, "one");

                        ElsticSerachSend((ElasticDataModel)model);

                        // bulk 사용
                        //dataList.Add((ElasticDataModel)dataUtil.DataModeling(data.Value, "bulk"));

                        if(dataList.Count >= 100)
                        {

                        }

                        count++;
                        //Console.WriteLine("Response: P{0},O{1} : {2}", data.Meta.PartitionId, data.Meta.Offset, data.Value.ToUtf8String());
                    }
                    catch (Exception ex)
                    {
                        _loger.Error(ex);
                    }

                }
                _loger.Info(DateTime.Now.ToString() + " / " + count.ToString(), "");
            });

            Console.ReadLine();
        }

        public void ElsticSerachSend(object model)
        {
            try
            {

                //string indexName = model.host + "_" + model.logSaveTime.ToString().Substring(0, 7);//model.logSaveTime.Year.ToString()+ "_" + model.logSaveTime.Month.ToString();
                string indexName = "125comlog";
                //string indexName = "indextest";

                //elasticSerach.bulkIndexCreate(indexName, model);

                //ElasticSerach에 해당 인덱스가 존재하는지 확인.
                if (elasticSerach.ExistsIndex(indexName))
                {
                    //있으면 데이터 추가 
                    Console.WriteLine("Insert Index : " + indexName);
                    elasticSerach.InsertDocument(indexName, (ElasticDataModel)model);
                    //elasticSerach.bulkIndexCreate(indexName, (List<ElasticDataModel>)model);
                }
                else
                {
                    //없으면 인덱스 생성
                    Console.WriteLine("Create Index : " + indexName);
                    elasticSerach.CreateIndex(indexName, (ElasticDataModel)model);

                }
            }
            catch (Exception ex)
            {
                _loger.Error(ex);
                Console.WriteLine(ex.Message);
            }

        }
    }
}
