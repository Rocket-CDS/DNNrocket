using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using Simplisity;
using static System.Net.Mime.MediaTypeNames;
using ThoughtWorks.QRCode.Codec;
using System.Net;
using ImageMagick;
using System.Web;
using System.Xml.Linq;
using System.Runtime.InteropServices.ComTypes;

// [TODO: Convert to https://github.com/dlemstra/Magick.NET]
// This is already using Magick, but it has been integrated for Webp and merged into old code that was created before webp and Magik existed.

namespace RocketUtils
{
    public static class ImgUtils
    {
        public static void ProcessResizeImage(string inputPath, string outputPath, int width, int height)
        {
            using (var img = new MagickImage(inputPath))
            {
                if (img.Height != height || img.Width != width)
                {
                    var displayExtension = Path.GetExtension(inputPath).ToLower();

                    decimal result_ratio = (decimal)height / (decimal)width;
                    decimal current_ratio = (decimal)img.Height / (decimal)img.Width;

                    Boolean preserve_width = false;
                    if (current_ratio > result_ratio)
                    {
                        preserve_width = true;
                    }
                    int new_width = 0;
                    int new_height = 0;
                    if (preserve_width)
                    {
                        new_width = width;
                        new_height = (int)Math.Round((decimal)(current_ratio * new_width));
                    }
                    else
                    {
                        new_height = height;
                        new_width = (int)Math.Round((decimal)(new_height / current_ratio));
                    }


                    String geomStr = width.ToString() + "x" + height.ToString();
                    String newGeomStr = new_width.ToString() + "x" + new_height.ToString();

                    var intermediate_geo = new MagickGeometry(newGeomStr);
                    var final_geo = new MagickGeometry(geomStr);
                    if (displayExtension == ".png")
                        img.Format = MagickFormat.Png;
                    else
                        img.Format = MagickFormat.WebP;
                    img.Resize(intermediate_geo);
                    img.Crop(final_geo);
                    img.Write(outputPath);
                }
            }
        }

