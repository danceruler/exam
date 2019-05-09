using System.IO;

namespace FangPage.Common
{
	public class FPFileItem
	{
		private Contract contract;

		public FPFileItem(FileInfo fileInfo)
		{
			contract = new LocalContract(fileInfo);
		}

		public FPFileItem(string filePath)
			: this(new FileInfo(filePath))
		{
		}

		public FPFileItem(string fileName, byte[] content)
			: this(fileName, content, null)
		{
		}

		public FPFileItem(string fileName, byte[] content, string mimeType)
		{
			contract = new ByteArrayContract(fileName, content, mimeType);
		}

		public FPFileItem(string fileName, Stream stream)
			: this(fileName, stream, null)
		{
		}

		public FPFileItem(string fileName, Stream stream, string mimeType)
		{
			contract = new StreamContract(fileName, stream, mimeType);
		}

		public bool IsValid()
		{
			return contract.IsValid();
		}

		public long GetFileLength()
		{
			return contract.GetFileLength();
		}

		public string GetFileName()
		{
			return contract.GetFileName();
		}

		public string GetMimeType()
		{
			return contract.GetMimeType();
		}

		public void Write(Stream output)
		{
			contract.Write(output);
		}
	}
}
