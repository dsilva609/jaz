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

		public InstructionSetHandler()
		{
			this._operationStack = new Stack();
			this._symbolTable = new Dictionary<string, object>();
		}

		public void DetermineAndExecuteInstructionOperation(Instruction item)
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
					this.Label();
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

		public virtual void Push(object item)
		{
			if (item.GetType() == typeof(Instruction))
			{
				Instruction temp = (Instruction)item;//maybe have a Instruction.Parse(object item) method
				this._operationStack.Push(temp.Value);
			}
			else
				this._operationStack.Push(item.ToString());//needs to get value of variable this is being set to if there is one
		}

		public virtual void RValue(object value)
		{
			//value = 0;
			this._operationStack.Push(0);//make sure these values are correct
			this._symbolTable.Add(value.ToString(), 0);//or should this be null?

			//this._symbolTable[value.Command] = value.Value;//probably not correct for now
		}

		public virtual void LValue(string address)
		{
			this._operationStack.Push(address);
			this._symbolTable.Add(address, null);
		}

		public virtual void Pop()
		{
			this._operationStack.Pop();
		}

		public virtual void ReplaceTop()
		{ throw new NotImplementedException(); }

		public virtual void Copy()
		{ throw new NotImplementedException(); }

		#endregion Stack Manipulation

		#region Control Flow

		public virtual void Label() //--need to figure out way to save methods, list of queues?
		{ throw new NotImplementedException(); }

		public virtual void GoTo()
		{ throw new NotImplementedException(); }

		public virtual void GoFalse()
		{ throw new NotImplementedException(); }

		public virtual void GoTrue()
		{ throw new NotImplementedException(); }

		public virtual void Halt()
		{
			Environment.Exit(0);//should this be a complete system exit?
		}

		#endregion Control Flow

		#region Arithmetic Operators

		public virtual void Addition()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 + operand1;

			this._operationStack.Push(result);
		}

		public virtual void Subtraction()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop().ToString());
			var operand2 = Convert.ToInt32(this._operationStack.Pop().ToString());

			var result = operand2 - operand1;

			this._operationStack.Push(result);
		}

		public virtual void Multuplication()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 * operand1;

			this._operationStack.Push(result);
		}

		public virtual void Division()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 / operand1;

			this._operationStack.Push(result);
		}

		public virtual void Remainder()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = operand2 % operand1;

			this._operationStack.Push(result);
		}

		#endregion Arithmetic Operators

		#region Logical Operators

		public virtual void AND()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 & operand1);

			this._operationStack.Push(result);
		}

		public virtual void NOT()
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

		public virtual void OR()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 | operand1);

			this._operationStack.Push(result);
		}

		#endregion Logical Operators

		#region Relational Operators

		public virtual void NotEqual()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 != operand1);

			this._operationStack.Push(result);
		}

		public virtual void LesserOrEqual()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 <= operand1);

			this._operationStack.Push(result);
		}

		public virtual void GreaterOrEqual()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 >= operand1);

			this._operationStack.Push(result);
		}

		public virtual void Lesser()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 < operand1);

			this._operationStack.Push(result);
		}

		public virtual void Greater()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 > operand1);

			this._operationStack.Push(result);
		}

		public virtual void Equal()
		{
			var operand1 = Convert.ToInt32(this._operationStack.Pop());
			var operand2 = Convert.ToInt32(this._operationStack.Pop());

			var result = Convert.ToInt32(operand2 == operand1);

			this._operationStack.Push(result);
		}

		#endregion Relational Operators

		#region Output

		public virtual void Print()
		{
			var value = this._operationStack.Peek();

			//var value = this._symbolTable[key.ToString()];
			Console.WriteLine(value);
		}

		public virtual void Show(object value)
		{
			Console.WriteLine(value);
		}

		#endregion Output

		#region Subprogram Control

		public virtual void Begin() //--run through a queue?
		{ throw new NotImplementedException(); }

		public virtual void End()
		{ throw new NotImplementedException(); }

		public virtual void Return()
		{ throw new NotImplementedException(); }

		public virtual void Call()
		{ throw new NotImplementedException(); }

		#endregion Subprogram Control
	}
}