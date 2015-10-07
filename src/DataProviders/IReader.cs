using System;

namespace DataProviders
{
	public interface IReader : IDisposable
	{
		string[] Read();
	}
}