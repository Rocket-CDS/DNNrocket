using Newtonsoft.Json.Linq;
using Simplisity;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.UI.WebControls;

namespace DNNrocketAPI.Components
{
    public class DNNrocketThumb : IHttpHandler
    {

        private static object _lock = new object();
        /// <summary>
        ///
        /// - DEFAULT RULE: WEBP will always output as WEBP, PNG will always output as PNG.  JPG,JPEG will always output as WEBP.
        /// note: The default rule can be overwritten is the "imgtype" is passed.
        /// default rule set by FMC on 9/1/2024
        /// 
        /// - IMPORTANT: If you need to delete the image file you MUST remove the cache first.
        /// The cache holds a link to the locked image file and must be disposed.
        /// use: ClearThumbnailLock()
        /// 
        /// </summary>
        /// <param name="context"></param>
        public void ProcessRequest(HttpContext context)
        {

            lock (_lock) // we need to lock to stop race condition when the same image is on the page mulitple times.
            {

                var w = DNNrocketUtils.RequestQueryStringParam(context, "w");
                var h = DNNrocketUtils.RequestQueryStringParam(context, "h");
                var src = DNNrocketUtils.RequestQueryStringParam(context, "src");
                var imgtype = DNNrocketUtils.RequestQueryStringParam(context, "imgtype").ToLower();
                if (imgtype == "") imgtype = Path.GetExtension(src).Trim('.');

                src = "/" + src.TrimStart('/'); // ensure a valid rel path.

                if (!GeneralUtils.IsNumeric(h)) h = "0";
                if (!GeneralUtils.IsNumeric(w)) w = "0";

                if (!GeneralUtils.IsAbsoluteUrl(src)) src = HttpContext.Current.Server.MapPath(src);

                var strCacheKey = context.Request.Url.Host.ToLower() + "*" + src + "*" + DNNrocketUtils.GetCurrentCulture() + "*img:" + w + "*" + h + "*";

                context.Response.Clear();
                context.Response.ClearHeaders();
                context.Response.AddFileDependency(src);
                context.Response.Cache.SetETagFromFileDependencies();
                context.Response.Cache.SetLastModifiedFromFileDependencies();
                context.Response.Cache.SetCacheability(HttpCacheability.Public);

                var newImage = (Bitmap)CacheUtils.GetCache(strCacheKey, "DNNrocketThumb");

                if (newImage == null)
                {
                    if (imgtype == "jpg" || imgtype == "jpeg") imgtype = "webp"; // jpg only output webp, if not forced to another format.

                    var portalId = PortalUtils.GetCurrentPortalId();
                    var bitFileMapPath = Path.Combine(Path.GetDirectoryName(src), Path.GetFileNameWithoutExtension(src) + "_" + w + "_" + h + "." + imgtype);
                    if (!File.Exists(bitFileMapPath) && File.Exists(src))
                    {
                        newImage = RocketUtils.ImgUtils.CreateThumbnail(src, Convert.ToInt32(w), Convert.ToInt32(h), imgtype);
                        newImage.Save(bitFileMapPath);
                    }
                    else
                    {
                        var fi = new FileInfo(bitFileMapPath);
                        newImage = RocketUtils.ImgUtils.NewBitmap(fi);
                    }

                    CacheUtils.SetCache(strCacheKey, newImage, "DNNrocketThumb");
                }

                if (newImage != null)
                {
                    ImageCodecInfo useEncoder = RocketUtils.ImgUtils.GetEncoder(ImageFormat.Jpeg);
                    if (imgtype.ToLower() == "png")
                    {
                        useEncoder = RocketUtils.ImgUtils.GetEncoder(ImageFormat.Png);
                        context.Response.ContentType = "image/png";
                    }
                    else if (imgtype.ToLower() == "jpg" || imgtype.ToLower() == "jpeg")
                    {
                        context.Response.ContentType = "image/jpeg";
                    }
                    else
                    {
                        context.Response.ContentType = "image/webp";
                    }

                    var encoderParameters = new EncoderParameters(1);
                    encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 85L);

                    try
                    {
                        newImage.Save(context.Response.OutputStream, useEncoder, encoderParameters);
                    }
                    catch (Exception exc)
                    {
                        var outArray = GeneralUtils.StrToByteArray(exc.ToString());
                        context.Response.BinaryWrite(outArray);
                    }
                }
            }
        }

        public string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}