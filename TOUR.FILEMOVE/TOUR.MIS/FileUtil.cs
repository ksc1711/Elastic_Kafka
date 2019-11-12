using System;
using System.IO;
using System.Configuration;

namespace TOUR.MIS
{
    class FileUtil
    {
        string dirPath = ConfigurationManager.AppSettings["COPY_FOLDER"].ToString();
        string serachDriver = ConfigurationManager.AppSettings["SEARCH_DRIVER"].ToString();
        FileInfo fileInfo;
        DateTime nowDay = DateTime.Today;
        TimeSpan timeGap;

        public void DirSearch(string sDir)
        {
            try
            {
                int count = 0;
                
                // 전달 받은 폴더안에 하위 디렉토리를 반환
                foreach (var d in Directory.GetDirectories(sDir))
                {
                    
                    foreach (string f in Directory.GetFiles(d))
                    {
                        fileInfo = new FileInfo(f);
                        timeGap = nowDay.Subtract(fileInfo.LastWriteTime.Date);

                        if (timeGap.TotalDays > 0)
                        {
                            count++;
                            Console.WriteLine("Move File : "+ f );
                            MoveeFile(f);
                        }
                    }
                    DirSearch(d);
                    Console.WriteLine(d + " Count : " + count);
                }
            }
            catch (System.Exception excpt)
            {
                Console.WriteLine(excpt.Message);
            }
        }

        private void MoveeFile(string FilePath)
        {
            DirectoryInfo di = new DirectoryInfo(dirPath);

            string TargetPath = "";
            string FileName = Path.GetFileName(FilePath);

            //TODO : 이동 및 복사의 경우 상위 폴더를 생성한 뒤 파일을 복사 하거나 이동한다.
            try
            {
                if (di.Exists != true) Directory.CreateDirectory(dirPath);

                TargetPath = CreateFolder(Path.GetDirectoryName(FilePath), dirPath);
                if (TargetPath.Equals("X"))
                    return;

                File.Move(FilePath, Path.Combine(TargetPath, FileName));


            }
            catch (Exception e)
            {

            }
        }

        private string CreateFolder(string sourceFolder, string destFolder)
        {
            try
            {
                if (!Directory.Exists(destFolder))
                    Directory.CreateDirectory(destFolder);

                // TODO :  복사 및 이동 할 파일의 상위 폴더명 그대로 target폴더에 생성한다.
                string targetPath = sourceFolder.Replace(serachDriver, destFolder + "\\");

                if (!Directory.Exists(targetPath))
                    Directory.CreateDirectory(targetPath);

                return targetPath;
            }
            catch (Exception ex)
            {
                return "X";
            }
        }
    }
}