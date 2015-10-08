using System;

using NUnit.Framework;

namespace Helpers.Common.Tests
{
	[TestFixture]
	public class ExtensionsTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void ThrowIfNull_SetNull_ThrowException()
		{
			object obj = null;
			obj.ThrowIfNull("obj");
		}

		[Test]		
		public void ThrowIfNull_SetNotNull_Success()
		{
			object obj = new Object();
			obj.ThrowIfNull("obj");
		}

		[Test]
		public void ToStream_PutString_SuccessfullyReadStringFromStream()
		{
			string str = "1a\n2b";
			using (var stream = str.ToStream())
			{
				var line1 = stream.ReadLine();
				Assert.True(line1 == "1a");
				var line2 = stream.ReadLine();
				Assert.True(line2 == "2b");
				var line3 = stream.ReadLine();
				Assert.True(line3 == null);
			}
		}
	}
}
