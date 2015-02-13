using jaz.Objects;
using System;
using System.Collections.Generic;

namespace jaz.Logic
{
	public class InstructionParser
	{
		private List<Instruction> _instructionList;

		public InstructionParser()
		{
			this._instructionList = new List<Instruction>();
		}

		public List<Instruction> Execute(ref string[] data)
		{
			this.Parse(ref data);
			return this._instructionList;
		}

		private void Parse(ref string[] data)
		{
			foreach (var item in data)
			{
				var instruction = new Instruction();
				var temp = item.Trim();

				if (String.IsNullOrWhiteSpace(temp))
				{
					temp = null;
					continue;
				}
				else if (temp.Contains(" "))
				{
					instruction.Command = temp.Substring(0, temp.IndexOf(" "));
					instruction.Value = temp.Substring(temp.IndexOf(" ") + 1);
					Console.WriteLine(temp.Substring(0, temp.IndexOf(" ")));
				}
				else
				{
					instruction.Command = temp;
					Console.WriteLine(temp);
				}
				this._instructionList.Add(instruction);

				instruction = null;
				temp = null;
			}
		}
	}
}