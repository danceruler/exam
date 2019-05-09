using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using ZXing;
using ZXing.Common;
using ZXing.QrCode;

namespace FangPage.Common
{
	public class FPCode
	{
		public static Image CreateBarCode(string source, int width, int height)
		{
			return CreateBarCode(source, "CODE128", width, height);
		}

		public static Image CreateBarCode(string source, string type, int width, int height)
		{
			if (width == 0 && !string.IsNullOrEmpty(source))
			{
				width = source.Length;
			}
			if (width == 0)
			{
				width = 100;
			}
			if (height == 0)
			{
				height = 80;
			}
			if (string.IsNullOrEmpty(source))
			{
				return new Bitmap(width, height);
			}
			BarcodeFormat format = BarcodeFormat.CODE_128;
			if (type.ToUpper() == "EAN13")
			{
				format = BarcodeFormat.EAN_13;
			}
			else if (type.ToLower() == "EAN8")
			{
				format = BarcodeFormat.EAN_8;
			}
			EncodingOptions encodingOptions = new EncodingOptions();
			encodingOptions.Height = height;
			encodingOptions.Width = width;
			BarcodeWriter barcodeWriter = new BarcodeWriter();
			barcodeWriter.Options = encodingOptions;
			barcodeWriter.Format = format;
			return barcodeWriter.Write(source);
		}

		public static Image CreateQRCode(string source, int width, int height, int margin)
		{
			if (width == 0 && height > 0)
			{
				width = height;
			}
			else if (width > 0 && height == 0)
			{
				height = width;
			}
			else
			{
				width = 200;
				height = 200;
			}
			if (string.IsNullOrEmpty(source))
			{
				return new Bitmap(width, height);
			}
			QrCodeEncodingOptions qrCodeEncodingOptions = new QrCodeEncodingOptions();
			qrCodeEncodingOptions.CharacterSet = "UTF-8";
			qrCodeEncodingOptions.Height = height;
			qrCodeEncodingOptions.Width = width;
			qrCodeEncodingOptions.Margin = margin;
			BarcodeWriter barcodeWriter = new BarcodeWriter();
			barcodeWriter.Format = BarcodeFormat.QR_CODE;
			barcodeWriter.Options = qrCodeEncodingOptions;
			return barcodeWriter.Write(source);
		}

		public static Image CreateQRCode(string source, string logo, int width, int height, int margin, int logoop)
		{
			if (width == 0 && height > 0)
			{
				width = height;
			}
			else if (width > 0 && height == 0)
			{
				height = width;
			}
			else
			{
				width = 200;
				height = 200;
			}
			if (logoop == 0)
			{
				logoop = 55;
			}
			if (margin < 0)
			{
				margin = 1;
			}
			if (string.IsNullOrEmpty(source))
			{
				return new Bitmap(width, height);
			}
			QrCodeEncodingOptions qrCodeEncodingOptions = new QrCodeEncodingOptions();
			qrCodeEncodingOptions.CharacterSet = "UTF-8";
			qrCodeEncodingOptions.Height = height;
			qrCodeEncodingOptions.Width = width;
			qrCodeEncodingOptions.Margin = margin;
			BarcodeWriter barcodeWriter = new BarcodeWriter();
			barcodeWriter.Format = BarcodeFormat.QR_CODE;
			barcodeWriter.Options = qrCodeEncodingOptions;
			Bitmap bitmap = barcodeWriter.Write(source);
			if (File.Exists(logo))
			{
				Bitmap bitmap2 = Image.FromFile(logo) as Bitmap;
				int num = 2;
				float horizontalResolution = bitmap.HorizontalResolution;
				float verticalResolution = bitmap.VerticalResolution;
				float num2 = (float)(10 * bitmap.Width - 46 * num) * 1f / (float)logoop;
				Image image = ZoomPic(bitmap2, num2 / (float)bitmap2.Width);
				int num3 = image.Width + num;
				Bitmap bitmap3 = new Bitmap(num3, num3);
				bitmap3.MakeTransparent();
				Graphics graphics = Graphics.FromImage(bitmap3);
				graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics.SmoothingMode = SmoothingMode.HighQuality;
				graphics.Clear(Color.Transparent);
				Pen pen = new Pen(new SolidBrush(Color.White));
				Rectangle rect = new Rectangle(0, 0, num3 - 1, num3 - 1);
				using (GraphicsPath path = CreateRoundedRectanglePath(rect, 7))
				{
					graphics.DrawPath(pen, path);
					graphics.FillPath(new SolidBrush(Color.White), path);
				}
				Bitmap image2 = new Bitmap(image.Width, image.Width);
				Graphics graphics2 = Graphics.FromImage(image2);
				graphics2.InterpolationMode = InterpolationMode.HighQualityBicubic;
				graphics2.SmoothingMode = SmoothingMode.HighQuality;
				graphics2.Clear(Color.Transparent);
				Pen pen2 = new Pen(new SolidBrush(Color.Gray));
				Rectangle rect2 = new Rectangle(0, 0, image.Width - 1, image.Width - 1);
				using (GraphicsPath path2 = CreateRoundedRectanglePath(rect2, 7))
				{
					graphics2.DrawPath(pen2, path2);
					TextureBrush brush = new TextureBrush(image);
					graphics2.FillPath(brush, path2);
				}
				graphics2.Dispose();
				PointF pointF = new PointF((num3 - image.Width) / 2, (num3 - image.Height) / 2);
				graphics.DrawImage(image2, pointF.X, pointF.Y, image.Width, image.Height);
				graphics.Dispose();
				Bitmap bitmap4 = new Bitmap(bitmap.Width, bitmap.Height);
				bitmap4.MakeTransparent();
				bitmap4.SetResolution(horizontalResolution, verticalResolution);
				bitmap3.SetResolution(horizontalResolution, verticalResolution);
				Graphics graphics3 = Graphics.FromImage(bitmap4);
				graphics3.Clear(Color.Transparent);
				graphics3.DrawImage(bitmap, 0, 0);
				PointF point = new PointF((bitmap.Width - bitmap3.Width) / 2, (bitmap.Height - bitmap3.Height) / 2);
				graphics3.DrawImage(bitmap3, point);
				graphics3.Dispose();
				return bitmap4;
			}
			return bitmap;
		}

