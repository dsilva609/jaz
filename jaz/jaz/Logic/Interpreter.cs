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
				this._instructionSetHandler.DetermineAndExecuteInstructionOperation(item);
			}
		}
	}
}