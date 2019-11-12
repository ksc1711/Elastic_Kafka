using System;
using TOUR.ELK.INDEXER.Util;
using KafkaNet;
using KafkaNet.Model;
using System.Threading.Tasks;
using System.Configuration;
using TOUR.ELK.INDEXER.Model;
using System.Collections.Generic;

namespace TOUR.ELK.INDEXER
{
    class Program
    {
        static void Main(string[] args)
        {
            //테스트 소스
            //NLogging nt = new NLogging();
            //nt.regTest();

            KafkaUtil kafaUtil = new KafkaUtil();
            kafaUtil.Run();

            /*
             Task => 쓰레드와 같은 병렬처리다.
             Consumer => 자체가 와일이 돌면서 감시하는 녀석으로 확인된다.
             Regex => 굉장히 무겁기때문에 생성자에 등록해두는게 좋다.
             에외처리 => - Task 및 스레드는 병렬처리이기때문에 그 위에서 예외를 처리한다고 해도 잡히지 않는다. 
                         - 반복문을 예외처리하면 문제 발생 시 프로그램이 종료된다.
                         - 하지만 반복문안에 에외처리 시에는 문제 발생시 문제처리 후 다음 단계로 진행된다.   
             */
        }
    }
}
