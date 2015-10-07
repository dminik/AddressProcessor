using System;
using System.IO;

namespace Helpers.Common
{
	public static class Extensions
	{
		public static T ThrowIfNull<T>(this T argument, string argumentName = "")
		{
			if (argument == null)
				throw new ArgumentNullException(argumentName);

			return argument;
		}

		public static StreamReader ToStream(this string s)
		{
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(s);
			writer.Flush();
			stream.Position = 0;
			return new StreamReader(stream);
		}
	}
}
