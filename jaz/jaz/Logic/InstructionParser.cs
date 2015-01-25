using System;

namespace jaz.Logic
{
	public class InstructionParser
	{
		public string[] Execute(ref string[] data)
		{
			return this.Parse(ref data);
		}

		private string[] Parse(ref string[] data)
		{
			foreach (var item in data)
			{
				var temp = item.Trim();

				if (String.IsNullOrWhiteSpace(temp))
				{
					temp = null;
					continue;
				}
				else if (temp.Contains(" "))
					Console.WriteLine(temp.Substring(0, temp.IndexOf(" ")));
				else
					Console.WriteLine(temp);
				temp = null;
			}
			return null;
		}
	}
}