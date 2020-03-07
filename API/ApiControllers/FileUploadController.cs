using DNNrocketAPI.Componants;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using Simplisity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DNNrocketAPI.ApiControllers
{
    public class FileUploadController : DnnApiController
    {

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task<IHttpActionResult> Upload(HttpRequestMessage request)
        {
            if (!Directory.Exists(DNNrocketUtils.TempDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.TempDirectoryMapPath());
            if (!Directory.Exists(DNNrocketUtils.HomeDNNrocketDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.HomeDNNrocketDirectoryMapPath());

            if (request.IsChunkUpload())
            {
                var uploadFileService = new UploadFileService();
                UploadProcessingResult uploadResult = await uploadFileService.HandleRequest(Request);

                if (uploadResult.IsComplete)
                {
                    // do other stuff here after file upload complete    
                    return Ok();
                }

                return Ok(HttpStatusCode.Continue);
            }
            else
            {
                var data = await Request.Content.ParseMultipartAsync();
                var userid = DNNrocketUtils.GetCurrentUserId();
                foreach (var f in data.Files)
                {
                    if (f.Value.File.Length > 0)
                    {
                        var fileName = f.Value.Filename;
                        FileUtils.SaveFile(DNNrocketUtils.TempDirectoryMapPath() + "\\" + userid + "_" + fileName, f.Value.File);
                    }
                }
                return Ok();
            }

        }

    }

    //public async Task<HttpResponseMessage> Upload(HttpRequestMessage request)
    //    {
    //        if (!Directory.Exists(DNNrocketUtils.TempDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.TempDirectoryMapPath());
    //        if (!Directory.Exists(DNNrocketUtils.HomeDNNrocketDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.HomeDNNrocketDirectoryMapPath());
    //        var fileuploadPath = DNNrocketUtils.TempDirectoryMapPath();

    //        if (!request.Content.IsMimeMultipartContent()) throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

    //            var data = await Request.Content.ParseMultipartAsync();
    //            var userid = DNNrocketUtils.GetCurrentUserId();

    //            foreach (var f in data.Files)
    //            {
    //                if (f.Value.File.Length > 0)
    //                {
    //                    var fileName = f.Value.Filename;
    //                    FileUtils.SaveFile(fileuploadPath + "\\" + userid + "_" + fileName, f.Value.File);
    //                }
    //            }

    //        return new HttpResponseMessage(HttpStatusCode.OK);
    //    }


   // }
    


    public class UploadFileService
    {
        private readonly string _uploadPath;
        private readonly MultipartFormDataStreamProvider _streamProvider;

        public UploadFileService()
        {
            _uploadPath = UserLocalPath;
            _streamProvider = new MultipartFormDataStreamProvider(_uploadPath);
        }

        #region Interface

        public async Task<UploadProcessingResult> HandleRequest(HttpRequestMessage request)
        {
            await request.Content.ReadAsMultipartAsync(_streamProvider);
            return await ProcessFile(request);
        }

        #endregion    

        #region Private implementation

        private async Task<UploadProcessingResult> ProcessFile(HttpRequestMessage request)
        {
            if (request.IsChunkUpload())
            {
                return await ProcessChunk(request);
            }

            return new UploadProcessingResult()
            {
                IsComplete = true,
                FileName = OriginalFileName,
                LocalFilePath = LocalFileName,
                FileMetadata = _streamProvider.FormData
            };
        }

        private async Task<UploadProcessingResult> ProcessChunk(HttpRequestMessage request)
        {
            //use the unique identifier sent from client to identify the file
            var userid = DNNrocketUtils.GetCurrentUserId();
            FileChunkMetaData chunkMetaData = request.GetChunkMetaData();
            string filePath = Path.Combine(_uploadPath, string.Format("{0}", userid + "_" + chunkMetaData.ChunkIdentifier));

            //append chunks to construct original file
            using (FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate | FileMode.Append))
            {
                var localFileInfo = new FileInfo(LocalFileName);
                var localFileStream = localFileInfo.OpenRead();

                await localFileStream.CopyToAsync(fileStream);
                await fileStream.FlushAsync();

                fileStream.Close();
                localFileStream.Close();

                //delete chunk
                localFileInfo.Delete();
            }

            return new UploadProcessingResult()
            {
                IsComplete = chunkMetaData.IsLastChunk,
                FileName = OriginalFileName,
                LocalFilePath = chunkMetaData.IsLastChunk ? filePath : null,
                FileMetadata = _streamProvider.FormData
            };

        }

        #endregion    

        #region Properties

        private string LocalFileName
        {
            get
            {
                MultipartFileData fileData = _streamProvider.FileData.FirstOrDefault();
                return fileData.LocalFileName;
            }
        }

        private string OriginalFileName
        {
            get
            {
                MultipartFileData fileData = _streamProvider.FileData.FirstOrDefault();
                return fileData.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
            }
        }

        private string UserLocalPath
        {
            get
            {
                return DNNrocketUtils.TempDirectoryMapPath();
            }
        }

        #endregion    
    }


    public static class HttpRequestMessageExtensions
    {
        public static bool IsChunkUpload(this HttpRequestMessage request)
        {
            return request.Content.Headers.ContentRange != null;
        }

        public static FileChunkMetaData GetChunkMetaData(this HttpRequestMessage request)
        {
            return new FileChunkMetaData()
            {
                ChunkIdentifier = request.Headers.GetValues("X-File-Identifier").FirstOrDefault(),
                ChunkStart = request.Content.Headers.ContentRange.From,
                ChunkEnd = request.Content.Headers.ContentRange.To,
                TotalLength = request.Content.Headers.ContentRange.Length
            };
        }
    }

    public class FileChunkMetaData
    {
        public string ChunkIdentifier { get; set; }
        public long? ChunkStart { get; set; }
        public long? ChunkEnd { get; set; }
        public long? TotalLength { get; set; }
        public bool IsLastChunk
        {
            get { return ChunkEnd + 1 >= TotalLength; }
        }
    }

    public class UploadProcessingResult
    {
        public bool IsComplete { get; set; }
        public string FileName { get; set; }
        public string LocalFilePath { get; set; }
        public NameValueCollection FileMetadata { get; set; }
    }

}
