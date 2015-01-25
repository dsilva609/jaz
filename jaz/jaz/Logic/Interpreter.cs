using jaz.Data;

namespace jaz.Logic
{
	public class Interpreter
	{
		private FileParser _fileParser;
		private InstructionParser _instructionParser;
		private string[] _instructions;

		public Interpreter(string filename)
		{
			this._fileParser = new FileParser(filename);
			this._instructionParser = new InstructionParser();
		}

		public void Execute()
		{
			this._instructions = this._fileParser.ExecuteRead();
			this._instructionParser.Execute(ref this._instructions);
		}
	}
}