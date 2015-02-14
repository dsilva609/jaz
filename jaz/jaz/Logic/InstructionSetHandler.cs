using jaz.Objects;
using System;
using System.Collections;
using System.Collections.Generic;

namespace jaz.Logic
{
	public class InstructionSetHandler
	{
		private Stack _operationStack;
		private Dictionary<string, object> _symbolTable;
		private List<Instruction> _instructionsToBeExecuted;
		private bool _populatingFunction = false;
		private List<Instruction> _currentFunctionToBePopulated;
		private bool _newVariablesAreLocal = false;

		public InstructionSetHandler()
		{
			this._operationStack = new Stack();
			this._symbolTable = new Dictionary<string, object>();
		}

		public void Run(List<Instruction> instructions)
		{
			this._instructionsToBeExecuted = instructions;

			this.IterateThrough(this._instructionsToBeExecuted, false);
		}

		private void ExecuteInstruction(Instruction item)
		{
			switch (item.Command)
			{
				default:
					break;

				case InstructionSet.Addition:
					this.Addition();
					break;

				case InstructionSet.AND:
					this.AND();
					break;

				case InstructionSet.Begin:
					this.Begin(item.GUID);
					break;

				case InstructionSet.Call:
					this.Call(item.Value, item.GUID);
					break;

				case InstructionSet.Copy:
					this.Copy();
					break;

				case InstructionSet.Division:
					this.Division();
					break;

				case InstructionSet.End:
					this.End();
					break;

				case InstructionSet.Equal:
					this.Equal();
					break;

				case InstructionSet.GoFalse:
					this.GoFalse(item.Value);
					break;

				case InstructionSet.GoTo:
					this.GoTo(item.Value);
					break;

				case InstructionSet.GoTrue:
					this.GoTrue(item.Value);
					break;

				case InstructionSet.Greater:
					this.Greater();
					break;

				case InstructionSet.GreaterOrEqual:
					this.GreaterOrEqual();
					break;

				case InstructionSet.Halt:
					this.Halt();
					break;

				case InstructionSet.Label:
					this.Label(item.Value);
					break;

				case InstructionSet.Lesser:
					this.Lesser();
					break;

				case InstructionSet.LesserOrEqual:
					this.LesserOrEqual();
					break;

				case InstructionSet.LValue:
					this.LValue(item.Value);
					break;

				case InstructionSet.Multuplication:
					this.Multuplication();
					break;

				case InstructionSet.NOT:
					this.NOT();
					break;

				case InstructionSet.NotEqual:
					this.NotEqual();
					break;

				case InstructionSet.OR:
					this.OR();
					break;

				case InstructionSet.Pop:
					this.Pop();
					break;

				case InstructionSet.Print:
					this.Print();
					break;

				case InstructionSet.Push:
					this.Push(item);
					break;

				case InstructionSet.Remainder:
					this.Remainder();
					break;

				case InstructionSet.Return:
					this.Return(item.GUID);
					break;

				case InstructionSet.RValue:
					this.RValue(item.Value);
					break;

				case InstructionSet.Show:
					this.Show(item.Value);
					break;

				case InstructionSet.Subtraction:
					this.Subtraction();
					break;

				case InstructionSet.ReplaceTop:
					this.ReplaceTop();
					break;
			}
		}

		#region Stack Manipulation

		private void Push(object item)
		{
			if (item.GetType() == typeof(Instruction))
			{
				Instruction temp = (Instruction)item;//maybe have a Instruction.Parse(object item) method
				this._operationStack.Push(temp.Value);
			}
			else
				this._operationStack.Push(item.ToString());//needs to get value of variable this is being set to if there is one
		}

		private void RValue(object value)
		{
			//value = 0;
			if (this._symbolTable.ContainsKey(value.ToString()))
			{
				this._operationStack.Push(this._symbolTable[value.ToString()]);
			}
			else
			{
				this._operationStack.Push(0);//make sure these values are correct
				this._symbolTable.Add(value.ToString(), 0);//or should this be null?
			}
			//this._symbolTable[value.Command] = value.Value;//probably not correct for now
		}

		private void LValue(string address)
		{
			this._operationStack.Push(address);
			if (!this._symbolTable.ContainsKey(address))
				this._symbolTable.Add(address, null);
		}

		private void Pop()
		{
			this._operationStack.Pop();
		}

		private void ReplaceTop()
		{
			var value = this._operationStack.Pop();
			var variable = this._operationStack.Pop();

			this._symbolTable[variable.ToString()] = value;

			this._operationStack.Push(value);
		}

		private void Copy()
		{
			this._operationStack.Push(this._operationStack.Peek());
		}

		#endregion Stack Manipulation

		#region Control Flow

