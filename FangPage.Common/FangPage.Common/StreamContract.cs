using System.IO;

namespace FangPage.Common
{
	internal class StreamContract : Contract
	{
		private string fileName;

		private Stream stream;

		private string mimeType;

		public StreamContract(string fileName, Stream stream, string mimeType)
		{
			this.fileName = fileName;
			this.stream = stream;
			this.mimeType = mimeType;
		}

		public long GetFileLength()
		{
			return 0L;
		}

		public string GetFileName()
		{
			return fileName;
		}

		public string GetMimeType()
		{
			if (string.IsNullOrEmpty(mimeType))
			{
				return "application/octet-stream";
			}
			return mimeType;
		}

		public bool IsValid()
		{
			return stream != null && fileName != null;
		}

		public void Write(Stream output)
		{
			using (this.stream)
			{
				int num = 0;
				byte[] array = new byte[4096];
				while ((num = this.stream.Read(array, 0, array.Length)) > 0)
				{
					output.Write(array, 0, num);
				}
			}
		}
	}
}
