using System.IO;

namespace FangPage.Common
{
	internal interface Contract
	{
		bool IsValid();

		string GetFileName();

		string GetMimeType();

		long GetFileLength();

		void Write(Stream output);
	}
}
