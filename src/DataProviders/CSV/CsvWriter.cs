using System;
using System.IO;
using System.Text;

using Helpers.Common;

namespace DataProviders.CSV
{
	public class CsvWriter : IWriter
	{
		private StreamWriter _writer;

		private bool _disposed;
		private readonly char _delimiter;

		public CsvWriter(string fileName, char delimiter = '\t', bool append = true, Encoding encoding = null)
		{
			fileName.ThrowIfNull("fileName");

			_delimiter = delimiter;

			if (encoding == null)
				encoding = Encoding.UTF8;

			_writer = new StreamWriter(fileName, append, encoding);
		}

		public CsvWriter(StreamWriter stream, char delimiter = '\t')
		{
			stream.ThrowIfNull("stream");
			_delimiter = delimiter;
			_writer = stream;
		}

		public void Write(string[] data)
		{
			data.ThrowIfNull("data");

			if (_disposed)
				throw new ObjectDisposedException(typeof(CsvWriter).FullName);

			var line = string.Join(_delimiter.ToString(), data);
			_writer.WriteLine(line);
		}

		public void Close()
		{
			Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (_writer != null)
					{
						_writer.Close();
						_writer = null;
					}
				}

				_disposed = true;
			}
		}
	}
}
