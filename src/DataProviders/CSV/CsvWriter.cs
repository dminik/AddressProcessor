using System;
using System.IO;
using System.Text;

using Helpers.Common;

namespace DataProviders.CSV
{
	public class CsvWriter : IWriter
	{
		protected StreamWriter Writer { get; set; }

		public bool IsDisposed { get; private set; }

		public char Delimiter { get; private set; }

		public CsvWriter(string fileName, char delimiter = '\t', bool append = true, Encoding encoding = null)
		{
			fileName.ThrowIfNull("fileName");

			Delimiter = delimiter;

			if (encoding == null)
				encoding = Encoding.ASCII;

			Writer = new StreamWriter(fileName, append, encoding);
		}

		public CsvWriter(StreamWriter stream, char delimiter = '\t')
		{
			stream.ThrowIfNull("stream");
			Delimiter = delimiter;
			Writer = stream;
		}

		public virtual void Write(string[] data)
		{
			data.ThrowIfNull("data");

			if (IsDisposed)
				throw new ObjectDisposedException(typeof(CsvWriter).FullName);

			var line = string.Join(Delimiter.ToString(), data);
			Writer.WriteLine(line);
		}

		public virtual void Close()
		{
			Dispose();
		}

		public virtual void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!IsDisposed)
			{
				if (disposing)
				{
					if (Writer != null)
					{
						Writer.Close();
						Writer = null;
					}
				}

				IsDisposed = true;
			}
		}
	}
}
