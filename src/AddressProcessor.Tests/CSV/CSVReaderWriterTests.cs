using System;
using System.Collections.Generic;

using AddressProcessing.CSV;
using AddressProcessing.Exceptions;

using DataProviders;

using Moq;

using NUnit.Framework;

namespace AddressProcessing.Tests.CSV
{
	[TestFixture]
	public class CSVReaderWriterTests
	{
		private const string _testInputFile = @"test_data\contacts.csv";

		#region ctor
			
		[Test]
		public void ctorWithFileName_Open_Success()
		{			
			using (new CSVReaderWriter(_testInputFile, CSVReaderWriter.Mode.Read))
			{				
			}
		}

		#endregion

		#region Open

		[Test]		
		public void Open_CloseOpenedForReadBeforeOpenNew_OpenedWasClosed()
		{
			// Arrange	
			var mockReader = new Mock<IReader>();
			using (var readerWriterUnderTest = new CSVReaderWriter(mockReader.Object))
			{
				// Act				
#pragma warning disable 618
				readerWriterUnderTest.Open(_testInputFile, CSVReaderWriter.Mode.Read);
#pragma warning restore 618

				// Assert 				
				mockReader.Verify(reader => reader.Close(), Times.Once);
			}
		}

		[Test]
		public void Open_CloseOpenedForWriteBeforeOpenNew_OpenedWasClosed()
		{
			// Arrange	
			var mockWriter = new Mock<IWriter>();
			using (var writerUnderTest = new CSVReaderWriter(mockWriter.Object))
			{
				// Act				
#pragma warning disable 618
				writerUnderTest.Open(_testInputFile, CSVReaderWriter.Mode.Write);
#pragma warning restore 618

				// Assert 				
				mockWriter.Verify(writer => writer.Close(), Times.Once);
			}
		}  

		#endregion

		#region Read/Write

		[Test]		
		public void Read_UslessParamsPassed_ReturnSuccessAndNotSuccess()
		{
			// Arrange	
			var columnsInLines = new List<string[]>
			{				 
				new [] {"123"},				
			};

			var mockReader = CreateMockReader(columnsInLines);
			using (var readerWriterUnderTest = new CSVReaderWriter(mockReader.Object))
			{
				// Act
				var column1 = string.Empty;
				var column2 = string.Empty;
#pragma warning disable 618
				var isReadSuccess = readerWriterUnderTest.Read(column1, column2);
				Assert.IsTrue(isReadSuccess);				
				isReadSuccess = readerWriterUnderTest.Read(column1, column2);
				Assert.IsFalse(isReadSuccess);				
#pragma warning restore 618
			}
		}

		[Test]
		[ExpectedException(typeof(NullReferenceException), ExpectedMessage = "_readerStream")]
		public void Read_StreamWasNotOpened_ThrowException()
		{
			// Arrange	
			var columnsInLines = new List<string[]>
			{				 
				new [] {"123"},				
			};

			var mockReader = CreateMockReader(columnsInLines);
			var readerWriterUnderTest = new CSVReaderWriter(mockReader.Object);
			readerWriterUnderTest.Close();

			// Act
			var column1 = string.Empty;
			var column2 = string.Empty;
#pragma warning disable 618
			readerWriterUnderTest.Read(column1, column2);				
#pragma warning restore 618
			
		}

		#endregion

		#region Format

		[Test]
		[ExpectedException(typeof(WrongFieldsNumberException), ExpectedMessage = "Wrong fields number in line 0. Expected 2 but was found 1.")]
		public void Read_FieldsLessThenNeed_ThrowException()
		{
			// Arrange	
			var columnsInLines = new List<string[]>
			{				 
				new [] {"789"},
			};

			var mockReader = CreateMockReader(columnsInLines);
			using (var readerWriterUnderTest = new CSVReaderWriter(mockReader.Object))
			{
				// Act
				string[] columns;
				readerWriterUnderTest.Read(out columns);	
			}			
		}

		#endregion
		
		#region Disposing

		[Test]
		[ExpectedException(typeof(NullReferenceException), ExpectedMessage = "_readerStream")]
		public void Read_ReadDisposed_ThrowException()
		{
			// Arrange	
			var columnsInLines = new List<string[]>
			{				 
				new [] {"1", "2"},
				new [] {"3", "4"},
			};

			var mockReader = CreateMockReader(columnsInLines);
			using (var readerWriterUnderTest = new CSVReaderWriter(mockReader.Object))
			{
				// Act
				string[] columns;
				readerWriterUnderTest.Read(out columns);
				readerWriterUnderTest.Dispose();
				readerWriterUnderTest.Read(out columns);
			}
		}

		[Test]
		[ExpectedException(typeof(NullReferenceException), ExpectedMessage = "_writerStream")]
		public void Write_WriteDisposed_ThrowException()
		{
			// Arrange				
			var mockWriter = new Mock<IWriter>();
			using (var readerWriterUnderTest = new CSVReaderWriter(mockWriter.Object))
			{
				// Act
				string[] columns = { "1", "2" };
				readerWriterUnderTest.Write(columns);
				readerWriterUnderTest.Dispose();
				readerWriterUnderTest.Write(columns);
			}
		} 
		#endregion

		#region Helpers

		/// <summary>
		/// Emulating streamReader data. 
		/// When IReader.Read() will call then next line with columns return. When last line return then return null.		 
		/// </summary>
		/// <param name="testLinesFields">List of lines. Each line contain array of columns.</param>		
		private static Mock<IReader> CreateMockReader(IEnumerable<string[]> testLinesFields)
		{
			var mockReader = new Mock<IReader>();
			var linesQueue = new Queue<string[]>();
			foreach (var currentItem in testLinesFields)
				linesQueue.Enqueue(currentItem);

			linesQueue.Enqueue(null);

			mockReader.Setup(x => x.Read()).Returns(linesQueue.Dequeue);
			return mockReader;
		} 
		#endregion
	}
}