		private void Label(string functionName) //--there are labels coupled to function calls and labels that are just pointers
		{
			//this._operationStack.Push(functionName);//is this necessary?
			if (!this._symbolTable.ContainsKey(functionName)) //what do if this already exists?
				this._symbolTable.Add(functionName, new List<Instruction>());//is this needed or can it be combined below?

			var coupledReturnValue = this._instructionsToBeExecuted.Find(x => x.Value == functionName && x.Command == InstructionSet.Return);

			if (coupledReturnValue != null)
			{
				this._currentFunctionToBePopulated = (List<Instruction>)this._symbolTable[this._operationStack.Peek().ToString()];//only if there is a coupled return of same guid

				this._populatingFunction = true;
			}
		}

		private void GoTo(string nextInstruction, bool fromReturn = false, Guid returnGUID = new Guid())//guid or just label?
		{
			int start = 0;
			if (fromReturn)
				//start = this._instructionsToBeExecuted.FindIndex(x => x.Value == nextInstruction && x.Command == InstructionSet.Call) + 1;
				start = this._instructionsToBeExecuted.FindIndex(x => x.GUID == returnGUID && x.Command == nextInstruction);
			else
				start = this._instructionsToBeExecuted.FindIndex(x => x.Value == nextInstruction && x.Command == InstructionSet.Label) + 1;
			int end = this._instructionsToBeExecuted.Count - 1;//make sure this number is not off by 1
			List<Instruction> instructions = this._instructionsToBeExecuted.GetRange(start, end - start + 1);

			this.IterateThrough(instructions, true);
		}

		private void GoFalse(string nextInstruction)//guid or just label?
		{
			int stackTop = Convert.ToInt32(this._operationStack.Pop());

			if (stackTop == 0)
				this.GoTo(nextInstruction, false);

			//throw new NotImplementedException();
		}

		private void GoTrue(string nextInstruction)//guid or just label?
		{
			int stackTop = Convert.ToInt32(this._operationStack.Pop());

			if (stackTop != 0)
				this.GoTo(nextInstruction);

			//throw new NotImplementedException();
		}

		private void Halt()
		{
			Environment.Exit(0);//should this be a complete system exit?
		}

		#endregion Control Flow

		#region Arithmetic Operators

		private void Addition()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 + operand1;

			this._operationStack.Push(result);
		}

