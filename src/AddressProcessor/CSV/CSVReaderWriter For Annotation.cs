using System;
using System.IO;

namespace AddressProcessing.CSV
{
	// TODO: 
	// Need interface. It will helpful for dependency injection and mocking in unit tests.	
	// Implement interface IDisposable
	// Better to use two different  classes to write and to read. It is a ‘Single responsibility principle’.
	// We have the "interface" of ReaderWriter already in a production and can not to rollback it. 
	// Let's try to use it like fasade for your new classes CSVReader and CSVWriter. You should implement them and its interfaces (IReader and IWriter).
    public class CSVReaderWriterForAnnotation
    {
		// TODO: Use proprty instead field. Property more flexible to change access and logic. 
        private StreamReader _readerStream = null;
        private StreamWriter _writerStream = null;

		// TODO: It is better to have each type declaration in separate file. May be you will reuse it with other type of ReaderWriter.
        [Flags]
        public enum Mode { Read = 1, Write = 2 };

		// TODO: Define new constructors 
		// CSVReaderWriter(IReader readerStream)
		// CSVReaderWriter(IWriter writerStream) 
		// CSVReaderWriter(string fileName, Mode mode)
		// It could be useful for consumers and unit tests

		// TODO: Constructor CSVReaderWriter() mark as obsolete.

		
		// TODO: It is better to usee Disposable pattern for Open and Close functionality in your classes. 
		// Implement Disposable interface and mark this method as obsolete.
        public void Open(string fileName, Mode mode)
        {
			// TODO: always check input params for public methods

			// TODO: "switch" is more tidy here 
            if (mode == Mode.Read)
            {
				// TODO: may be _readerStream already has opened stream. You need close it before opening a new one.

				// TODO: use CSVReader here
                _readerStream = File.OpenText(fileName);
            }
            else if (mode == Mode.Write)
            {
				// TODO: may be _readerStream already has opened stream. You need close it before opening a new one.

				// TODO: use CSVReader here
				FileInfo fileInfo = new FileInfo(fileName); 
                _writerStream = fileInfo.CreateText();
            }
            else
            {
				// TODO: it is more informative to say what mode is unknown then a file name
				// TODO: to concatinate strings use String.Format()
                throw new Exception("Unknown file mode for " + fileName);
            }
        }

        public void Write(params string[] columns)
        {
			// TODO: always check input params for public methods

			// TODO: move next code to CSVWriter.Write(string[] columns)

            string outPut = "";

			// TODO: use string.Join() to join strings with some separators
            for (int i = 0; i < columns.Length; i++)
            {
                outPut += columns[i]; 
                if ((columns.Length - 1) != i)
                {
					// TODO: use separator as variable
                    outPut += "\t";
                }
            }

            WriteLine(outPut);
        }

		// TODO: 
		// Wrong method because it never return column1 and column2 with results. Hovewer it could used to check if success reading of next line was or not.
		// It is need for backward compatibility to has this method. Mark this method as 'obsolete' and return a boolean result of CSVWriter.Read().
		// This method and 'Read(out string column1, out string column2)' have code duplication. You need reuse method in other method. 
		// The second way is to create and use the third method with common code.
        public bool Read(string column1, string column2)
        {
			// TODO: always check input params for public methods			

            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();
            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            }
            else
            {
                column1 = columns[FIRST_COLUMN];
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

		// TODO: Not extended solution. Better to have method bool Read(out string[] columns).
		// TODO: Implement a new method in CSVReader and mark the current method as 'obsolete'.
		// TODO: Use CSVReader.Read(out string[] columns) in all reading methods of CSVReaderWriter.
        public bool Read(out string column1, out string column2)
        {
            const int FIRST_COLUMN = 0;
            const int SECOND_COLUMN = 1;

            string line;
            string[] columns;

            char[] separator = { '\t' };

            line = ReadLine();

            if (line == null)
            {
                column1 = null;
                column2 = null;

                return false;
            }

            columns = line.Split(separator);

            if (columns.Length == 0)
            {
                column1 = null;
                column2 = null;

                return false;
            } 
            else
            {
                column1 = columns[FIRST_COLUMN];

				// TODO: check columns.Length before access to second column. Define constant MIN_COLUMN_COUNT for it. 
                column2 = columns[SECOND_COLUMN];

                return true;
            }
        }

        private void WriteLine(string line)
        {
			// TODO: Too simple function. It could use inline;
            _writerStream.WriteLine(line);
        }

        private string ReadLine()
        {
			// TODO: Too simple function. It could use inline;
            return _readerStream.ReadLine();
        }
        
		public void Close()
        {
            if (_writerStream != null)
            {
                _writerStream.Close();
				// TODO: set _writerStream = null to mark _writerStream as disposed.  
            }

            if (_readerStream != null)
            {
                _readerStream.Close();
				// TODO: set _readerStream = null to mark stream as disposed.
            }
        }
    }
}
