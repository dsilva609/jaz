using jaz.Data;
using System;
using System.IO;
using System.Reflection;

namespace jaz
{
	public class MainWrapper
	{
		public static void Main(string[] args)
		{
			var parser = new FileParser();

			var data = parser.ReadData(Assembly.GetExecutingAssembly().Location.Replace("\\bin\\Debug\\jaz.exe", "") + @"\\Resources\\foo.jaz");//@"..\..\Resources\foo.jaz");


			Console.WriteLine(data.Length);

			Console.WriteLine(new Uri(@"Resources\foo.jaz", UriKind.Relative).ToString());
		}
	}
}
