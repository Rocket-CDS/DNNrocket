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
        public void ProcessRequest(HttpContext context)
        {
            lock (_lock) // we need to lock to stop race condition when the same image is on the page mulitple times.
            {

                var w = DNNrocketUtils.RequestQueryStringParam(context, "w");
                var h = DNNrocketUtils.RequestQueryStringParam(context, "h");
                var src = DNNrocketUtils.RequestQueryStringParam(context, "src");
                var imgtype = DNNrocketUtils.RequestQueryStringParam(context, "imgtype").ToLower();

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
                var cacheimgtype = (string)CacheUtils.GetCache(strCacheKey + "imgtype", "DNNrocketThumb");
                if (!String.IsNullOrEmpty(cacheimgtype)) imgtype = cacheimgtype;

                //IMPORTANT: If you need to delete the image file you MUST remove the cache first.
                //The cache holds a link to the locked image file and must be disposed.
                //use: ClearThumbnailLock()

                if (newImage == null)
                {
                    // Resize does not support trnasparency in webp, workaround to output png.
                    var rtnImgMapPath = ImgUtils.ThumbMapPath(src);
                    if (src != rtnImgMapPath)
                    {
                        src = rtnImgMapPath;
                        imgtype = Path.GetExtension(src).Trim('.');
                        CacheUtils.SetCache(strCacheKey + "imgtype", imgtype, "DNNrocketThumb");
                    }
                    newImage = ImgUtils.CreateThumbnail(src, Convert.ToInt32(w), Convert.ToInt32(h), imgtype);
                    CacheUtils.SetCache(strCacheKey, newImage, "DNNrocketThumb");
                }

                if (newImage != null)
                {
                    if (imgtype == "") imgtype = Path.GetExtension(src).Trim('.');
                    ImageCodecInfo useEncoder = ImgUtils.GetEncoder(ImageFormat.Jpeg);
                    if (imgtype.ToLower() == "png")
                    {
                        useEncoder = ImgUtils.GetEncoder(ImageFormat.Png);
                        context.Response.ContentType = "image/png";
                    }
                    else if (imgtype.ToLower() == "webp")
                    {
                        context.Response.ContentType = "image/webp";
                    }
                    else
                    {
                        context.Response.ContentType = "image/jpeg";
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