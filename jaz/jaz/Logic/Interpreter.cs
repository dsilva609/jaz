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
		private InstructionSetHandler _instructionSetHandler;
		private string[] _data;
		private List<Instruction> _instructions;
		private List<string> _executionFiles;

		public Interpreter()
		{
			this._executionFiles = new List<string>
			{
				"demo.jaz",
				"factProc.jaz",
				"foo.jaz",
				"guessTheAnswer.jaz",
				"operatorsTest.jaz",
				"recFact.jaz"
			};

			//--add test file selector
		}

		public void Execute()
		{
			string input;
			int value = 0;
			bool parsed;
			while (value != -1)
			{
				Console.WriteLine("Please enter the number of the file you wish to run or -1 to exit:");
				for (int i = 0; i < this._executionFiles.Count; i++)
				{
					Console.WriteLine((i + 1) + ": " + this._executionFiles[i]);
				}

				input = Console.ReadLine();

				parsed = Int32.TryParse(input, out value);

				if (parsed && value > 0 && value <= this._executionFiles.Count)
				{
					Console.WriteLine("Running: " + this._executionFiles[value - 1]);

					this._fileParser = new FileParser(@"Resources\" + this._executionFiles[value - 1]);
					this._data = this._fileParser.ExecuteRead();
					this._instructionParser = new InstructionParser();
					this._instructions = this._instructionParser.Execute(ref this._data);
					this._instructionSetHandler = new InstructionSetHandler();
					this._instructionSetHandler.Run(this._instructions);

					this._instructionSetHandler = null;
					this._data = null;
					this._instructions.Clear();
					this._fileParser = null;
					this._instructionParser = null;
				}
				else if (parsed && value == -1)
				{
					Console.WriteLine("Exiting...");
					Environment.Exit(0);
				}
				else
				{
					Console.WriteLine("Sorry, but the input was invalid. Try again.");
				}
			}
		}
	}
}