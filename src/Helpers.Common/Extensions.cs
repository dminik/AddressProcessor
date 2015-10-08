using System;
using System.IO;

namespace Helpers.Common
{
	public static class Extensions
	{
		public static void ThrowIfNull<T>(this T argument, string argumentName = "")
		{
			if (argument == null)
				throw new ArgumentNullException(argumentName);
		}

		public static void ThrowIfNullOrEmpty(this string argument, string argumentName = "")
		{
			if (argument == null)
				throw new ArgumentNullException(argumentName);
			if (argument == string.Empty)
				throw new ArgumentException(argumentName);
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
