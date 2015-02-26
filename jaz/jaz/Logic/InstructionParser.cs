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
			Guid currentCoupledUnindentedLabelReturnGUID = new Guid();
			Guid currentCoupledIndentedLabelReturnGUID = new Guid();
			bool labelFrontWasTrimmed = true;
			bool returnFrontWasTrimmed = true;

			foreach (var item in data)
			{
				string temp = string.Empty;
				var instruction = new Instruction();
				if (!item.Contains("label") || item.Contains("return"))
					temp = item.Trim();
				else
					temp = item;

				if (String.IsNullOrWhiteSpace(temp))
				{
					temp = null;
					continue;
				}
				else if (temp.Contains(" ") && !temp.Contains("return"))
				{
					if (temp.Contains("label") && !char.IsWhiteSpace(temp, 0))
						labelFrontWasTrimmed = false;

					temp = temp.Trim();
					instruction.Command = temp.Substring(0, temp.IndexOf(" "));
					instruction.Value = temp.Substring(temp.IndexOf(" ") + 1);
					instruction.GUID = Guid.NewGuid();

					if (instruction.Command == InstructionSet.Label && !labelFrontWasTrimmed)
						currentCoupledUnindentedLabelReturnGUID = instruction.GUID;
					else if (instruction.Command == InstructionSet.Label && labelFrontWasTrimmed)
						currentCoupledIndentedLabelReturnGUID = instruction.GUID;

					labelFrontWasTrimmed = true;
				}
				else
				{
					if (temp.Contains("return") && !char.IsWhiteSpace(temp, 0))
						returnFrontWasTrimmed = false;

					temp = temp.Trim();
					instruction.Command = temp;
					instruction.GUID = Guid.NewGuid();

					if (instruction.Command == InstructionSet.Begin)
						currentBeginGUID = instruction.GUID;
					if (instruction.Command == InstructionSet.End)
						instruction.GUID = currentBeginGUID;

					if (instruction.Command == InstructionSet.Return && !returnFrontWasTrimmed)
						instruction.GUID = currentCoupledUnindentedLabelReturnGUID;
					else if (instruction.Command == InstructionSet.Return && returnFrontWasTrimmed)
						instruction.GUID = currentCoupledIndentedLabelReturnGUID;

					returnFrontWasTrimmed = true;
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