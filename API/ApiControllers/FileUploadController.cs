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
    public class FileUploadController : DnnApiController
    {

        [AllowAnonymous]
        [HttpGet]
        [HttpPost]
        public async Task<HttpResponseMessage> Upload(HttpRequestMessage request)
        {
            if (!Directory.Exists(DNNrocketUtils.TempDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.TempDirectoryMapPath());
            if (!Directory.Exists(DNNrocketUtils.HomeDNNrocketDirectoryMapPath())) Directory.CreateDirectory(DNNrocketUtils.HomeDNNrocketDirectoryMapPath());
            var fileuploadPath = DNNrocketUtils.TempDirectoryMapPath();

            if (!request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);

                var data = await Request.Content.ParseMultipartAsync();

                foreach (var f in data.Files)
                {
                    if (f.Value.File.Length > 0)
                    {
                        var fileName = f.Value.Filename;
                        FileUtils.SaveFile(fileuploadPath + "\\" + fileName, f.Value.File);
                    }
                }

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

    }

}
