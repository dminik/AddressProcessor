using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using DataProviders.CSV;
using NUnit.Framework;

namespace DataProviders.Tests.CSV
{
	[TestFixture]
	public class CsvWriterTests
	{
		string fileName = @"..\..\SampleData\write.txt";
		const string DELIMETER_AS_SPACE = " ";
		const string DELIMETER_AS_SEMICOLON = ";";

		[SetUp]
		public void Setup()
		{
			fileName = Path.Combine(ConfigurationManager.AppSettings.Get("sampleDataPath"), "quotesUTF8.txt");
		}

		[Test]
		public void Write_RealFile_Success()
		{
			// Arrange
			if (File.Exists(fileName))
				File.Delete(fileName);

			try
			{
				// Act
				using (var writer = new CsvWriter(fileName, DELIMETER_AS_SPACE))
				{
					string[] values1 = { "qw", "er" };
					writer.Write(values1);
					string[] values2 = { "ty", "ui" };
					writer.Write(values2);
					writer.Close();
				}

				// Assert
				var fileLines = File.ReadAllLines(fileName);
				Assert.AreEqual(2, fileLines.Count(), "Invalid file exported");
				Assert.AreEqual("qw er", fileLines[0]);
				Assert.AreEqual("ty ui", fileLines[1]);
			}
			finally
			{
				if (File.Exists(fileName))
					File.Delete(fileName);
			}
		}

		[Test]
		public void Write_RealFileUTF8_Success()
		{
			// Arrange
			if (File.Exists(fileName))
				File.Delete(fileName);

			try
			{
				// Act
				using (var writer = new CsvWriter(fileName, DELIMETER_AS_SPACE))
				{
					string[] values1 = { "è", "€" };
					writer.Write(values1);
					string[] values2 = { "è2", "€2" };
					writer.Write(values2);
					writer.Close();
				}

				// Assert
				var fileLines = File.ReadAllLines(fileName);
				Assert.AreEqual(2, fileLines.Count(), "Invalid file exported");
				Assert.AreEqual("è €", fileLines[0]);
				Assert.AreEqual("è2 €2", fileLines[1]);
			}
			finally
			{
				if (File.Exists(fileName))
					File.Delete(fileName);
			}
		}

		[Test]
		public void Write_TwoStringsWithSemicolonDelimeter_Success()
		{
			// Arrange			
			using (var stream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(stream))

				// Act
				using (var writerUnderTest = new CsvWriter(streamWriter, DELIMETER_AS_SEMICOLON))
				{
					string[] values1 = { "qw", "er" };
					writerUnderTest.Write(values1);
					string[] values2 = { "ty", "ui" };
					writerUnderTest.Write(values2);
					writerUnderTest.Close();
				}

				// Assert
				var expectedLines = string.Format("qw;er{0}ty;ui{0}", Environment.NewLine);
				var actualLines = Encoding.UTF8.GetString(stream.ToArray());
				Assert.AreEqual(expectedLines, actualLines);
			}
		}

		[Test]
		[ExpectedException(typeof(ObjectDisposedException))]
		public void Write_WriterDisposed_ThrowException()
		{
			// Arrange	
			using (var stream = new MemoryStream())
			{
				using (var streamWriter = new StreamWriter(stream))
				{
					// Act
					var writerUnderTest = new CsvWriter(streamWriter);

					string[] values1 = { "qw" };
					writerUnderTest.Write(values1);
					writerUnderTest.Close();

					string[] values2 = { "ty" };
					writerUnderTest.Write(values2);
				}
			}
		}
	}
}
