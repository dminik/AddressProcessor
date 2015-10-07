using System;

namespace DataProviders
{
	public interface IReader : IDisposable
	{
		/// <summary>
		/// Read next line, split it by delimeter and return array of fields.		
		/// Returns:
		///		Array of fields.
		///		If line was empty (or has only white spaces) then return empty array.
		///		If no line was found then return null.
		/// </summary>
		string[] Read();

		uint ProcessedLines { get; }

		void Close();
	}
}