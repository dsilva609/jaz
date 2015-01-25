using jaz.Data;
using System;
using System.Reflection;

namespace jaz
{
	public class MainWrapper
	{
		public static void Main(string[] args)
		{
			var parser = new FileParser();

			var data = parser.ReadData(@"Resources\foo.jaz");

			Console.WriteLine(data.Length);
		}
	}
}
