using FangPage.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace FangPage.MVC
{
	public class FPThumb
	{
		private Image srcImage;

		private string srcFileName;

		public bool SetImage(string FileName)
		{
			srcFileName = FPFile.GetMapPath(FileName);
			try
			{
				srcImage = Image.FromFile(srcFileName);
			}
			catch
			{
				return false;
			}
			return true;
		}

		public bool ThumbnailCallback()
		{
			return false;
		}

		public static string GetThumbnail(string imgpath, int maxsize)
		{
			int num = maxsize;
			if (num <= 0)
			{
				num = 600;
			}
			string text = FPFile.GetMapPath(WebConfig.WebPath) + imgpath;
			if (!File.Exists(text))
			{
				return "";
			}
			string fileName = Path.GetFileName(imgpath);
			string text2 = Path.GetExtension(imgpath).ToLower();
			switch (text2)
			{
			case ".jpg":
			case ".bmp":
			case ".png":
			{
				fileName = Path.GetFileNameWithoutExtension(imgpath);
				string text3 = $"{WebConfig.WebPath}cache/thumbnail/{fileName}_{num}{text2}";
				string mapPath = FPFile.GetMapPath(text3);
				if (!File.Exists(mapPath))
				{
					string mapPath2 = FPFile.GetMapPath(WebConfig.WebPath + "cache/thumbnail/");
					if (!Directory.Exists(mapPath2))
					{
						try
						{
							Directory.CreateDirectory(mapPath2);
						}
						catch
						{
							throw new Exception("请检查程序目录下cache文件夹的用户权限！");
						}
					}
					CreateThumbnail(mapPath, text, num);
				}
				return text3;
			}
			case ".gif":
				return imgpath;
			default:
				return "";
			}
		}

		public static void CreateThumbnail(string attPhyCachePath, string attPhyPath, int theMaxsize)
		{
			if (File.Exists(attPhyPath))
			{
				try
				{
					MakeThumbnailImage(attPhyPath, attPhyCachePath, theMaxsize, theMaxsize);
				}
				catch
				{
				}
			}
		}

		public Image GetImage(int Width, int Height)
		{
			Image.GetThumbnailImageAbort callback = ThumbnailCallback;
			return srcImage.GetThumbnailImage(Width, Height, callback, IntPtr.Zero);
		}

		public void SaveThumbnailImage(int Width, int Height)
		{
			string a = Path.GetExtension(srcFileName).ToLower();
			if (!(a == ".png"))
			{
				if (a == ".gif")
				{
					SaveImage(Width, Height, ImageFormat.Gif);
				}
				else
				{
					SaveImage(Width, Height, ImageFormat.Jpeg);
				}
			}
			else
			{
				SaveImage(Width, Height, ImageFormat.Png);
			}
		}

		public void SaveImage(int Width, int Height, ImageFormat imgformat)
		{
			if ((imgformat != ImageFormat.Gif && srcImage.Width > Width) || srcImage.Height > Height)
			{
				Image.GetThumbnailImageAbort callback = ThumbnailCallback;
				Image thumbnailImage = srcImage.GetThumbnailImage(Width, Height, callback, IntPtr.Zero);
				srcImage.Dispose();
				thumbnailImage.Save(srcFileName, imgformat);
				thumbnailImage.Dispose();
			}
		}

		private static void SaveImage(Image image, string savePath, ImageCodecInfo ici)
		{
			EncoderParameters encoderParameters = new EncoderParameters(1);
			encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 100L);
			image.Save(savePath, ici, encoderParameters);
			encoderParameters.Dispose();
		}

		private static ImageCodecInfo GetCodecInfo(string mimeType)
		{
			ImageCodecInfo[] imageEncoders = ImageCodecInfo.GetImageEncoders();
			foreach (ImageCodecInfo imageCodecInfo in imageEncoders)
			{
				if (imageCodecInfo.MimeType == mimeType)
				{
					return imageCodecInfo;
				}
			}
			return null;
		}

		private static Size ResizeImage(int width, int height, int maxWidth, int maxHeight)
		{
			if (maxWidth <= 0)
			{
				maxWidth = width;
			}
			if (maxHeight <= 0)
			{
				maxHeight = height;
			}
			decimal num = maxWidth;
			decimal d = maxHeight;
			decimal d2 = num / d;
			decimal d3 = width;
			decimal num2 = height;
			int width2;
			int height2;
			if (d3 > num || num2 > d)
			{
				if (d3 / num2 > d2)
				{
					decimal d4 = d3 / num;
					width2 = Convert.ToInt32(d3 / d4);
					height2 = Convert.ToInt32(num2 / d4);
				}
				else
				{
					decimal d4 = num2 / d;
					width2 = Convert.ToInt32(d3 / d4);
					height2 = Convert.ToInt32(num2 / d4);
				}
			}
			else
			{
				width2 = width;
				height2 = height;
			}
			return new Size(width2, height2);
		}

		public static ImageFormat GetFormat(string name)
		{
			switch (name.Substring(name.LastIndexOf(".") + 1).ToLower())
			{
			case "jpg":
			case "jpeg":
				return ImageFormat.Jpeg;
			case "bmp":
				return ImageFormat.Bmp;
			case "png":
				return ImageFormat.Png;
			case "gif":
				return ImageFormat.Gif;
			default:
				return ImageFormat.Jpeg;
			}
		}

		public static void MakeSquareImage(Image image, string newFileName, int newSize)
		{
			int width = image.Width;
			int height = image.Height;
			Bitmap bitmap = new Bitmap(newSize, newSize);
			try
			{
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.Clear(Color.Transparent);
				if (width < height)
				{
					graphics.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle(0, (height - width) / 2, width, width), GraphicsUnit.Pixel);
				}
				else
				{
					graphics.DrawImage(image, new Rectangle(0, 0, newSize, newSize), new Rectangle((width - height) / 2, 0, height, height), GraphicsUnit.Pixel);
				}
				SaveImage(bitmap, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
			}
			finally
			{
				image.Dispose();
				bitmap.Dispose();
			}
		}

		public static void MakeSquareImage(string fileName, string newFileName, int newSize)
		{
			MakeSquareImage(Image.FromFile(fileName), newFileName, newSize);
		}

		public static void MakeRemoteSquareImage(string url, string newFileName, int newSize)
		{
			Stream remoteImage = GetRemoteImage(url);
			if (remoteImage != null)
			{
				Image image = Image.FromStream(remoteImage);
				remoteImage.Close();
				MakeSquareImage(image, newFileName, newSize);
			}
		}

		public static void MakeThumbnailImage(Image original, string newFileName, int maxWidth, int maxHeight)
		{
			Size newSize = ResizeImage(original.Width, original.Height, maxWidth, maxHeight);
			using (Image image = new Bitmap(original, newSize))
			{
				try
				{
					image.Save(newFileName, original.RawFormat);
				}
				finally
				{
					original.Dispose();
				}
			}
		}

		public static void MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight)
		{
			MakeThumbnailImage(Image.FromStream(new MemoryStream(File.ReadAllBytes(fileName))), newFileName, maxWidth, maxHeight);
		}

		public static void MakeThumbnailImage(string fileName, string newFileName, int width, int height, string mode)
		{
			Image image = Image.FromFile(fileName);
			int num = width;
			int num2 = height;
			int x = 0;
			int y = 0;
			int num3 = image.Width;
			int num4 = image.Height;
			switch (mode)
			{
			case "W":
				num2 = image.Height * width / image.Width;
				break;
			case "H":
				num = image.Width * height / image.Height;
				break;
			case "Cut":
				if ((double)image.Width / (double)image.Height > (double)num / (double)num2)
				{
					num4 = image.Height;
					num3 = image.Height * num / num2;
					y = 0;
					x = (image.Width - num3) / 2;
				}
				else
				{
					num3 = image.Width;
					num4 = image.Width * height / num;
					x = 0;
					y = (image.Height - num4) / 2;
				}
				break;
			}
			Bitmap bitmap = new Bitmap(num, num2);
			try
			{
				Graphics graphics = Graphics.FromImage(bitmap);
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.AntiAlias;
				graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
				graphics.Clear(Color.Transparent);
				graphics.DrawImage(image, new Rectangle(0, 0, num, num2), new Rectangle(x, y, num3, num4), GraphicsUnit.Pixel);
				SaveImage(bitmap, newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				image.Dispose();
				bitmap.Dispose();
			}
		}

		public static bool MakeThumbnailImage(string fileName, string newFileName, int maxWidth, int maxHeight, int cropWidth, int cropHeight, int X, int Y)
		{
			Image image = Image.FromStream(new MemoryStream(File.ReadAllBytes(fileName)));
			Bitmap bitmap = new Bitmap(cropWidth, cropHeight);
			try
			{
				using (Graphics graphics = Graphics.FromImage(bitmap))
				{
					graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
					graphics.SmoothingMode = SmoothingMode.AntiAlias;
					graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
					graphics.Clear(Color.Transparent);
					graphics.DrawImage(image, new Rectangle(0, 0, cropWidth, cropHeight), X, Y, cropWidth, cropHeight, GraphicsUnit.Pixel);
					SaveImage(new Bitmap(bitmap, maxWidth, maxHeight), newFileName, GetCodecInfo("image/" + GetFormat(newFileName).ToString().ToLower()));
					return true;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
			finally
			{
				image.Dispose();
				bitmap.Dispose();
			}
		}

		public static void MakeRemoteThumbnailImage(string url, string newFileName, int maxWidth, int maxHeight)
		{
			Stream remoteImage = GetRemoteImage(url);
			if (remoteImage != null)
			{
				Image original = Image.FromStream(remoteImage);
				remoteImage.Close();
				MakeThumbnailImage(original, newFileName, maxWidth, maxHeight);
			}
		}

		private static Stream GetRemoteImage(string url)
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			httpWebRequest.ContentLength = 0L;
			httpWebRequest.Timeout = 20000;
			try
			{
				return ((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream();
			}
			catch
			{
				return null;
			}
		}
	}
}
