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
	}
}
