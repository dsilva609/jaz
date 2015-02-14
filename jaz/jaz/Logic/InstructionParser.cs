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
			Guid currentBeginGUID = new Guid();
			//Guid currentFunctionGUID = new Guid();
			Guid currentCoupledGUID = new Guid();
			bool callAndLabelAreCoupled = false;
			string currentCallName = string.Empty;

			//	Guid guid;
			foreach (var item in data)
			{
				var instruction = new Instruction();
				var temp = item.Trim();
				//	guid = new Guid();

				if (String.IsNullOrWhiteSpace(temp))
				{
					temp = null;
					continue;
				}
				else if (temp.Contains(" "))
				{
					instruction.Command = temp.Substring(0, temp.IndexOf(" "));
					instruction.Value = temp.Substring(temp.IndexOf(" ") + 1);
					instruction.GUID = Guid.NewGuid();

					if (instruction.Command == InstructionSet.Call)
					{
						currentCoupledGUID = instruction.GUID;
						currentCallName = instruction.Value;
					}
					if (instruction.Command == InstructionSet.Label && instruction.Value == currentCallName)
					{
						instruction.GUID = currentCoupledGUID;
					}
					//Console.WriteLine(temp.Substring(0, temp.IndexOf(" ")));
				}
				else
				{
					instruction.Command = temp;
					instruction.GUID = Guid.NewGuid();

					if (instruction.Command == InstructionSet.Begin)
						currentBeginGUID = instruction.GUID;
					if (instruction.Command == InstructionSet.End)
						instruction.GUID = currentBeginGUID;//make sure these are not duplicated

					if (instruction.Command == InstructionSet.Return)
						instruction.GUID = currentCoupledGUID;//make sure these are not duplicated

					//Console.WriteLine(temp);
				}
				this._instructionList.Add(instruction);

				instruction = null;
				temp = null;
			}
		}
	}
}