        public static List<string> MoveImageToFolder(int userid, SimplisityInfo postInfo, string destinationFolder, string tempDirectoryMapPath, int maxImages = 50)
        {
            destinationFolder = destinationFolder.TrimEnd('\\');
            var rtn = new List<string>();
            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
            var createseo = postInfo.GetXmlPropertyBool("genxml/hidden/createseo");
            var resize = postInfo.GetXmlPropertyInt("genxml/hidden/imageresize");
            if (resize == 0) resize = 640;
            var fileuploadlist = postInfo.GetXmlProperty("genxml/hidden/fileuploadlist");
            if (fileuploadlist != "")
            {
                var imageCount = 1;
                foreach (var f in fileuploadlist.Split(';'))
                {
                    if (f != "")
                    {
                        var friendlyname = GeneralUtils.DeCode(f);
                        var userfilename = userid + "_" + friendlyname;
                        if (imageCount <= maxImages)
                        {
                            var ext = ".webp";
                            if (Path.GetExtension(friendlyname).ToLower() == ".png") ext = ".png";
                            var unqName = RocketUtils.GetUniqueFileName(GeneralUtils.UrlFriendly(Path.GetFileNameWithoutExtension(friendlyname)) + ext, destinationFolder);
                            var outputMapPath = destinationFolder + "\\" + unqName;

                            ProcessResizeImage(tempDirectoryMapPath + "\\" + userfilename, outputMapPath, resize, 0); 

                            if (File.Exists(outputMapPath))
                            {
                                rtn.Add(unqName);
                                imageCount += 1;
                            }
                        }
                        File.Delete(tempDirectoryMapPath + "\\" + userfilename);
                    }
                }
            }
            return rtn;
        }
        public static bool IsImageFile(FileInfo sFileInfo)
        {
            return IsImageFile(sFileInfo.Extension);
        }
        public static bool IsImageFile(string strExtension)
        {
            return strExtension.ToLower() == ".webp" ||
                strExtension.ToLower() == ".jpg" ||
                strExtension.ToLower() == ".jpeg" ||
                strExtension.ToLower() == ".gif" ||
                strExtension.ToLower() == ".png" ||
                strExtension.ToLower() == ".tiff" ||
                strExtension.ToLower() == ".bmp" ||
                strExtension.ToLower() == ".avif" ||
                strExtension.ToLower() == ".jfif";
        }
        public static string ConvertWebpToJpg(string imgPathName)
        {
            var outFileMapPath = Path.GetDirectoryName(imgPathName) + "\\" + Path.GetFileNameWithoutExtension(imgPathName) + ".jpg";
            using (MagickImage image = new MagickImage(imgPathName))
            {
                image.Format = MagickFormat.Jpg;
                image.Write(outFileMapPath);
            }
            return outFileMapPath;
        }
        public static bool ContainsTransparent(string MapPath)
        {
            if (File.Exists(MapPath))
            {
                try
                {
                    Bitmap image = new Bitmap(MapPath);
                    for (int y = 0; y < image.Height; ++y)
                    {
                        for (int x = 0; x < image.Width; ++x)
                        {
                            if (image.GetPixel(x, y).A != 255)
                            {
                                return true;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }
        public static string ConvertToWebp(string imgPathName, string outFileMapPath = "")
        {
            if (outFileMapPath == "") outFileMapPath = Path.GetDirectoryName(imgPathName) + "\\" + Path.GetFileNameWithoutExtension(imgPathName) + ".webp";
            if (Path.GetExtension(imgPathName.ToLower()) == ".webp")
            {
                if (imgPathName != outFileMapPath) File.Copy(imgPathName, outFileMapPath);
            }
            else
            {
                try
                {
                    using (MagickImage image = new MagickImage(imgPathName))
                    {
                        image.Format = MagickFormat.WebP;
                        image.Write(outFileMapPath);
                    }
                }
                catch (Exception)
                {
                    GC.Collect(); // attempt to clear all file locks and try again
                    try
                    {
                        using (MagickImage image = new MagickImage(imgPathName))
                        {
                            image.Format = MagickFormat.WebP;
                            image.Write(outFileMapPath);
                        }
                    }
                    catch
                    {
                        //abandon save. 
                    }
                }
            }
            return outFileMapPath;
        }
        public static List<string> UploadBase64Image(string[] filenameList, string[] filebase64List, string tempFileMapPath, string imageFolderMapPath, int size)
        {
            var rtn = new List<string>();
            if (filebase64List.Length == filenameList.Length && filenameList.Length > 0)
            {
                var lp = 0;
                foreach (var ncode in filenameList)
                {
                    var fname = GeneralUtils.DeCode(ncode);
                    var fext = Path.GetExtension(fname).ToLower();
                    if (IsImageFile(fext) || fext == ".svg")
                    {
                        var fbase64 = filebase64List[lp];
                        fbase64 = fbase64.Split(',')[1];
                        var bytes = Convert.FromBase64String(fbase64.Replace(" ","+")); // string altered during call. Fix with replace.
                        using (var imageFile = new FileStream(tempFileMapPath, FileMode.Create))
                        {
                            imageFile.Write(bytes, 0, bytes.Length);
                            imageFile.Flush();
                        }
                        if (fext == ".svg")
                        {
                            var newfilename = imageFolderMapPath + "\\" + FileUtils.RemoveInvalidFileChars(GeneralUtils.GetGuidKey() + Path.GetExtension(fname));
                            File.Move(tempFileMapPath, newfilename);
                            rtn.Add(newfilename);
                        }
                        else
                        {
                            var ext = ".webp";
                            if (Path.GetExtension(fname).ToLower() == ".png") ext = ".png";
                            var newfilename = imageFolderMapPath + "\\" + FileUtils.RemoveInvalidFileChars(GeneralUtils.GetGuidKey() + ext);
                            ProcessResizeImage(tempFileMapPath, newfilename, size, 0);
                            rtn.Add(newfilename);
                        }

                        try
                        {
                            File.Delete(tempFileMapPath);
                        }
                        catch (Exception ex)
                        {
                            // ignore, no log + could be locked
                            //[TDO: Add log functionality]
                        }
                        lp += 1;
                    }
            }
            }
            return rtn;
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            string mimeType = "image/x-unknown";

            if (format.Equals(ImageFormat.Gif))
            {
                mimeType = "image/gif";
            }
            else if (format.Equals(ImageFormat.Jpeg))
            {
                mimeType = "image/jpeg";
            }
            else if (format.Equals(ImageFormat.Png))
            {
                mimeType = "image/png";
            }
            else if (format.Equals(ImageFormat.Bmp) || format.Equals(ImageFormat.MemoryBmp))
            {
                mimeType = "image/bmp";
            }
            else if (format.Equals(ImageFormat.Tiff))
            {
                mimeType = "image/tiff";
            }
            else if (format.Equals(ImageFormat.Icon))
            {
                mimeType = "image/x-icon";
            }

            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo e = encoders.FirstOrDefault(x => x.MimeType == mimeType);
            return e;
        }
        public static void CreateQRcode(string data, string fileMapPath)
        {
            if (data != "" && fileMapPath != "")
            {
                fileMapPath = Path.GetDirectoryName(fileMapPath).TrimEnd('\\') + "\\" + Path.GetFileNameWithoutExtension(fileMapPath) + ".jpg";
                if (File.Exists(fileMapPath)) File.Delete(fileMapPath);

                var qrCodeEncoder = new QRCodeEncoder();
                //qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
                //qrCodeEncoder.QRCodeScale = 16;
                //qrCodeEncoder.QRCodeVersion = 29;
                //qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;
                var newImage = qrCodeEncoder.Encode(data);

                var useEncoder = GetEncoder(ImageFormat.Jpeg);
                var encoderParameters = new EncoderParameters(1);
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 85L);

                try
                {
                    newImage.Save(fileMapPath, useEncoder, encoderParameters);
                }
                catch (Exception)
                {
                    GC.Collect();
                    // attempt to clear all file locks and try again
                    try
                    {
                        newImage.Save(fileMapPath, useEncoder, encoderParameters);
                    }
                    catch
                    {
                        //Assumption is the thumb already is there, but locked. So no need for error.
                    }

                }
            }
        }
        public static void DownloadAndSaveImage(string imageUrl, string filename)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(imageUrl);
            Bitmap bitmap; bitmap = new Bitmap(stream);

            if (bitmap != null)
            {
                bitmap.Save(filename, ImageFormat.Jpeg);
            }

            stream.Flush();
            stream.Close();
            client.Dispose();
        }
    }
}
