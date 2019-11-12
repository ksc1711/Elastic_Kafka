using System;
using System.IO;
using System.Configuration;
using System.Net;

namespace TOUR.CLAS.QUEUE
{
    class AppEnvironment
    {
        private static volatile AppEnvironment _instance;
        private static object syncRoot = new Object();

        private AppEnvironment()
        {
            var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var settings = ConfigurationManager.AppSettings;

            this.MaxThread = Convert.ToInt32(settings["MAX_THREAD"]);
            this.TcpEndPoint = new IPEndPoint(IPAddress.Parse(settings["SOCKET_IP"].Trim()), Convert.ToInt32(settings["SOCKET_PORT"].Trim()));

            if (this.MaxThread > 512) throw new ArgumentOutOfRangeException("MaxThread", "the MaxThread is greater than 512");
        }

        public static AppEnvironment Get
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new AppEnvironment();
                    }
                }

                return _instance;
            }
        }

        public string DataDirectory { get; private set; }
        public string HistoryDirectory { get; private set; }
        public IPEndPoint TcpEndPoint { get; private set; }
        public string AdminAddress { get; private set; }
        public int MaxThread { get; private set; }


    }
}
