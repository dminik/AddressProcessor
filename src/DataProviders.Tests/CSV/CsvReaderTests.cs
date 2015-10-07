using System;
using System.Configuration;
using System.IO;
using System.Linq;
using DataProviders.CSV;
using Helpers.Common;
using NUnit.Framework;

namespace DataProviders.Tests.CSV
{
	[TestFixture]
	public class CsvReaderTests
	{
		const char DELIMETER_AS_SPACE = ' ';

		[Test]
		public void Read_RealFile_Success()
		{
			// Arrange
			var filePath = Path.Combine(ConfigurationManager.AppSettings.Get("sampleDataPath"), "quotes.txt");
			using (var reader = new CsvReader(filePath))
			{
				var lineCounter = 0;

				// Act
				while (reader.Read() != null)
					lineCounter++;

				// Assert
				var LINES_IN_SAMPLE_FILE = 1000;
				Assert.AreEqual(LINES_IN_SAMPLE_FILE, lineCounter);
			}
		}

		[Test]
		public void Read_RealFileUTF8_Success()
		{
			// Arrange
			var filePath = Path.Combine(ConfigurationManager.AppSettings.Get("sampleDataPath"), "quotesUTF8.txt");
			using (var reader = new CsvReader(filePath))
			{
				// Act
				var values = reader.Read();

				// Assert
				Assert.IsNotNull(values);
				Assert.AreEqual(2, values.Count());
				Assert.AreEqual("è", values[0]);
				Assert.AreEqual("€", values[1]);

				// Act
				values = reader.Read();

				// Assert
				Assert.IsNotNull(values);
				Assert.AreEqual(2, values.Count());
				Assert.AreEqual("è2", values[0]);
				Assert.AreEqual("€2", values[1]);
			}
		}

		[Test]
		public void Read_TwoStringsAndTwoFields_Success()
		{
			// Arrange
			var str = "123 4567\nabc grs";

			using (var stream = str.ToStream())
			{
				// Act			
				using (var reader = new CsvReader(stream, ' '))
				{
					// Assert
					string[] values = null;
					values = reader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(2, values.Count());
					Assert.AreEqual("123", values[0]);
					Assert.AreEqual("4567", values[1]);

					values = reader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(2, values.Count());
					Assert.AreEqual("abc", values[0]);
					Assert.AreEqual("grs", values[1]);

					values = reader.Read();
					Assert.IsNull(values);

					reader.Close();
				}
			}
		}

		[Test]
		public void Read_TwoStringsWithSemicolonDelimeter_Success()
		{
			// Arrange
			const char DELIMETER_AS_SEMICOLON = ';';
			var str = "123;4567\nabc;grs";

			using (var stream = str.ToStream())
			{
				// Act			
				using (var reader = new CsvReader(stream, DELIMETER_AS_SEMICOLON))
				{
					// Assert
					string[] values = null;
					values = reader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(2, values.Count());
					Assert.AreEqual("123", values[0]);
					Assert.AreEqual("4567", values[1]);

					values = reader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(2, values.Count());
					Assert.AreEqual("abc", values[0]);
					Assert.AreEqual("grs", values[1]);

					values = reader.Read();
					Assert.IsNull(values);

					reader.Close();
				}
			}
		}

		[Test]
		public void Read_StringWithTwoFilledFieldsAndOneEmptyField_Success()
		{
			// Arrange			
			const string INPUT_STR = "123  45";

			using (var inputStream = INPUT_STR.ToStream())
			{
				// Act			
				using (var inputReader = new CsvReader(inputStream, DELIMETER_AS_SPACE))
				{
					// Assert
					string[] values = null;
					values = inputReader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(3, values.Count());
					Assert.AreEqual("123", values[0]);
					Assert.AreEqual("", values[1]);
					Assert.AreEqual("45", values[2]);
				}
			}
		}

		[Test]
		public void Read_ExistsEmptyString_ReadEmptyString()
		{
			// Arrange			
			const string INPUT_STR = "123\n\n456";

			using (var stream = INPUT_STR.ToStream())
			{
				// Act			
				using (var reader = new CsvReader(stream, DELIMETER_AS_SPACE))
				{
					// Assert
					string[] values = null;
					values = reader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(1, values.Count());
					Assert.AreEqual("123", values[0]);

					values = reader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(1, values.Count());
					Assert.AreEqual(string.Empty, values[0]);

					values = reader.Read();
					Assert.IsNotNull(values);
					Assert.AreEqual(1, values.Count());
					Assert.AreEqual("456", values[0]);

					reader.Close();
				}
			}
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void Read_ReaderDisposed_ThrowException()
		{
			// Arrange			
			var str = "123 4567\n89 10";

			using (var inputStream = str.ToStream())
			{
				// Act			
				var inputReader = new CsvReader(inputStream);

				inputReader.Read();
				inputReader.Close();
				inputReader.Read();
			}
		}
	}
}
