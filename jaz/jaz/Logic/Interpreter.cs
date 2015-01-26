using jaz.Data;
using jaz.Objects;
using System;
using System.Collections.Generic;

namespace jaz.Logic
{
	public class Interpreter
	{
		private FileParser _fileParser;
		private InstructionParser _instructionParser;
		private string[] _data;
		private List<Instruction> _instructions;

		public Interpreter(string filename)
		{
			this._fileParser = new FileParser(filename);
			this._instructionParser = new InstructionParser();
		}

		public void Execute()
		{
			this._data = this._fileParser.ExecuteRead();
			this._instructions = this._instructionParser.Execute(ref this._data);

			foreach (var item in this._instructions)
			{
				Console.WriteLine(DetermineInstructionOperation(item.Command));
			}
		}

		private string DetermineInstructionOperation(string value)
		{
			switch (value)
			{
				default: return string.Empty;
				case InstructionSet.Addition: return InstructionSet.Addition;
				case InstructionSet.AND: return InstructionSet.AND;
				case InstructionSet.Begin: return InstructionSet.Begin;
				case InstructionSet.Call: return InstructionSet.Call;
				case InstructionSet.Copy: return InstructionSet.Copy;
				case InstructionSet.Division: return InstructionSet.Division;
				case InstructionSet.End: return InstructionSet.End;
				case InstructionSet.Equal: return InstructionSet.Equal;
				case InstructionSet.GoFalse: return InstructionSet.GoFalse;
				case InstructionSet.GoTo: return InstructionSet.GoTo;
				case InstructionSet.GoTrue: return InstructionSet.GoTrue;
				case InstructionSet.Greater: return InstructionSet.Greater;
				case InstructionSet.GreaterOrEqual: return InstructionSet.GreaterOrEqual;
				case InstructionSet.Halt: return InstructionSet.Halt;
				case InstructionSet.Label: return InstructionSet.Label;
				case InstructionSet.Lesser: return InstructionSet.Lesser;
				case InstructionSet.LesserOrEqual: return InstructionSet.LesserOrEqual;
				case InstructionSet.LValue: return InstructionSet.LValue;
				case InstructionSet.Multuplication: return InstructionSet.Multuplication;
				case InstructionSet.NOT: return InstructionSet.NOT;
				case InstructionSet.NotEqual: return InstructionSet.NotEqual;
				case InstructionSet.OR: return InstructionSet.OR;
				case InstructionSet.Pop: return InstructionSet.Pop;
				case InstructionSet.Print: return InstructionSet.Print;
				case InstructionSet.Push: return InstructionSet.Push;
				case InstructionSet.Remainder: return InstructionSet.Remainder;
				case InstructionSet.Return: return InstructionSet.Return;
				case InstructionSet.RValue: return InstructionSet.RValue;
				case InstructionSet.Show: return InstructionSet.Show;
				case InstructionSet.Subtraction: return InstructionSet.Subtraction;
				case InstructionSet.Top: return InstructionSet.Top;
			}
		}
	}
}