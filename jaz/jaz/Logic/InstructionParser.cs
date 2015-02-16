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
			Guid currentCoupledUnindentedLabelReturnGUID = new Guid();
			Guid currentCoupledIndentedLabelReturnGUID = new Guid();
			bool labelFrontWasTrimmed = true;
			bool returnFrontWasTrimmed = true;
			//	bool callAndLabelAreCoupled = false;//is this needed?
			//string currentCallName = string.Empty;

			//	Guid guid;
			foreach (var item in data)
			{
				string temp = string.Empty;
				var instruction = new Instruction();
				if (!item.Contains("label") || item.Contains("return"))
					temp = item.Trim();
				else
					temp = item;
				//	guid = new Guid();

				if (String.IsNullOrWhiteSpace(temp))
				{
					temp = null;
					continue;
				}
				else if (temp.Contains(" ") && !temp.Contains("return")) //make sure that correct values are hitting correct statements
				{
					if (temp.Contains("label") && !char.IsWhiteSpace(temp, 0))//label starting with space or tab
						labelFrontWasTrimmed = false;

					temp = temp.Trim();
					instruction.Command = temp.Substring(0, temp.IndexOf(" "));
					instruction.Value = temp.Substring(temp.IndexOf(" ") + 1);
					instruction.GUID = Guid.NewGuid();

					if (instruction.Command == InstructionSet.Label && !labelFrontWasTrimmed)//indented label vs non indented?
						currentCoupledUnindentedLabelReturnGUID = instruction.GUID;
					else if (instruction.Command == InstructionSet.Label && labelFrontWasTrimmed)
						currentCoupledIndentedLabelReturnGUID = instruction.GUID;

					labelFrontWasTrimmed = true;

					//Console.WriteLine(temp.Substring(0, temp.IndexOf(" ")));
				}
				else
				{
					if (temp.Contains("return") && !char.IsWhiteSpace(temp, 0))//return starts with space or tab
						returnFrontWasTrimmed = false;

					temp = temp.Trim();
					instruction.Command = temp;
					instruction.GUID = Guid.NewGuid();

					if (instruction.Command == InstructionSet.Begin)
						currentBeginGUID = instruction.GUID;
					if (instruction.Command == InstructionSet.End)
						instruction.GUID = currentBeginGUID;//make sure these are not duplicated

					if (instruction.Command == InstructionSet.Return && !returnFrontWasTrimmed)//calls are not always defined before labels, so labels and calls need to be coupled first then calls to label/couple pairs
						instruction.GUID = currentCoupledUnindentedLabelReturnGUID;//make sure these are not duplicated
					else if (instruction.Command == InstructionSet.Return && returnFrontWasTrimmed)
						instruction.GUID = currentCoupledIndentedLabelReturnGUID;

					returnFrontWasTrimmed = true;
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
				this._instructionList[this._instructionList.FindIndex(x => x.Command == InstructionSet.Return && x.GUID == tempGuid)].GUID = item.GUID;//label within a label issue, couples to the closest label
			}
		}
	}
}