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

        public void ProcessRequest(HttpContext context)
        {

            var w = DNNrocketUtils.RequestQueryStringParam(context, "w");
            var h = DNNrocketUtils.RequestQueryStringParam(context, "h");
            var src = DNNrocketUtils.RequestQueryStringParam(context, "src");
            var imgtype = DNNrocketUtils.RequestQueryStringParam(context, "imgtype");

            src = "/" + src.TrimStart('/'); // ensure a valid rel path.

            if (h == "") h = "0";
            if (w == "") w = "0";

            if (GeneralUtils.IsNumeric(w) && GeneralUtils.IsNumeric(h))
            {
                if (!GeneralUtils.IsAbsoluteUrl(src)) src = HttpContext.Current.Server.MapPath(src);

                var strCacheKey = context.Request.Url.Host.ToLower() + "*" + src + "*" + DNNrocketUtils.GetCurrentCulture() + "*img:" + w + "*" + h + "*";

                context.Response.Clear();
                context.Response.ClearHeaders();
                context.Response.AddFileDependency(src);
                context.Response.Cache.SetETagFromFileDependencies();
                context.Response.Cache.SetLastModifiedFromFileDependencies();
                context.Response.Cache.SetCacheability(HttpCacheability.Public);

                if (imgtype.ToLower() == "webp")
                {
                    try
                    {
                        var cacheDir = PortalUtils.TempDirectoryMapPath().Trim('\\') + "\\cache";
                        var cacheFile = cacheDir + "\\" + GeneralUtils.GetMd5Hash(strCacheKey);
                        if (File.Exists(src))
                        {
                            byte[] bytes;
                            if (!File.Exists(cacheFile))
                            {
                                if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);
                                ImgUtils.CreateThumbOnDisk(src, w + "," + h, cacheFile, true);
                            }
                            bytes = System.IO.File.ReadAllBytes(cacheFile);

                            context.Response.ContentType = "image/webp";
                            context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                        }
                        else
                        {
                            File.Delete(cacheFile);
                        }
                    }
                    catch (Exception exc)
                    {
                        var outArray = GeneralUtils.StrToByteArray(exc.ToString());
                        context.Response.BinaryWrite(outArray);
                    }
                }
                else
                {

                    var newImage = (Bitmap)CacheUtils.GetCache(strCacheKey, "DNNrocketThumb");
                    //IMPORTANT: If you need to delete the image file you MUST remove the cache first.
                    //The cache holds a link to the locked image file and must be disposed.
                    //use: ClearThumbnailLock()

                    if (newImage == null)
                    {
                        newImage = ImgUtils.CreateThumbnail(src, Convert.ToInt32(w), Convert.ToInt32(h));
                        CacheUtils.SetCache(strCacheKey, newImage, "DNNrocketThumb");
                    }

                    if ((newImage != null))
                    {
                        ImageCodecInfo useEncoder = ImgUtils.GetEncoder(ImageFormat.Jpeg);

                        // due to issues on some servers not outputing the png format correctly from the thumbnailer.
                        // this thumbnailer will always output jpg, unless specifically told to do a png format.
                        if (imgtype.ToLower() == "png")
                        {
                            useEncoder = ImgUtils.GetEncoder(ImageFormat.Png);
                            context.Response.ContentType = "image/png";
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