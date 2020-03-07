using DNNrocketAPI.Componants;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using Newtonsoft.Json;
using Simplisity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DNNrocketAPI.ApiControllers
{
    public class FileUploadLargeController : ApiController
    {

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task<string> Upload()
        {
            try
            {
                HttpRequestMessage request = this.Request;

                if (!Directory.Exists(DNNrocketUtils.TempDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.TempDirectoryMapPath());
                if (!Directory.Exists(DNNrocketUtils.HomeDNNrocketDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.HomeDNNrocketDirectoryMapPath());
                var fileuploadPath = DNNrocketUtils.TempDirectoryMapPath();

                if (requestBase.GetBufferedInputStream().Position > 0)
                {
                    // If GetBufferedInputStream() was completely read, we can continue accessing it via Request.InputStream.
                    // If it was partially read, accessing InputStream will throw, but at that point we have no
                    // way of recovering.
                    requestBase.InputStream.Position = 0;
                    return requestBase.InputStream;
                }

                var provider = new MultipartFormDataStreamProvider(fileuploadPath);
                var content = new StreamContent(Request.Content.)
                {
                    content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }

                await content.ReadAsMultipartAsync(provider);


                var userid = DNNrocketUtils.GetCurrentUserId();
                string uploadingFileName = provider.FileData.Select(x => x.LocalFileName).FirstOrDefault();
                string originalFileName = String.Concat(fileuploadPath, "\\" + userid + "_" + (provider.Contents[0].Headers.ContentDisposition.FileName).Trim(new Char[] { '"' }));
                if (File.Exists(originalFileName)) File.Delete(originalFileName);
                File.Move(uploadingFileName, originalFileName);

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }

        }



    }

}
