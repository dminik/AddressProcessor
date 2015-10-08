using System;

namespace AddressProcessing.CSV
{
	public interface ICSVReaderWriter : IDisposable
	{
		void Open(string fileName, CSVReaderWriter.Mode mode);

		void Write(params string[] columns);

		bool Read(string column1, string column2);

		bool Read(out string column1, out string column2);

		bool Read(out string[] columns);

		void Close();
	}
}