using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Simplisity;

using System;


// [TODO: Convert to https://github.com/dlemstra/Magick.NET]
// This is already using Magick, but it has been integrated for Webp and merged into old code that was created before webp and Magik existed.

namespace DNNrocketAPI.Components
{
    public static class ImgUtils
    {
        [Obsolete("Use RocketUtils.ImgUtils.MoveImageToFolder instead")]
        public static List<string> MoveImageToFolder(SimplisityInfo postInfo, string destinationFolder, int maxImages = 50)
        {
            var userid = UserUtils.GetCurrentUserId();
            return RocketUtils.ImgUtils.MoveImageToFolder(userid, postInfo, destinationFolder, PortalUtils.TempDirectoryMapPath(), maxImages);
        }
        [Obsolete("Use RocketUtils.ImgUtils.CopyImageForSEO instead")]
        public static void CopyImageForSEO(string sourceFilePath, string destinationfolder, string fileName = "")
        {
            RocketUtils.ImgUtils.CopyImageForSEO(sourceFilePath, destinationfolder, fileName);
        }
        [Obsolete("Use RocketUtils.ImgUtils.GetEncoder instead")]
        public static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            return RocketUtils.ImgUtils.GetEncoder(format);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static byte[] ImageToByteArray(string imagefilePath)
        {
            return RocketUtils.ImgUtils.ImageToByteArray(imagefilePath);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void ByteArrayToImageFilebyMemoryStream(byte[] imageByte, string fileMapPath)
        {
            RocketUtils.ImgUtils.ByteArrayToImageFilebyMemoryStream(imageByte, fileMapPath);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static bool IsImageFile(FileInfo sFileInfo)
        {
            return RocketUtils.ImgUtils.IsImageFile(sFileInfo);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static bool IsImageFile(string strExtension)
        {
            return RocketUtils.ImgUtils.IsImageFile(strExtension);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void AddWatermark(string imageFilePath, string waterMarkImagePath, string newImageMapPath, int watermarksize = 24, string watermarkposition = "C")
        {
            RocketUtils.ImgUtils.AddWatermark(imageFilePath, waterMarkImagePath, newImageMapPath, watermarksize, watermarkposition);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static Bitmap CreateCanvas(int width, int height)
        {
            return RocketUtils.ImgUtils.CreateCanvas(width, height);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static Bitmap CreateCanvas(int width, int height, string backGroundColor)
        {
            return RocketUtils.ImgUtils.CreateCanvas(width, height, backGroundColor);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void AddToCanvas(string imageFilePath, string canvasImagePath, int watermarksize = 24, string watermarkposition = "C")
        {
            RocketUtils.ImgUtils.AddToCanvas(imageFilePath, canvasImagePath, watermarksize, watermarkposition);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string ResizeImageToJpg(string fileNameIn, string fileNameOut, double imgSize)
        {
            return RocketUtils.ImgUtils.ResizeImageToJpg(fileNameIn, fileNameOut, imgSize);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string ResizeImageToPng(string fileNameIn, string fileNameOut, double imgSize)
        {
            return RocketUtils.ImgUtils.ResizeImageToPng(fileNameIn, fileNameOut, imgSize);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static Bitmap NewBitmap(this FileInfo fi)
        {
            return RocketUtils.ImgUtils.NewBitmap(fi);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string ResizeImage(string fileNamePath, string fileNamePathOut, double intMaxWidth)
        {
            return RocketUtils.ImgUtils.ResizeImage(fileNamePath, fileNamePathOut, intMaxWidth);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static int GetThumbWidth(string thumbSize)
        {
            return RocketUtils.ImgUtils.GetThumbWidth(thumbSize);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static int GetThumbHeight(string thumbSize)
        {
            return RocketUtils.ImgUtils.GetThumbHeight(thumbSize);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string GetThumbFileName(string imgPathName, int thumbW)
        {
            return RocketUtils.ImgUtils.GetThumbFileName(imgPathName, thumbW);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string GetThumbFileName(string imgPathName, int thumbW, int thumbH)
        {
            return RocketUtils.ImgUtils.GetThumbFileName(imgPathName, thumbW, thumbH);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string GetThumbFilePathName(string imgPathName, int thumbW)
        {
            return RocketUtils.ImgUtils.GetThumbFilePathName(imgPathName, thumbW);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string GetThumbFilePathName(string imgPathName, int thumbW, int thumbH)
        {
            return RocketUtils.ImgUtils.GetThumbFilePathName(imgPathName, thumbW, thumbH);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void CreateThumbOnDisk(string imgPathName, string thumbSizeCsv)
        {
            RocketUtils.ImgUtils.CreateThumbOnDisk(imgPathName, thumbSizeCsv);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void CreateThumbOnDisk(string imgPathName, string thumbSizeCsv, string outMapPath, bool webpConvert = false)
        {
            RocketUtils.ImgUtils.CreateThumbOnDisk(imgPathName, thumbSizeCsv, outMapPath, webpConvert);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void CreateThumbnailOnDisk(string imgPathName, int intMaxWidth, int intMaxHeight)
        {
            RocketUtils.ImgUtils.CreateThumbnailOnDisk(imgPathName, intMaxWidth, intMaxHeight);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string ConvertWebpToJpg(string imgPathName)
        {
            return RocketUtils.ImgUtils.ConvertWebpToJpg(imgPathName);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static bool ContainsTransparent(string MapPath)
        {
            return RocketUtils.ImgUtils.ContainsTransparent(MapPath);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string ConvertToWebp(string imgPathName, string outFileMapPath = "")
        {
            return RocketUtils.ImgUtils.ConvertToWebp(imgPathName, outFileMapPath);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void CreateThumbnailOnDisk(string imgPathName, int intMaxWidth, int intMaxHeight, string filePathOut)
        {
            RocketUtils.ImgUtils.CreateThumbnailOnDisk(imgPathName, intMaxWidth, intMaxHeight, filePathOut);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static Bitmap CreateThumbnail(string strFilepath, int intMaxWidth, int intMaxHeight)
        {
            return RocketUtils.ImgUtils.CreateThumbnail(strFilepath, intMaxWidth, intMaxHeight);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static Bitmap CreateThumbnail(string strFilepath, int intMaxWidth)
        {
            return RocketUtils.ImgUtils.CreateThumbnail(strFilepath, intMaxWidth);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static string ThumbMapPath(string sourceFileMapPath)
        {
            return RocketUtils.ImgUtils.ThumbMapPath(sourceFileMapPath);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static Bitmap CreateThumbnail(string strFilepath, int intMaxWidth, int intMaxHeight, string imgtype)
        {
            return RocketUtils.ImgUtils.CreateThumbnail(strFilepath, intMaxWidth, intMaxHeight, imgtype);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static Bitmap ProcessBitMap(Bitmap sourceImage, int intMaxWidth, int intMaxHeight)
        {
            return RocketUtils.ImgUtils.ProcessBitMap(sourceImage, intMaxWidth, intMaxHeight);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static List<string> UploadBase64Image(string[] filenameList, string[] filebase64List, string tempFileMapPath, string imageFolderMapPath, int size)
        {
            return RocketUtils.ImgUtils.UploadBase64Image(filenameList, filebase64List, tempFileMapPath, imageFolderMapPath, size);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void CreateQRcode(string data, string fileMapPath)
        {
            RocketUtils.ImgUtils.CreateQRcode(data, fileMapPath);
        }
        [Obsolete("Use RocketUtils.ImgUtils instead")]
        public static void DownloadAndSaveImage(string imageUrl, string filename)
        {
            RocketUtils.ImgUtils.DownloadAndSaveImage(imageUrl, filename);
        }
    }
}
