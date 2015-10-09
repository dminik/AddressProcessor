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
		private const string _testFileForWrite = @"..\..\test_data\contacts3.csv";
		private const string _testFileForRead = @"..\..\test_data\contacts2.csv";		

		#region ctor
			
		[Test]
		public void ctorWithFileName_Open_Success()
		{			
			using (new CSVReaderWriter(_testFileForRead, CSVReaderWriter.Mode.Read))
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
#pragma warning disable 618 // obsolete warning
				readerWriterUnderTest.Open(_testFileForRead, CSVReaderWriter.Mode.Read);
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
#pragma warning disable 618 // obsolete warning
				writerUnderTest.Open(_testFileForWrite, CSVReaderWriter.Mode.Write);
#pragma warning restore 618

				// Assert 				
				mockWriter.Verify(writer => writer.Close(), Times.Once);
			}
		}  

		#endregion

		#region Read/Write

		[Test]		
		public void Read_UselessParamsPassed_ReturnSuccessAndNotSuccess()
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
#pragma warning disable 618 // obsolete warning
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
#pragma warning disable 618 // obsolete warning
			readerWriterUnderTest.Read(column1, column2);				
#pragma warning restore 618
			
		}

		[Test]		
		public void Write_WriteLine_Success()
		{
			// Arrange				
			var mockWriter = new Mock<IWriter>();
			using (var readerWriterUnderTest = new CSVReaderWriter(mockWriter.Object))
			{
				// Act
				string[] columns = { "1", "2" };
				readerWriterUnderTest.Write(columns);				
			}

			// Assert 				
			mockWriter.Verify(writer => writer.Write(It.IsAny<string[]>()), Times.Once);
		}

		[Test]
		public void ReadWrite_OpenRealFilesForReadAndWriteForOneInstance_Success()
		{
			// Arrange				
			var mockWriter = new Mock<IWriter>();
#pragma warning disable 618 // obsolete warning 
			using (var readerWriterUnderTest = new CSVReaderWriter(mockWriter.Object))

			{
				// Act

				// writing
				string[] columns = { "1", "2" };
				readerWriterUnderTest.Write(columns);

				// reading
				readerWriterUnderTest.Open(_testFileForRead, CSVReaderWriter.Mode.Read);
				var isReadSuccess = readerWriterUnderTest.Read(out columns);

				// Assert
				mockWriter.Verify(writer => writer.Write(It.IsAny<string[]>()), Times.Once);
				Assert.IsTrue(isReadSuccess);
			}

#pragma warning restore 618			 							
		}

		[Test]
		public void Read_OpenRealFile_Success()
		{
			// Arrange				
			
#pragma warning disable 618 // obsolete warning
			using (var readerWriterUnderTest = new CSVReaderWriter())
			{
				// Act

				
				// reading
				readerWriterUnderTest.Open(_testFileForRead, CSVReaderWriter.Mode.Read);
				string[] columns;
				var isReadSuccess = readerWriterUnderTest.Read(out columns);

				// Assert
				
				Assert.IsTrue(isReadSuccess);
			}

#pragma warning restore 618
		}

		[Test]
		public void Read_ReadLines_Success()
		{
			// Arrange	
			var columnsInLines = new List<string[]>
			{				 
				new [] {"123", "456", },				
				new [] {"789", "101", },	
			};

			var mockReader = CreateMockReader(columnsInLines);
			using (var readerWriterUnderTest = new CSVReaderWriter(mockReader.Object))
			{
				// Act
				string[] columns;
				
				var isReadSuccess = readerWriterUnderTest.Read(out columns);
				Assert.IsTrue(isReadSuccess);
				Assert.AreEqual(2, columns.Length);
				Assert.AreEqual("123", columns[0]);
				Assert.AreEqual("456", columns[1]);

				isReadSuccess = readerWriterUnderTest.Read(out columns);
				Assert.IsTrue(isReadSuccess);
				Assert.AreEqual(2, columns.Length);
				Assert.AreEqual("789", columns[0]);
				Assert.AreEqual("101", columns[1]);

				isReadSuccess = readerWriterUnderTest.Read(out columns);
				Assert.IsFalse(isReadSuccess);
				Assert.IsNull(columns);
			}
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

		[Test]
		public void CreateMockReader_ReadLines_Success()
		{
			// Arrange	
			var columnsInLines = new List<string[]> { new[] { "123", "456", }, new[] { "789", "101", }, };
			
			using (var mockReader = CreateMockReader(columnsInLines).Object)
			{				
				string[] columns;

				columns = mockReader.Read();
				Assert.NotNull(columns);
				Assert.AreEqual(2, columns.Length);
				Assert.AreEqual("123", columns[0]);
				Assert.AreEqual("456", columns[1]);

				columns = mockReader.Read();
				Assert.NotNull(columns);
				Assert.AreEqual(2, columns.Length);
				Assert.AreEqual("789", columns[0]);
				Assert.AreEqual("101", columns[1]);

				columns = mockReader.Read();
				Assert.IsNull(columns);
			}
		}


		#endregion
	}
}
