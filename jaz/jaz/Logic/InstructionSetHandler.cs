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
		private bool _populatingFunction = false;
		private Queue<Instruction> _currentFunctionToBePopulated;

		public InstructionSetHandler()
		{
			this._operationStack = new Stack();
			this._symbolTable = new Dictionary<string, object>();
		}

		public void Run(List<Instruction> instructions)
		{
			foreach (var item in instructions)
			{
				if (!this._populatingFunction)
					this.DetermineAndExecuteInstructionOperation(item);
				else
					this.PopulateFunction(item);
			}
		}

		private void DetermineAndExecuteInstructionOperation(Instruction item)
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
					this.Begin();
					break;

				case InstructionSet.Call:
					this.Call();
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
					this.GoFalse();
					break;

				case InstructionSet.GoTo:
					this.GoTo();
					break;

				case InstructionSet.GoTrue:
					this.GoTrue();
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
					this.Return();
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
			this._operationStack.Push(0);//make sure these values are correct
			this._symbolTable.Add(value.ToString(), 0);//or should this be null?

			//this._symbolTable[value.Command] = value.Value;//probably not correct for now
		}

		private void LValue(string address)
		{
			this._operationStack.Push(address);
			this._symbolTable.Add(address, null);
		}

		private void Pop()
		{
			this._operationStack.Pop();
		}

		private void ReplaceTop()
		{ throw new NotImplementedException(); }

		private void Copy()
		{ throw new NotImplementedException(); }

		#endregion Stack Manipulation

		#region Control Flow

		private void Label(string functionName) //--need to figure out way to save methods, list of queues in the dictionary?
		{
			this._operationStack.Push(functionName);
			this._symbolTable.Add(functionName, new Queue<Instruction>());

			this._currentFunctionToBePopulated = (Queue<Instruction>)this._symbolTable[this._operationStack.Peek().ToString()];

			this._populatingFunction = true;
		}

		private void GoTo()
		{ throw new NotImplementedException(); }

		private void GoFalse()
		{ throw new NotImplementedException(); }

		private void GoTrue()
		{ throw new NotImplementedException(); }

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

		private void Begin() //--run through a queue?
		{ throw new NotImplementedException(); }

		private void End()
		{ throw new NotImplementedException(); }

		private void Return()
		{ throw new NotImplementedException(); }

		private void Call()
		{ throw new NotImplementedException(); }

		#endregion Subprogram Control

		#region Helpers

		private void PopulateFunction(Instruction instruction)
		{
			if (instruction.Value == InstructionSet.Return)
			{
				this._currentFunctionToBePopulated.Enqueue(instruction);
				this._symbolTable[this._operationStack.Peek().ToString()] = this._currentFunctionToBePopulated;
				this._currentFunctionToBePopulated.Clear();
				this._populatingFunction = false;
			}
			else
				this._currentFunctionToBePopulated.Enqueue(instruction);
		}

		#endregion Helpers
	}
}