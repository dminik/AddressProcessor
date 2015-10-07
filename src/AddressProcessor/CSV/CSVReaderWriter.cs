using System;
using System.IO;

using AddressProcessing.Exceptions;

using DataProviders;
using DataProviders.CSV;

using Helpers.Common;

namespace AddressProcessing.CSV
{
	/// <summary>
	/// Предположим, что класс используется в продакшине как ридер и врайтер одновременно. Может иметь 2 открытых потока.
	/// Может открывать новые потоки, закрывая старые. 
	/// CSVReaderWriter will close any passed or created streams while disposing.
	/// </summary>
	public class CSVReaderWriter : IDisposable
	{
		IReader _readerStream = null;
		IWriter _writerStream = null;

		readonly char DELIMETER_AS_TAB = '\t';
		const int FIRST_COLUMN = 0;
		const int SECOND_COLUMN = 1;
		const int MIN_COLUMN_COUNT = 2;
		
		[Flags]
		public enum Mode { Read = 1, Write = 2 };

		[Obsolete("This constructor is obsolete.  with parameters in 'using()'")]
		public CSVReaderWriter()
		{			
		}

		public CSVReaderWriter(string fileName, Mode mode)
		{
#pragma warning disable 618
			Open(fileName, mode);
#pragma warning restore 618
		}

		public CSVReaderWriter(IReader readerStream)
		{
			_readerStream = readerStream;
		}

		public CSVReaderWriter(IWriter writerStream)
		{
			_writerStream = writerStream;
		}
		
		[Obsolete("This method is obsolete. Use constructors with parameters in 'using()' instead")]
		public void Open(string fileName, Mode mode)
		{			
			fileName.ThrowIfNull("fileName");
			
			switch (mode)
			{
				case Mode.Read:
				{
					if (_readerStream != null) 
						_readerStream.Close();

					_readerStream = new CsvReader(new StreamReader(fileName), DELIMETER_AS_TAB);
					break;
				}
				case Mode.Write:
				{
					if (_writerStream != null)
						_writerStream.Close();

					_writerStream = new CsvWriter(new StreamWriter(fileName), DELIMETER_AS_TAB);
					break;
				}
				default:
					throw new Exception(String.Format("Unknown mode {0}", mode));
			}
		}

		public void Write(params string[] columns)
		{
			columns.ThrowIfNull("columns");

			if(_writerStream == null)
				throw new NullReferenceException("_writerStream == null");

			_writerStream.Write(columns);
		}

		[Obsolete("This method is obsolete. Use 'bool Read(out string[] columns)' instead")]
		public bool Read(string reservedParam1 = null, string reservedParam2 = null)
		{			
			var columns = _readerStream.Read();			
			return columns.Length == 0;
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
				column1 = columns[FIRST_COLUMN];
				column2 = columns[SECOND_COLUMN];
			}

			return isReadSuccess;
		}

		public bool Read(out string[] columns)
		{			
			bool isReadSuccess;

			columns = _readerStream.Read();

			var isEOF = columns == null;
			var isEmptyLine = columns == null || columns.Length == 0;

			if (isEOF || isEmptyLine)						
				isReadSuccess = false;			
			else if (columns.Length < MIN_COLUMN_COUNT)
			{
				throw new WrongFieldsNumberException(_readerStream.ProcessedLines, MIN_COLUMN_COUNT, (uint)columns.Length);
			}
			else			
				isReadSuccess = true;
			
			return isReadSuccess;
		}
		
		public void Close()
		{
			if (_writerStream != null)			
				_writerStream.Close();			

			if (_readerStream != null)			
				_readerStream.Close();			
		}

		public void Dispose()
		{
			Close();
		}
	}
}
