using jaz.Data;
using jaz.Objects;
using System.Collections.Generic;

namespace jaz.Logic
{
	public class Interpreter
	{
		private FileParser _fileParser;
		private InstructionParser _instructionParser;
		private InstructionSetHandler _instructionSetHandler;
		private string[] _data;
		private List<Instruction> _instructions;

		public Interpreter(string filename)
		{
			this._fileParser = new FileParser(filename);
			this._instructionParser = new InstructionParser();
			this._instructionSetHandler = new InstructionSetHandler();

			//--add test file selector
		}

		public void Execute()
		{
			this._data = this._fileParser.ExecuteRead();
			this._instructions = this._instructionParser.Execute(ref this._data);

			foreach (var item in this._instructions)
			{
				DetermineAndExecuteInstructionOperation(item);
			}
		}

		private void DetermineAndExecuteInstructionOperation(Instruction item)
		{
			switch (item.Command)
			{
				default:
					break;

				case InstructionSet.Addition:
					this._instructionSetHandler.Addition();
					break;

				case InstructionSet.AND:
					this._instructionSetHandler.AND();
					break;

				case InstructionSet.Begin:
					this._instructionSetHandler.Begin();
					break;

				case InstructionSet.Call:
					this._instructionSetHandler.Call();
					break;

				case InstructionSet.Copy:
					this._instructionSetHandler.Copy();
					break;

				case InstructionSet.Division:
					this._instructionSetHandler.Division();
					break;

				case InstructionSet.End:
					this._instructionSetHandler.End();
					break;

				case InstructionSet.Equal:
					this._instructionSetHandler.Equal();
					break;

				case InstructionSet.GoFalse:
					this._instructionSetHandler.GoFalse();
					break;

				case InstructionSet.GoTo:
					this._instructionSetHandler.GoTo();
					break;

				case InstructionSet.GoTrue:
					this._instructionSetHandler.GoTrue();
					break;

				case InstructionSet.Greater:
					this._instructionSetHandler.Greater();
					break;

				case InstructionSet.GreaterOrEqual:
					this._instructionSetHandler.GreaterOrEqual();
					break;

				case InstructionSet.Halt:
					this._instructionSetHandler.Halt();
					break;

				case InstructionSet.Label:
					this._instructionSetHandler.Label();
					break;

				case InstructionSet.Lesser:
					this._instructionSetHandler.Lesser();
					break;

				case InstructionSet.LesserOrEqual:
					this._instructionSetHandler.LesserOrEqual();
					break;

				case InstructionSet.LValue:
					this._instructionSetHandler.LValue(item.Value);
					break;

				case InstructionSet.Multuplication:
					this._instructionSetHandler.Multuplication();
					break;

				case InstructionSet.NOT:
					this._instructionSetHandler.NOT();
					break;

				case InstructionSet.NotEqual:
					this._instructionSetHandler.NotEqual();
					break;

				case InstructionSet.OR:
					this._instructionSetHandler.OR();
					break;

				case InstructionSet.Pop:
					this._instructionSetHandler.Pop();
					break;

				case InstructionSet.Print:
					this._instructionSetHandler.Print();
					break;

				case InstructionSet.Push:
					this._instructionSetHandler.Push(item);
					break;

				case InstructionSet.Remainder:
					this._instructionSetHandler.Remainder();
					break;

				case InstructionSet.Return:
					this._instructionSetHandler.Return();
					break;

				case InstructionSet.RValue:
					this._instructionSetHandler.RValue(item.Value);
					break;

				case InstructionSet.Show:
					this._instructionSetHandler.Show(item.Value);
					break;

				case InstructionSet.Subtraction:
					this._instructionSetHandler.Subtraction();
					break;

				case InstructionSet.Top:
					this._instructionSetHandler.Top();
					break;
			}
		}
	}
}