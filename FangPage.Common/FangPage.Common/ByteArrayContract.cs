using System.IO;

namespace FangPage.Common
{
	internal class ByteArrayContract : Contract
	{
		private string fileName;

		private byte[] content;

		private string mimeType;

		public ByteArrayContract(string fileName, byte[] content, string mimeType)
		{
			this.fileName = fileName;
			this.content = content;
			this.mimeType = mimeType;
		}

		public bool IsValid()
		{
			return content != null && fileName != null;
		}

		public long GetFileLength()
		{
			return content.Length;
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

		public void Write(Stream output)
		{
			output.Write(content, 0, content.Length);
		}
	}
}