		public static string ReadBarCode(Image img)
		{
			DecodingOptions decodingOptions = new DecodingOptions();
			decodingOptions.PossibleFormats = new List<BarcodeFormat>
			{
				BarcodeFormat.EAN_13
			};
			BarcodeReader barcodeReader = new BarcodeReader();
			barcodeReader.Options = decodingOptions;
			Result result = barcodeReader.Decode(img as Bitmap);
			if (result == null)
			{
				return "";
			}
			return result.Text;
		}

		public static string ReadBarCode(Bitmap img)
		{
			DecodingOptions decodingOptions = new DecodingOptions();
			decodingOptions.PossibleFormats = new List<BarcodeFormat>
			{
				BarcodeFormat.EAN_13
			};
			BarcodeReader barcodeReader = new BarcodeReader();
			barcodeReader.Options = decodingOptions;
			Result result = barcodeReader.Decode(img);
			if (result == null)
			{
				return "";
			}
			return result.Text;
		}

		public static string ReadBarCode(string imgpath)
		{
			if (!File.Exists(imgpath))
			{
				return "";
			}
			Bitmap barcodeBitmap = new Bitmap(imgpath);
			DecodingOptions decodingOptions = new DecodingOptions();
			decodingOptions.PossibleFormats = new List<BarcodeFormat>
			{
				BarcodeFormat.EAN_13
			};
			BarcodeReader barcodeReader = new BarcodeReader();
			barcodeReader.Options = decodingOptions;
			Result result = barcodeReader.Decode(barcodeBitmap);
			if (result == null)
			{
				return "";
			}
			return result.Text;
		}

		public static string ReadQRCode(Image img)
		{
			DecodingOptions decodingOptions = new DecodingOptions();
			decodingOptions.PossibleFormats = new List<BarcodeFormat>
			{
				BarcodeFormat.QR_CODE
			};
			BarcodeReader barcodeReader = new BarcodeReader();
			barcodeReader.Options = decodingOptions;
			Result result = barcodeReader.Decode(img as Bitmap);
			if (result == null)
			{
				return "";
			}
			return result.Text;
		}

		public static string ReadQRCode(Bitmap img)
		{
			DecodingOptions decodingOptions = new DecodingOptions();
			decodingOptions.PossibleFormats = new List<BarcodeFormat>
			{
				BarcodeFormat.QR_CODE
			};
			BarcodeReader barcodeReader = new BarcodeReader();
			barcodeReader.Options = decodingOptions;
			Result result = barcodeReader.Decode(img);
			if (result == null)
			{
				return "";
			}
			return result.Text;
		}

		public static string ReadQRCode(string imgpath)
		{
			if (!File.Exists(imgpath))
			{
				return "";
			}
			Bitmap barcodeBitmap = new Bitmap(imgpath);
			DecodingOptions decodingOptions = new DecodingOptions();
			decodingOptions.PossibleFormats = new List<BarcodeFormat>
			{
				BarcodeFormat.QR_CODE
			};
			BarcodeReader barcodeReader = new BarcodeReader();
			barcodeReader.Options = decodingOptions;
			Result result = barcodeReader.Decode(barcodeBitmap);
			if (result == null)
			{
				return "";
			}
			return result.Text;
		}

		private static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
		{
			GraphicsPath graphicsPath = new GraphicsPath();
			graphicsPath.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180f, 90f);
			graphicsPath.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
			graphicsPath.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270f, 90f);
			graphicsPath.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
			graphicsPath.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0f, 90f);
			graphicsPath.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
			graphicsPath.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90f, 90f);
			graphicsPath.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
			graphicsPath.CloseFigure();
			return graphicsPath;
		}

		private static Image ZoomPic(Image initImage, double n)
		{
			double num = initImage.Width;
			double num2 = initImage.Height;
			num = n * (double)initImage.Width;
			num2 = n * (double)initImage.Height;
			Image image = new Bitmap((int)num, (int)num2);
			Graphics graphics = Graphics.FromImage(image);
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.Clear(Color.Transparent);
			graphics.DrawImage(initImage, new Rectangle(0, 0, image.Width, image.Height), new Rectangle(0, 0, initImage.Width, initImage.Height), GraphicsUnit.Pixel);
			graphics.Dispose();
			return image;
		}
	}
}
