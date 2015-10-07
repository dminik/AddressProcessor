using System;

namespace DataProviders
{
	public interface IWriter : IDisposable
	{
		void Write(string[] data);
		void Close();
	}
}