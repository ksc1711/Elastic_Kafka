using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Configuration;

namespace TOUR.CLAS.BOT
{
    class FtpUtil
    {
        #region 멤버 변수 & 프로퍼티

        /// <summary>
        ///
        /// </summary>
        private string host;

        /// <summary>
        /// FTP 서버 호스트명(IP)를 가져옵니다.
        /// </summary>
        public string Host
        {
            get { return host; }
            private set { host = value; }
        }

        private string userName;

        /// <summary>
        /// 사용자 명을 가져옵니다.
        /// </summary>
        public string UserName
        {
            get { return userName; }
            private set { userName = value; }
        }

        private string password;

        /// <summary>
        /// 비밀번호를 가져옵니다.
        /// </summary>
        public string Password
        {
            get { return password; }
            private set { password = value; }
        }

        #endregion

        #region 생성자

        /// <summary>
        /// FXFtpUtil의 새 인스턴스를 생성합니다.
        /// </summary>
        /// <param name="host">FTP 서버 주소 입니다.</param>
        /// <param name="userName">FTP 사용자 아이디 입니다.</param>
        /// <param name="password">FTP 비밀번호 입니다.</param>
        public FtpUtil(string host, string userName, string password)
        {
            this.Host = host;
            this.UserName = userName;
            this.Password = password;
        }

        #endregion

        #region 메서드

        /// <summary>
        /// 파일을 FTP 서버에 업로드 합니다.
        /// </summary>
        /// <param name="localFileFullPath">로컬 파일의 전체 경로 입니다.</param>
        /// <param name="ftpFilePath"><para>파일을 업로드 할 FTP 전체 경로 입니다.</para><para>하위 디렉터리에 넣는 경우 /디렉터리명/파일명.확장자 형태 입니다.</para></param>
        /// <exception cref="FileNotFoundException">지정한 로컬 파일(localFileFullPath)이 없을 때 발생합니다.</exception>
        /// <returns>업로드 성공 여부입니다.</returns>
        public bool Upload(string localFileFullPath, string ftpFilePath)
        {
            string rootPath = ConfigurationManager.AppSettings["SearchDrive"].ToString(); ;

            //해당파일의 파일정보를 가져온다.
            FileInfo fileInfo = new FileInfo(localFileFullPath);

            //지정한 날짜의 폴더가 ftp 서버에 있는지 확인.
            IsExtistDirectory(this.host + ftpFilePath, ftpFilePath.Replace("/", ""));

            //지정한 날짜 폴더 아래에 복사하려는 로그파일들의 상위 폴더들이 있는지 확인하고 생성한다.
            
            //FTP 전송 파일 전체 경로 
            string ftpFileFullPath =  this.Host + ftpFilePath + GetParentDirectory(this.host + ftpFilePath,localFileFullPath.Replace(rootPath, "")) + "/" + fileInfo.Name;

            FtpWebRequest ftpWebRequest = WebRequest.Create(new Uri(ftpFileFullPath)) as FtpWebRequest;
            ftpWebRequest.Credentials = GetCredentials();
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.Timeout = 10000;
            ftpWebRequest.Method = WebRequestMethods.Ftp.UploadFile;

            
            
            FileStream fileStream = fileInfo.OpenRead();
            Stream stream = null;
            byte[] buf = new byte[2048];
            int currentOffset = 0;
            try
            {
                stream = ftpWebRequest.GetRequestStream();
                currentOffset = fileStream.Read(buf, 0, 2048);
                while (currentOffset != 0)
                {
                    stream.Write(buf, 0, currentOffset);
                    currentOffset = fileStream.Read(buf, 0, 2048);
                }

                Console.WriteLine("FTP UpLoad File : " + ftpFileFullPath);
            }
            finally
            {
                fileStream.Dispose();
                if (stream != null)
                    stream.Dispose();
            }

            return true;
        }

        /// <summary>
        /// 해당 경로에 파일이 존재하는지 여부를 가져옵니다.
        /// </summary>
        /// <param name="ftpFilePath">파일 경로</param>
        /// <returns>존재시 참 </returns>
        private bool IsFTPFileExsit(string ftpFilePath)
        {
            string fileName = GetFileName(ftpFilePath);
            string ftpFileFullPath = string.Format("ftp://{0}/{1}", this.Host, GetDirectoryPath(ftpFilePath));
            FtpWebRequest ftpWebRequest = WebRequest.Create(new Uri(ftpFileFullPath)) as FtpWebRequest;
            ftpWebRequest.Credentials = new NetworkCredential(this.UserName, this.Password);
            ftpWebRequest.UseBinary = true;
            ftpWebRequest.UsePassive = true;
            ftpWebRequest.Timeout = 10000;
            ftpWebRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            FtpWebResponse response = null;
            string data = string.Empty;
            try
            {
                response = ftpWebRequest.GetResponse() as FtpWebResponse;
                if (response != null)
                {
                    StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                    data = streamReader.ReadToEnd();
                }
            }
            finally
            {
                if (response != null)
                {
                    response.Close();
                }
            }

            string[] directorys = data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return (from directory in directorys
                    select directory.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries)
                        into directoryInfos
                        where directoryInfos[0][0] != 'd'
                        select directoryInfos[8]).Any(
                        name => name == fileName);
        }

