using System;
using System.Text;

using AddressProcessing.Exceptions;

using DataProviders;
using DataProviders.CSV;

using Helpers.Common;

namespace AddressProcessing.CSV
{
	/// <summary>	
	/// CSVReaderWriter will close any passed or opened streams while disposing.
	/// </summary>
	public class CSVReaderWriter : ICSVReaderWriter
	{
		const char DELIMETER_AS_TAB = '\t';		
		

		private IReader ReaderStream { get; set; }
		private IWriter WriterStream { get; set; }

		[Flags]
		public enum Mode { Read = 1, Write = 2 };

		[Obsolete("This constructor is obsolete. Use constructors with parameters in using()")]
		public CSVReaderWriter()
		{			
		}

		public CSVReaderWriter(string fileName, Mode mode, char delimiter = DELIMETER_AS_TAB, bool append = true, Encoding encoding = null)
		{			
			fileName.ThrowIfNullOrEmpty("fileName");

			Open_Internal(fileName, mode, delimiter, append, encoding);
		}

		public CSVReaderWriter(IReader readerStream)
		{
			WriterStream = null;
			readerStream.ThrowIfNull("readerStream");

			ReaderStream = readerStream;
		}

		public CSVReaderWriter(IWriter writerStream)
		{
			ReaderStream = null;
			writerStream.ThrowIfNull("writerStream");

			WriterStream = writerStream;
		}
		
		[Obsolete("This method is obsolete. Use constructors with parameters in using() instead")]
		public void Open(string fileName, Mode mode)
		{
			Open_Internal(fileName, mode, DELIMETER_AS_TAB, false, Encoding.UTF8);
		}

		protected void Open_Internal(string fileName, Mode mode, char delimiter, bool append, Encoding encoding)
		{
			fileName.ThrowIfNullOrEmpty("fileName");
			
			switch (mode)
			{
				case Mode.Read:
				{
					if (ReaderStream != null) 
						ReaderStream.Close();

					ReaderStream = new CsvReader(fileName, delimiter, encoding);
					break;
				}
				case Mode.Write:
				{
					if (WriterStream != null)
						WriterStream.Close();

					WriterStream = new CsvWriter(fileName, delimiter, append, encoding);
					break;
				}
				default:
					throw new Exception(String.Format("Unknown mode {0}", mode));
			}
		}

		public void Write(params string[] columns)
		{
			columns.ThrowIfNull("columns");

			if (WriterStream == null)
				throw new NullReferenceException("_writerStream");
			
			WriterStream.Write(columns);
		}

		[Obsolete("This method is obsolete. Use 'bool Read(out string[] columns)' instead")]
		public bool Read(string column1, string column2)
		{
			if (ReaderStream == null)
				throw new NullReferenceException("_readerStream");
			
			var columns = ReaderStream.Read();

			var isReadFailed = columns == null || columns.Length == 0;
			return !isReadFailed;
		}

		[Obsolete("This method is obsolete. Use 'bool Read(out string[] columns)' instead")]
		public bool Read(out string column1, out string column2)
		{
			string[] columns;
			var isReadSuccess = this.Read(out columns);
						
			if (!isReadSuccess)  
			{
				column1 = null;
				column2 = null;
			}			
			else
			{
				const int FIRST_COLUMN = 0;
				const int SECOND_COLUMN = 1;

				column1 = columns[FIRST_COLUMN];
				column2 = columns[SECOND_COLUMN];
			}

			return isReadSuccess;
		}

		public bool Read(out string[] columns)
		{
			if(ReaderStream == null)
				throw new NullReferenceException("_readerStream");

			bool isReadSuccess;

			columns = ReaderStream.Read();

			var isEOF = columns == null;
			var isEmptyLine = columns == null || columns.Length == 0;

			const int MIN_COLUMN_COUNT = 2;

			if (isEOF || isEmptyLine)						
				isReadSuccess = false;			
			else if (columns.Length < MIN_COLUMN_COUNT)
			{
				throw new WrongFieldsNumberException(ReaderStream.ProcessedLines, MIN_COLUMN_COUNT, (uint)columns.Length);
			}
			else			
				isReadSuccess = true;
			
			return isReadSuccess;
		}
		
		public void Close()
		{
			if (WriterStream != null)
			{
				WriterStream.Close();
				WriterStream = null;
			}

			if (ReaderStream != null)
			{
				ReaderStream.Close();
				ReaderStream = null;
			}			
		}

		public void Dispose()
		{
			Close();
		}
	}
}