		private void Subtraction()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop().ToString());
			var operand2 = Convert.ToInt32(this._operationStack.Pop().ToString());

			var result = operand2 - operand1;

			this._operationStack.Push(result);
		}

		private void Multuplication()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 * operand1;

			this._operationStack.Push(result);
		}

		private void Division()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 / operand1;

			this._operationStack.Push(result);
		}

		private void Remainder()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 % operand1;

			this._operationStack.Push(result);
		}

		#endregion Arithmetic Operators

		#region Logical Operators

		private void AND()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 & operand1);

			this._operationStack.Push(result);
		}

		private void NOT()
		{
			var operand = Convert.ToInt32(this._operationStack.Pop());
			int result = 0;

			if (operand == 0)
				result = 1;
			else if (operand == 1)
				result = 0;
			else
				result = operand * -1;

			this._operationStack.Push(Convert.ToInt32(result));
		}

		private void OR()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 | operand1);

			this._operationStack.Push(result);
		}

		#endregion Logical Operators

		#region Relational Operators

		private void NotEqual()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 != operand1);

			this._operationStack.Push(result);
		}

		private void LesserOrEqual()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 <= operand1);

			this._operationStack.Push(result);
		}

		private void GreaterOrEqual()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 >= operand1);

			this._operationStack.Push(result);
		}

		private void Lesser()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 < operand1);

			this._operationStack.Push(result);
		}

		private void Greater()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 > operand1);

			this._operationStack.Push(result);
		}

		private void Equal()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 == operand1);

			this._operationStack.Push(result);
		}

		#endregion Relational Operators

		#region Output

		private void Print()
		{
			var value = this._operationStack.Peek();

			//var value = this._symbolTable[key.ToString()];
			Console.WriteLine(value);
		}

		private void Show(object value)
		{
			Console.WriteLine(value);
		}

		#endregion Output

		#region Subprogram Control

		private void Begin(Guid guid) //--needs to handle multiple begin and end blocks
		{
			var tempInstructions = this._instructionsToBeExecuted;

			List<Instruction> subroutine;
			int beginning = tempInstructions.FindIndex(x => x.GUID == guid) + 1;
			int end = tempInstructions.FindIndex(y => y.Command == InstructionSet.End && y.GUID == guid) + 1;
			subroutine = tempInstructions.GetRange(beginning, end - beginning);//is this off by 1?

			this._newVariablesAreLocal = true;
			Instruction previousInstr = new Instruction();
			subroutine.ForEach(x =>
			{
				string origValue = x.Value;
				//x.Value = (x.Command == InstructionSet.RValue) ? "local::" + x.Value : x.Value = x.Value;
				//if (x.Command == InstructionSet.LValue && !this._symbolTable.ContainsKey("passed::" + origValue))
				//this._symbolTable.Add("passed::" + origValue, 0);
				if (x.Command == InstructionSet.RValue && previousInstr.Command == InstructionSet.LValue && !this._symbolTable.ContainsKey("passed::" + previousInstr.Value) && this._symbolTable.ContainsKey(origValue))
					this._symbolTable.Add("passed::" + previousInstr.Value, this._symbolTable[origValue]);
				//					this._symbolTable["passed::" + origValue] = this._symbolTable[origValue];//origvalue does not exist yet
				//else if (x.Command == InstructionSet.RValue && previousInstr.Command == InstructionSet.LValue && !this._symbolTable.ContainsKey("passed::" + previousInstr.Value))
				//this._symbolTable[previousInstr.Value] = this._symbolTable[origValue];
				//	this._symbolTable.Add("passed::" + previousInstr.Value, 0);
				//string value = x.Value;
				//if (!string.IsNullOrWhiteSpace(value) && value.Contains("local::"))
				//	this._symbolTable.Add(value, 0);
				previousInstr = x;
			});
			this.IterateThrough(subroutine, this._newVariablesAreLocal);
			//for (int i = tempInstructions.FindIndex(x => x.GUID == guid); i < tempInstructions.Count; i++)
			///	{
			//		if (tempInstructions.Find(x => x.))
			//	}
		}

		private void End()
		{
			this._newVariablesAreLocal = false;
		}

		private void Return(Guid returnToInstruction)//use guid to return to call index + 1
		{
			//need to make sure that it goes back to the correct instruction
			//this._operationStack.Pop();
			int returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == returnToInstruction) + 1;
			Instruction returnValue = this._instructionsToBeExecuted[returnIndex];//currently returns null
			this.GoTo(returnValue.Command, true, returnValue.GUID);
		}

		private void Call(string functionName, Guid functionGUID)
		{
			this._operationStack.Push(this._instructionsToBeExecuted[this._instructionsToBeExecuted.FindIndex(x => x.GUID == functionGUID) + 1].GUID);

			if (!this._symbolTable.ContainsKey(functionName))
			{
				this.SearchAndPopulateFunction(functionName, functionGUID);
			}
			//else
			this.IterateThrough((List<Instruction>)this._symbolTable[functionName], true);

			/*
			 * else
			 *		save all instructions until label functionName is found?
			 *		then populate function name
			 *		then execute function functionName
			 */
		}

		#endregion Subprogram Control

		#region Helpers

		private void IterateThrough(List<Instruction> instructions, bool newVariablesAreLocal)
		{
			foreach (var item in instructions)
			{
				var test = item.Command;//for testing
				var test2 = item.Value;
				if (!this._populatingFunction)
					this.ExecuteInstruction(item);
				else
					this.PopulateFunction(item);
			}
		}

		private void PopulateFunction(Instruction instruction)
		{
			if (instruction.Value == InstructionSet.Return)
			{
				this._currentFunctionToBePopulated.Add(instruction);
				this._symbolTable[this._operationStack.Peek().ToString()] = this._currentFunctionToBePopulated;
				this._currentFunctionToBePopulated.Clear();
				this._populatingFunction = false;
			}
			else
				this._currentFunctionToBePopulated.Add(instruction);
		}

		private void SearchAndPopulateFunction(string functionName, Guid functionGUID)//is the guid even used?
		{
			var tempInstructions = this._instructionsToBeExecuted;
			List<Instruction> function;
			int start = tempInstructions.FindIndex(x => x.Command == InstructionSet.Label && x.Value == functionName) + 1;
			int end = tempInstructions.FindIndex(y => y.Command == InstructionSet.Return && y.GUID == tempInstructions[start - 1].GUID);

			Instruction previousInstr = new Instruction();
			function = tempInstructions.GetRange(start, end - start + 1);
			function.ForEach(x =>
			{
				string origValue = x.Value;
				string currentCmd = x.Command;
				//x.Value = (x.Command == InstructionSet.LValue) ? "local::" + x.Value : x.Value;// = x.Value;
				//x.Value = x.Command == InstructionSet.RValue ? "passed::" + x.Value : x.Value;
				string value = x.Value;

				if (x.Command == InstructionSet.RValue && previousInstr.Command == InstructionSet.LValue && !this._symbolTable.ContainsKey("local::" + origValue))//local value needs to be set to the passed balue
				{
					this._symbolTable.Add("local::" + origValue, this._symbolTable["passed::" + origValue]);
				}
				previousInstr = x;
			});
			this._symbolTable.Add(functionName, function);
		}

		#endregion Helpers
	}
}