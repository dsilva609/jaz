using System;
using System.IO;

namespace jaz.Data
{
	public class FileParser
	{
		private readonly string _filename;

		public FileParser(string filename)
		{
			this._filename = filename;
		}
		public String[] ExecuteRead()
		{
			return File.ReadAllLines(this._filename);

		}
	}
}