using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Configuration;
using System.IO;

namespace TOUR.CLAS.BOT
{
    // 파일 위변조 클래스 
    class WatcherRun
    {
        public int FileWatchNum { get; set; }
        public string CreateFileName { get; set; }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void Run()
        {
            string[] args = System.Environment.GetCommandLineArgs();

            string SD = ConfigurationManager.AppSettings["SearchDrive"].ToString();

            Watch(SD);

            // Wait for the user to quit the program.
            Console.WriteLine("Press \'q\' to quit the Program.");
            while (Console.Read() != 'q') ;
        }

        public static void Watch(string watch_folder)
        {
            WatcherRun obj = new WatcherRun();

            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = watch_folder;

            // watcher가 하위 폴더까지 검색할수있는 권한 설정
            watcher.IncludeSubdirectories = true;

            watcher.NotifyFilter = NotifyFilters.LastAccess
               | NotifyFilters.FileName | NotifyFilters.Size;

            // Add event handlers.
            watcher.Created += new FileSystemEventHandler(obj.OnChanged);
            watcher.Changed += new FileSystemEventHandler(obj.OnChanged);

            // Begin watching.
            watcher.EnableRaisingEvents = true;
        }


        // Create, Change, Delete 이벤트 발생시 실행
        public void OnChanged(object source, FileSystemEventArgs e)
        {

            // 이벤트가 created일 경우 발생 
            if (e.ChangeType.ToString().ToLower().Equals("created"))
            {
                CreateFileName = e.Name;
                Console.WriteLine("OnCreate /" + e.Name + "/" + e.FullPath);

            }
            else
            {

                Console.WriteLine("OnChanged /" + e.Name + "/" + e.FullPath);
            }

            //e.ChangeType
            FileWatchNumCheck();
        }


        public void FileWatchNumCheck()
        {
            // 100개의 이벤트 발생시 consol의 글씨를 삭제한다.
            FileWatchNum++;

            if (FileWatchNum >= 100)
            {
                Console.Clear();
                FileWatchNum = 0;
            }
        }
    }
}
