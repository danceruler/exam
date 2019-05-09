using System.IO;

namespace FangPage.Common
{
	internal class LocalContract : Contract
	{
		private FileInfo fileInfo;

		public LocalContract(FileInfo fileInfo)
		{
			this.fileInfo = fileInfo;
		}

		public long GetFileLength()
		{
			return fileInfo.Length;
		}

		public string GetFileName()
		{
			return fileInfo.Name;
		}

		public string GetMimeType()
		{
			return "application/octet-stream";
		}

		public bool IsValid()
		{
			return fileInfo != null && fileInfo.Exists;
		}

		public void Write(Stream output)
		{
			using (BufferedStream bufferedStream = new BufferedStream(fileInfo.OpenRead()))
			{
				int num = 0;
				byte[] array = new byte[4096];
				while ((num = bufferedStream.Read(array, 0, array.Length)) > 0)
				{
					output.Write(array, 0, num);
				}
			}
		}
	}
}
