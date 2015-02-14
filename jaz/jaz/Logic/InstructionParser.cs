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
			this.AssociateCoupling();

			return this._instructionList;
		}

		private void Parse(ref string[] data)
		{
			Guid currentBeginGUID = new Guid();
			//Guid currentFunctionGUID = new Guid();
			Guid currentCoupledLabelReturnGUID = new Guid();
			bool callAndLabelAreCoupled = false;//is this needed?
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

					//if (instruction.Command == InstructionSet.Call)
					//	{
					//		currentCoupledGUID = instruction.GUID;
					//		currentCallName = instruction.Value;
					//		callAndLabelAreCoupled = true;
					//	}
					if (instruction.Command == InstructionSet.Label)
					{
						currentCoupledLabelReturnGUID = instruction.GUID;
						//	callAndLabelAreCoupled = false;
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

					if (instruction.Command == InstructionSet.Return)//calls are not always defined before labels, so labels and calls need to be coupled first then calls to label/couple pairs
						instruction.GUID = currentCoupledLabelReturnGUID;//make sure these are not duplicated

					//Console.WriteLine(temp);
				}
				this._instructionList.Add(instruction);

				instruction = null;
				temp = null;
			}
		}

		private void AssociateCoupling()
		{
			var calls = this._instructionList.FindAll(x => x.Command == InstructionSet.Call);

			foreach (var item in calls)
			{
				Instruction label = this._instructionList.Find(x => x.Command == InstructionSet.Label && x.Value == item.Value);
				Guid tempGuid = label.GUID;
				this._instructionList[this._instructionList.IndexOf(label)].GUID = item.GUID;
				this._instructionList[this._instructionList.FindIndex(x => x.Command == InstructionSet.Return && x.GUID == tempGuid)].GUID = item.GUID;
			}
		}
	}
}