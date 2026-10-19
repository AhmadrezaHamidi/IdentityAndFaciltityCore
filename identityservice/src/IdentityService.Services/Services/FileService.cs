namespace IdentityService.Services.Services
{
    public class FileService : BaseService, IFileService
    {
        private readonly ILogger<FileService> _logger;
        private readonly MediaCenterServerSettingsViewModel _mediaCenterServerSettings;

        public FileService(IOptions<MediaCenterServerSettingsViewModel> mediaCenterServerSettings, ILogger<FileService> logger)
        {
            _mediaCenterServerSettings = mediaCenterServerSettings.Value;
            _logger = logger;
        }

        public void SaveFileSync(LocalFileParameterViewModel fileParameters, CancellationToken cancellationToken)
        {
            if (_mediaCenterServerSettings.IsMediaCenterServer)
            {
                fileParameters.DirectoryPath = Path.Combine(_mediaCenterServerSettings.MediaCenterBaseFolderPath, fileParameters.DirectoryPath);

                //Create Path
                string filePath = Path.Combine(fileParameters.DirectoryPath, $"{fileParameters.FileName}");
                if (!fileParameters.FileName.Contains("."))
                    filePath = Path.Combine(fileParameters.DirectoryPath, $"{fileParameters.FileName}.{fileParameters.Extension}");

                //Create Directory
                var directoryPath = Path.GetDirectoryName(filePath);

                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);

                //Save File
                if (fileParameters.File != null)
                {
                    var stream = new FileStream(filePath, FileMode.Create);
                    fileParameters.File.CopyTo(stream);
                    stream.Flush();
                }
                else if (fileParameters.FileBytes != null)
                {
                    //Save File
                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(fileParameters.FileBytes, 0, fileParameters.FileBytes.Length);
                    }
                }
                else if (!string.IsNullOrEmpty(fileParameters.FileString))
                {
                    //Create Path
                    var bytes = Convert.FromBase64String(fileParameters.FileString);

                    //Save File
                    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(bytes, 0, bytes.Length);
                    }
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Not Found"); ;
                }
            }
            else
            {

                byte[] fileBytes = null;

                //Save File
                if (fileParameters.File != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        fileParameters.File.CopyTo(ms);
                        fileBytes = ms.ToArray();
                    }
                }
                else if (fileParameters.FileBytes != null)
                {
                    //Save File
                    fileBytes = fileParameters.FileBytes;
                }
                else if (!string.IsNullOrEmpty(fileParameters.FileString))
                {
                    //Create Path
                    fileBytes = Convert.FromBase64String(fileParameters.FileString);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Not Found"); ;
                }

                //await _userLogGrain.Add("Log", 1, 1, 1, "Start Save File To Remove Server:" + (_mediaCenterServerSettings.MediaCenterServerAddress + fileParameters.RemoteDirectoryPath), 1);
                SaveFtpfile(fileParameters.RemoteDirectoryPath, fileParameters.BaseFileNameWithExtension, fileBytes, cancellationToken);
            }
        }

        public async Task<MemoryStream> DownloadLocalFileMemoryStream(string filePath, CancellationToken cancellationToken)
        {
            //Read image into memory stream
            var memoryStream = new MemoryStream();
            await using var stream = new FileStream(filePath, FileMode.Open);
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task<byte[]> DownloadLocalFileByteArray(string filePath, CancellationToken cancellationToken)
        {
            //Read image into memory stream
            var memoryStream = new MemoryStream();
            await using var stream = new FileStream(filePath, FileMode.Open);
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream.ToArray();
        }

        public async Task<string> ByteToDisplaySize(long value, int decimalPlaces, CancellationToken cancellationToken)
        {
            string[] SizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            if (decimalPlaces < 0)
                throw new ArgumentOutOfRangeException("decimalPlaces");
            if (value < 0)
                return "-" + await ByteToDisplaySize(-value, decimalPlaces, cancellationToken);
            if (value == 0)
                return string.Format("{0:n" + decimalPlaces + "} bytes", 0);

            // mag is 0 for bytes, 1 for KB, 2, for MB, etc.
            int mag = (int)Math.Log(value, 1024);

            // 1L << (mag * 10) == 2 ^ (10 * mag) 
            // [i.e. the number of bytes in the unit corresponding to mag]
            decimal adjustedSize = (decimal)value / (1L << mag * 10);

            // make adjustment when the value is large enough that
            // it would round up to 1000 or more
            if (Math.Round(adjustedSize, decimalPlaces) >= 1000)
            {
                mag += 1;
                adjustedSize /= 1024;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", adjustedSize, SizeSuffixes[mag]);
        }

        public async Task<Enums.FileTypesEnum> GetFileType(string mimeType, CancellationToken cancellationToken)
        {
            if (mimeType.ToLower().Contains("image"))
            {
                return Enums.FileTypesEnum.Image;
            }
            else if (mimeType.ToLower().Contains("audio"))
            {
                return Enums.FileTypesEnum.Audio;
            }
            else if (mimeType.ToLower().Contains("video"))
            {
                return Enums.FileTypesEnum.Video;
            }
            else if (mimeType.ToLower().Contains("pdf"))
            {
                return Enums.FileTypesEnum.Pdf;
            }
            else if (mimeType.ToLower().Contains("word"))
            {
                return Enums.FileTypesEnum.Word;
            }
            else if (mimeType.ToLower().Contains("excel"))
            {
                return Enums.FileTypesEnum.Excel;
            }
            else if (mimeType.ToLower().Contains("powerpoint"))
            {
                return Enums.FileTypesEnum.PowerPoint;
            }
            else if (mimeType.ToLower().Contains("zip"))
            {
                return Enums.FileTypesEnum.Zip;
            }
            else if (mimeType.ToLower().Contains("rar"))
            {
                return Enums.FileTypesEnum.Rar;
            }
            else if (mimeType.ToLower().Contains("text"))
            {
                return Enums.FileTypesEnum.Text;
            }
            else
            {
                return Enums.FileTypesEnum.Other;
            }
        }

        public async Task<string> GetMimeType(string fileName, CancellationToken cancellationToken)
        {
            var provider = new FileExtensionContentTypeProvider();

            if (!provider.TryGetContentType(fileName, out var contentType))
                contentType = string.Empty;

            return contentType;
        }

        public async Task<string> GetFileExtension(string fileName, CancellationToken cancellationToken)
        {
            return Path.GetExtension(fileName).TrimStart('.');
        }

        public async Task<string> GetFileBaseUrl(string fileUsageType, CancellationToken cancellationToken)
        {
            return $"{_mediaCenterServerSettings.OnlineMediaCenterAdress}/files/{fileUsageType}/".ToLower();
        }

        public async Task<string> GetFileFullUrl(string fileUsageType, string fileNameWithExtension, int? fileId, CancellationToken cancellationToken)
        {
            return $"{_mediaCenterServerSettings.OnlineMediaCenterAdress}/files/{fileUsageType}/{fileNameWithExtension}".ToLower();
        }

        private void CopyStream(Stream input, Stream output, CancellationToken cancellationToken)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public async Task<MemoryStream> DownloadFile(string filePath, CancellationToken cancellationToken)
        {
            //Read File into memory stream
            var memoryStream = new MemoryStream();
            await using var stream = new FileStream(filePath, FileMode.Open);
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            return memoryStream;
        }

        public void SaveFtpfile(string ftpfilepath, string fileName, byte[] fileBytes, CancellationToken cancellationToken)
        {
            string ftpfullpath = _mediaCenterServerSettings.MediaCenterServerAddress + (ftpfilepath + "/" + fileName);
            if (CreateFTPDirectory(ftpfilepath, cancellationToken))
            {
                /* Create an FTP Request */
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(ftpfullpath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                if (!string.IsNullOrEmpty(_mediaCenterServerSettings.MediaCenterServerUserName))
                    ftpRequest.Credentials = new NetworkCredential(_mediaCenterServerSettings.MediaCenterServerUserName, _mediaCenterServerSettings.MediaCenterServerPassword);
                /* When in doubt, use these options */
                ftpRequest.UseBinary = true;
                //ftpRequest.EnableSsl = true;
                ftpRequest.UsePassive = true;
                ftpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                ftpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Establish Return Communication with the FTP Server */
                Stream ftpStream = ftpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */

                ftpStream.Write(fileBytes, 0, fileBytes.Length);

                ftpStream.Close();
                ftpRequest = null;
            }
            else
            {
                throw new Exception("Folder not exists on remote server...");
            }

        }

        private bool CreateFTPDirectory(string pathToCreate, CancellationToken cancellationToken)
        {
            try
            {
                FtpWebRequest reqFTP = null;
                Stream ftpStream = null;

                string[] subDirs = pathToCreate.Split('/');

                string currentDir = _mediaCenterServerSettings.MediaCenterServerAddress;

                foreach (string subDir in subDirs)
                {
                    try
                    {
                        currentDir = currentDir + "/" + subDir;
                        reqFTP = (FtpWebRequest)FtpWebRequest.Create(currentDir);
                        reqFTP.Method = WebRequestMethods.Ftp.MakeDirectory;
                        reqFTP.UseBinary = true;
                        if (!string.IsNullOrEmpty(_mediaCenterServerSettings.MediaCenterServerUserName))
                            reqFTP.Credentials = new NetworkCredential(_mediaCenterServerSettings.MediaCenterServerUserName, _mediaCenterServerSettings.MediaCenterServerPassword);

                        FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                        ftpStream = response.GetResponseStream();
                        ftpStream.Close();
                        response.Close();
                    }
                    catch (Exception exp)
                    {
                        //directory already exist I know that is weak but there is no way to check if a folder exist on ftp...
                    }
                }
                return true;

            }
            catch (WebException ex)
            {
                FtpWebResponse response = (FtpWebResponse)ex.Response;
                if (response.StatusCode == FtpStatusCode.ActionNotTakenFileUnavailable)
                {
                    response.Close();
                    return true;
                }
                else
                {
                    response.Close();
                    return false;
                }
            }
        }

        private void UploadFilesToServer(Uri uri, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData, CancellationToken cancellationToken)
        {
            string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
            httpWebRequest.ContentType = "multipart/form-data; boundary=" + boundary;
            httpWebRequest.Method = "POST";
            httpWebRequest.BeginGetRequestStream((result) =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)result.AsyncState;
                    using (Stream requestStream = request.EndGetRequestStream(result))
                    {
                        WriteMultipartForm(requestStream, boundary, data, fileName, fileContentType, fileData, cancellationToken);
                    }
                    request.BeginGetResponse(a =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(a);
                            var responseStream = response.GetResponseStream();
                            using (var sr = new StreamReader(responseStream))
                            {
                                using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
                                {
                                    string responseString = streamReader.ReadToEnd();
                                    //responseString is depend upon your web service.
                                    if (responseString == "Success")
                                    {
                                    }
                                    else
                                    {
                                        throw new Exception("Send File:" + responseString);
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }
                    }, null);
                }
                catch (Exception)
                {

                }
            }, httpWebRequest);
        }

        private void WriteMultipartForm(Stream s, string boundary, Dictionary<string, string> data, string fileName, string fileContentType, byte[] fileData, CancellationToken cancellationToken)
        {
            /// The first boundary
            byte[] boundarybytes = Encoding.UTF8.GetBytes("--" + boundary + "\r\n");
            /// the last boundary.
            byte[] trailer = Encoding.UTF8.GetBytes("\r\n--" + boundary + "--\r\n");
            /// the form data, properly formatted
            string formdataTemplate = "Content-Dis-data; name=\"{0}\"\r\n\r\n{1}";
            /// the form-data file upload, properly formatted
            string fileheaderTemplate = "Content-Dis-data; name=\"{0}\"; filename=\"{1}\";\r\nContent-Type: {2}\r\n\r\n";

            /// Added to track if we need a CRLF or not.
            bool bNeedsCRLF = false;

            if (data != null)
            {
                foreach (string key in data.Keys)
                {
                    /// if we need to drop a CRLF, do that.
                    if (bNeedsCRLF)
                        WriteToStream(s, "\r\n");

                    /// Write the boundary.
                    WriteToStream(s, boundarybytes);

                    /// Write the key.
                    WriteToStream(s, string.Format(formdataTemplate, key, data[key]));
                    bNeedsCRLF = true;
                }
            }

            /// If we don't have keys, we don't need a crlf.
            if (bNeedsCRLF)
                WriteToStream(s, "\r\n");

            WriteToStream(s, boundarybytes);
            WriteToStream(s, string.Format(fileheaderTemplate, "file", fileName, fileContentType));
            /// Write the file data to the stream.
            WriteToStream(s, fileData);
            WriteToStream(s, trailer);
        }

        private void WriteToStream(Stream s, string txt)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(txt);
            s.Write(bytes, 0, bytes.Length);
        }

        private void WriteToStream(Stream s, byte[] bytes)
        {
            s.Write(bytes, 0, bytes.Length);
        }

        public async Task<bool> CheckInvitedCode(string InvitedCode)
        {
            try
            {
                string jsonInvitedCode = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + @"/InvitedCodes.json");
                var values = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonInvitedCode,JsonHelper.JsonCamelCaseOption());
                foreach (var item in values)
                    if (item.Value.ToUpper().Replace(" ", "").Contains(InvitedCode))
                        return true;
                return false;
            }
            catch (Exception exp)
            {
                _logger.LogError(exp.Message);
                return false;
            }
        }
    }
}
