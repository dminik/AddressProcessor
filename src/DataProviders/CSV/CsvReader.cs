using System;
using System.IO;
using System.Text;

using Helpers.Common;

namespace DataProviders.CSV
{
	public class CsvReader : IReader
	{
		protected StreamReader Reader { get; set; }

		public bool IsDisposed { get; private set; }

		public char Delimiter { get; private set; }

		public uint ProcessedLines { get; private set; }

		public CsvReader(string fileName, char delimiter = '\t', Encoding encoding = null)
		{
			fileName.ThrowIfNull("fileName");

			Delimiter = delimiter;

			if (encoding == null)
				encoding = Encoding.UTF8;

			Reader = new StreamReader(fileName, encoding);
		}

		public CsvReader(StreamReader stream, char delimiter = '\t')
		{
			stream.ThrowIfNull("stream");

			Delimiter = delimiter;
			Reader = stream;
		}

		public virtual string[] Read()
		{
			if (IsDisposed)
				throw new ObjectDisposedException(typeof(CsvReader).FullName);

			var values = new string[0];

			if (Reader.Peek() > -1)
			{
				var line = Reader.ReadLine();

				if (line != null)
					values = line.Split(Delimiter);

				ProcessedLines++;
				return values;
			}

			return null;
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
					if (Reader != null)
					{
						Reader.Close();
						Reader = null;
					}
				}

				IsDisposed = true;
			}
		}
	}
}