        /// <summary>
        /// FTP 풀 경로에서 Directory 경로만 가져옵니다.
        /// </summary>
        /// <param name="ftpFilePath">FTP 풀 경로</param>
        /// <returns>디렉터리 경로입니다.</returns>
        private string GetDirectoryPath(string ftpFilePath)
        {
            string[] datas = ftpFilePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            string directoryPath = string.Empty;

            for (int i = 0; i < datas.Length - 1; i++)
            {
                directoryPath += string.Format("/{0}", datas[i]);
            }
            return directoryPath;
        }

        /// <summary>
        /// FTP 풀 경로에서 파일이름만 가져옵니다.
        /// </summary>
        /// <param name="ftpFilePath">FTP 풀 경로</param>
        /// <returns>파일명입니다.</returns>
        private string GetFileName(string ftpFilePath)
        {
            string[] datas = ftpFilePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            return datas[datas.Length - 1];
        }

        /// <summary>
        /// FTP에 해당 디렉터리가 있는지 알아온다.
        /// </summary>
        /// <param name="currentDirectory"> 파일 및 폴더를 생성할 FTP 폴더 풀 경로</param>
        /// /// <param name="serachDirectory">새로 생성할 폴더 명</param>
        /// <returns>있으면 참</returns>
        public bool IsExtistDirectory(string currentDirectory, string serachDirectory)
        {
            try
            {

                Uri ftpUri = new Uri(currentDirectory.Remove(currentDirectory.Length - serachDirectory.Length, serachDirectory.Length));

                FtpWebRequest reqFtp = (FtpWebRequest)WebRequest.Create(ftpUri);
                reqFtp.Credentials = new NetworkCredential(UserName, Password);
                reqFtp.Timeout = 10000;
                reqFtp.Method = WebRequestMethods.Ftp.ListDirectory;

                FtpWebResponse resFtp = (FtpWebResponse)reqFtp.GetResponse();

                StreamReader reader;
                reader = new StreamReader(resFtp.GetResponseStream());

                string strData;
                strData = reader.ReadToEnd();

                if (strData.Equals(""))
                {
                    MakeDirectory(currentDirectory);
                    return false;
                }
                //string[] filesInDirectory = strData.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (strData.Contains(serachDirectory+"\r\n"))
                {
                    resFtp.Close();
                    return true;
                }
                else
                {
                    MakeDirectory(currentDirectory);
                    resFtp.Close();
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 상위 디렉터리를 알아옵니다.
        /// </summary>
        /// <param name="currentDirectory"> 복사해야할 target파일의 root폴더를 제외한 상위 폴더들</param>
        /// <param name="ftpRootPath"> 상위 폴더들을 생성해야되는 root폴더 위치 </param>
        /// <returns></returns>
        private string GetParentDirectory(string ftpRootPath, string currentDirectory)
        {
            string[] directorys = currentDirectory.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
            string parentDirectory = string.Empty;
            for (int i = 0; i < directorys.Length - 1; i++)
            {
                IsExtistDirectory(ftpRootPath + parentDirectory + "/" +directorys[i], directorys[i]);
                parentDirectory += "/" + directorys[i];
                
            }


            return parentDirectory;
        }

        /// <summary>
        /// 인증을 가져옵니다.
        /// </summary>
        /// <returns>인증</returns>
        private ICredentials GetCredentials()
        {
            return new NetworkCredential(this.UserName, this.Password);
        }

        private string GetStringResponse(FtpWebRequest ftp)
        {
            string result = "";
            using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
            {
                long size = response.ContentLength;
                using (Stream datastream = response.GetResponseStream())
                {
                    if (datastream != null)
                    {
                        using (StreamReader sr = new StreamReader(datastream))
                        {
                            result = sr.ReadToEnd();
                            sr.Close();
                        }

                        datastream.Close();
                    }
                }

                response.Close();
            }

            return result;
        }

        private FtpWebRequest GetRequest(string URI)
        {
            FtpWebRequest result = (FtpWebRequest)WebRequest.Create(URI);
            result.Credentials = GetCredentials();
            result.KeepAlive = false;
            return result;
        }

        /// <summary>
        /// FTP에 해당 디렉터리를 만든다.
        /// </summary>
        /// <param name="dirpath"></param>
        private bool MakeDirectory(string dirpath)
        {
            try
            {
                Uri myUri = new Uri(dirpath);
                WebRequest request = WebRequest.Create(myUri);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                request.Credentials = new NetworkCredential(UserName, Password);
                using (var resp = (FtpWebResponse)request.GetResponse())
                {
                    Console.WriteLine(resp.StatusCode);
                    Console.WriteLine("FTP Create Folder : " + dirpath);
                }

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return true;
        }

        /// <summary>
        /// 지정한 로컬 파일이 실제 존재하는지 확인합니다.
        /// </summary>
        /// <param name="localFileFullPath">로컬 파일의 전체 경로입니다.</param>
        private void LocalFileValidationCheck(string localFileFullPath)
        {
            if (!File.Exists(localFileFullPath))
            {
                throw new FileNotFoundException(string.Format("지정한 로컬 파일이 없습니다.\n경로 : {0}", localFileFullPath));
            }
        }


        #endregion
    }
}
