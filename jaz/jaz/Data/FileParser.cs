using System;
using System.IO;

namespace jaz.Data
{
	public class FileParser
	{
		public String[] ReadData(string filename)
		{
			return File.ReadAllLines(filename);

		}
	}
}
