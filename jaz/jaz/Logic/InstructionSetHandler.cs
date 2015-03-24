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
		private List<Instruction> _currentFunctionToBePopulated;
		private int _numMainCallsRemaining;
		private int _recursiveCallsRemaining;
		private int _exitCode;
		private Queue<RecursionDatum> _recursionQueue;
		private RecursionDatum _currentRecursionDatum;
		private bool _currentlyInRecursion;

		public InstructionSetHandler()
		{
			this._operationStack = new Stack();
			this._symbolTable = new Dictionary<string, object>();
			this._numMainCallsRemaining = 0;
			this._recursionQueue = new Queue<RecursionDatum>();
			this._recursiveCallsRemaining = 0;
			this._exitCode = -1;
		}

		public int Run(List<Instruction> instructions = null)
		{
			if (this._exitCode != 0)
			{
				this._instructionsToBeExecuted = instructions;

				this.SetInitialRun(this._instructionsToBeExecuted);
			}

			return this._exitCode;
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
				Instruction temp = (Instruction)item;
				this._operationStack.Push(temp.Value);
			}
			else
				this._operationStack.Push(item.ToString());
		}

		private void RValue(object value)
		{
			if (this._symbolTable.ContainsKey(value.ToString()))
				this._operationStack.Push(this._symbolTable[value.ToString()]);
			else
			{
				this._operationStack.Push(0);
				this._symbolTable.Add(value.ToString(), 0);
			}
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

		private void Label(string functionName)
		{
			if (!this._symbolTable.ContainsKey(functionName))
				this._symbolTable.Add(functionName, new List<Instruction>());

			var coupledReturnValue = this._instructionsToBeExecuted.Find(x => x.Value == functionName && x.Command == InstructionSet.Return);

			if (coupledReturnValue != null)
				this._currentFunctionToBePopulated = (List<Instruction>)this._symbolTable[this._operationStack.Peek().ToString()];
		}

		private void GoTo(string nextInstruction, bool fromReturn = false, Guid returnGUID = new Guid())
		{
			int start = 0;
			if (fromReturn)
				start = this._instructionsToBeExecuted.FindIndex(x => x.GUID == returnGUID && x.Command == nextInstruction);
			else
				start = this._instructionsToBeExecuted.FindIndex(x => x.Value == nextInstruction && x.Command == InstructionSet.Label) + 1;
			int end = this._instructionsToBeExecuted.Count - 1;
			List<Instruction> instructions = this._instructionsToBeExecuted.GetRange(start, end - start + 1);

			if (this._exitCode != 0)
			{
				this.IterateThrough(instructions);

				if (this._currentRecursionDatum != null)
					this.SaveVariableStates(InstructionSet.GoTo);
			}
		}

		private void GoFalse(string nextInstruction)
		{
			int stackTop = Convert.ToInt32(this._operationStack.Pop());

			if (stackTop == 0)
				this.GoTo(nextInstruction, false);
		}

		private void GoTrue(string nextInstruction)
		{
			int stackTop = Convert.ToInt32(this._operationStack.Pop());

			if (stackTop != 0)
				this.GoTo(nextInstruction);
		}

		private void Halt()
		{
			this._exitCode = 0;
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

			Console.WriteLine(value);
		}

		private void Show(object value)
		{
			Console.WriteLine(value);
		}

		#endregion Output

		#region Subprogram Control

		private void Begin(Guid guid)
		{
			var tempInstructions = this._instructionsToBeExecuted;

			List<Instruction> subroutine;
			int beginning = tempInstructions.FindIndex(x => x.GUID == guid) + 1;
			int end = tempInstructions.FindIndex(y => y.Command == InstructionSet.End && y.GUID == guid) + 1;
			subroutine = tempInstructions.GetRange(beginning, end - beginning);

			if (this._numMainCallsRemaining == 0)
				subroutine = AssignScope(subroutine);

			if (this._currentRecursionDatum != null)
				this.SaveVariableStates(InstructionSet.Begin);

			this.IterateThrough(subroutine);
		}

		private void End()
		{
			if (this._numMainCallsRemaining > 0)
				this._numMainCallsRemaining--;
		}

		private void Return(Guid returnToInstruction)
		{
			int returnIndex = -1;

			if (this._recursiveCallsRemaining == 0 && this._recursionQueue.Count != 0)
				this._recursionQueue.Dequeue();

			if (this._recursionQueue.Count != 0)
			{
				RecursionDatum currentRecursionDatum = this._recursionQueue.Peek();
				returnToInstruction = currentRecursionDatum.RecursiveReturnValue;
				this._recursiveCallsRemaining--;

				returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.GUID == returnToInstruction);

				if (this._currentRecursionDatum != null && this._currentRecursionDatum.RecursiveValueStorage.Count > 0)
				{
					foreach (var item in this._currentRecursionDatum.RecursiveValueStorage)
						this._symbolTable[item.Key] = item.Value.Pop();
				}
			}

			if (this._recursionQueue.Count == 0 && this._currentlyInRecursion)
			{
				returnToInstruction = this._currentRecursionDatum.OriginalReturnValue;
				returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.GUID == returnToInstruction);
				this._currentlyInRecursion = false;
			}
			if (this._currentRecursionDatum == null)
				returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == returnToInstruction) + 1;

			Instruction returnValue = this._instructionsToBeExecuted[returnIndex];
			this.GoTo(returnValue.Command, true, returnValue.GUID);
		}

		private void Call(string functionName, Guid functionGUID)
		{
			/* should be able to return if called function is recursive since the return location will be within the function itself
			 * `need to make sure that the return at the end returns to the main method when all the recursive calls are completed
			 *	maybe parse the call and label to make sure what guid goes to what location so that the execution is run correctly
			 *		there is a label (function)
			 *			inside is a begin
			 *				inside is a call to the label (function) again
			 *				return should be to the label
			 *				save the state of the variable
			 *					create a unique variable
			 *						guid?
			 *						separate`tack that pops top after each recursive call?
			 *		should return to the originating call, most likely main
			 */

			if (!this._symbolTable.ContainsKey(functionName))
				this.SearchAndPopulateFunction(functionName, functionGUID, true);
			else
			{
				if (this._recursionQueue.Count != 0)
				{
					if (functionGUID == this._currentRecursionDatum.RecursionID)
						this._recursiveCallsRemaining++;
				}
			}

			this.IterateThrough((List<Instruction>)this._symbolTable[functionName]);

			/*
			 * else
			 *		save all instructions until label functionName is found?
			 *		then populate function name
			 *		then execute function functionName
			 */
		}

		#endregion Subprogram Control

		#region Helpers

		private void SetInitialRun(List<Instruction> instructions)
		{
			List<Instruction> mainInstructions = new List<Instruction>();
			bool nextInstructionFound = true;
			bool mainIsPopulated = false;
			int index = 0;
			Queue<string> searchTerms = new Queue<string>();
			Queue<int> returnIndices = new Queue<int>();
			string currentSearchTerm = string.Empty;
			int returnLabelsNotFound = 0;
			bool isInBeginBlock = false;
			bool labelHasEnded = false;

			while (!mainIsPopulated && index < instructions.Count)
			{
				if (instructions[index].Command == InstructionSet.GoTo)
				{
					nextInstructionFound = false;
					mainInstructions.Add(instructions[index]);
					searchTerms.Enqueue(instructions[index].Value);
					currentSearchTerm = searchTerms.Dequeue();
					returnIndices.Enqueue(index + 1);
				}

				if (!nextInstructionFound && (instructions[index].Command == InstructionSet.Label && instructions[index].Value == currentSearchTerm))
				{
					nextInstructionFound = true;
					returnLabelsNotFound++;
				}

				if (!labelHasEnded && (instructions[index].Command == InstructionSet.Return || instructions[index].Command == InstructionSet.GoTo) && returnLabelsNotFound > 0)
				{
					labelHasEnded = true;
					returnLabelsNotFound--;
					mainInstructions.Add(instructions[index]);
					index = returnIndices.Dequeue();
					if (searchTerms.Count > 0)
						currentSearchTerm = searchTerms.Dequeue();
					continue;
				}

				if (nextInstructionFound && instructions[index].Command == InstructionSet.Begin)
				{
					isInBeginBlock = true;
					this._numMainCallsRemaining++;
				}

				if (nextInstructionFound && instructions[index].Command == InstructionSet.End)
					isInBeginBlock = false;

				if (nextInstructionFound && instructions[index].Command == InstructionSet.Halt)
				{
					mainIsPopulated = true;
					mainInstructions.Add(instructions[index]);
					break;
				}

				if (nextInstructionFound)
					mainInstructions.Add(instructions[index]);

				index++;
			}

			this.AssignScope(mainInstructions, "main");
			this.IterateThrough(mainInstructions);
		}

		private void IterateThrough(List<Instruction> instructions)
		{
			foreach (var item in instructions)
			{
				if (this._exitCode != 0)
					this.ExecuteInstruction(item);
				else
				{
					this.Run();
					break;
				}
			}
		}

		private void SearchAndPopulateFunction(string functionName, Guid functionGUID, bool populateCall = false)
		{
			var tempInstructions = this._instructionsToBeExecuted;
			List<Instruction> function;
			int start = tempInstructions.FindIndex(x => x.Command == InstructionSet.Label && x.Value == functionName) + 1;
			int end = tempInstructions.FindIndex(y => y.Command == InstructionSet.Return && y.GUID == tempInstructions[start - 1].GUID);

			Instruction previousInstr = new Instruction();
			function = tempInstructions.GetRange(start, end - start + 1);

			function = AssignScope(function, functionName);

			this.DetermineIfRecursionExists(function, functionName, functionGUID);
			/* if in label
			 *	if find begin
			 *		iterate to find call
			 *		save call name
			 *		lvalues in begin are callName::variableName
			 *		rvalues in begin are labelName::variableName
			 *	if not in begin
			 *		lvalue is labelName::variableName
			 *		rvalue is labelName::variableName
			 *
			 * have function to take in instructions and insert scope
			 *	AssignScope(instructions, labelName = string.Empty) --optional labelName if begin/end block is within a label, otherwise this is always determining variable passing scope
			 */

			this._symbolTable.Add(functionName, function);
		}

		private List<Instruction> AssignScope(List<Instruction> instructions, string labelName = "")
		{
			List<Instruction> tempInstructions = new List<Instruction>();
			string callName = string.Empty;
			/* if in label
			 *  if find gofalse || gotrue
			 *		save label
			 *		find label
			 *			if find lvalue
			 *				scope is originalLabel::variableName
			 *			if find rvalue
			 *				scope is go value::variableName
			 *	if find begin
			 *		iterate to find call
			 *		save call name
			 *		lvalues in begin are callName::variableName
			 *		rvalues in begin are labelName::variableName
			 *	if not in begin
			 *		lvalue is labelName::variableName
			 *		rvalue is labelName::variableName
			 *
			 * have function to take in instructions and insert scope
			 *	AssignScope(instructions, labelName = string.Empty) --optional labelName if begin/end block is within a label, otherwise this is always determining variable passing scope
			 */
			Queue<string> callQueue = new Queue<string>();
			foreach (var item in instructions)
			{
				if (item.Command == InstructionSet.Call)
					callQueue.Enqueue(item.Value);
			}

			if (callQueue.Count > 0)
				callName = callQueue.Dequeue();

			bool withinBegin = false;
			bool afterCall = false;
			foreach (var item in instructions)
			{
				if (item.Command == InstructionSet.GoFalse || item.Command == InstructionSet.GoTrue)
				{
					int goStart = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Label && x.Value == item.Value);
					while (goStart < this._instructionsToBeExecuted.Count && (this._instructionsToBeExecuted[goStart].Command != InstructionSet.Return || this._instructionsToBeExecuted[goStart].Command != InstructionSet.Halt))
					{
						if (this._instructionsToBeExecuted[goStart].Command == InstructionSet.LValue)
							this._instructionsToBeExecuted[goStart].Value = labelName + "::" + this._instructionsToBeExecuted[goStart].Value;

						goStart++;
					}
				}

				if (item.Command == InstructionSet.Begin)
					withinBegin = true;

				if (withinBegin)
				{
					if (item.Command == InstructionSet.Call)
						afterCall = true;

					if (!string.IsNullOrWhiteSpace(labelName))
					{
						if (item.Command == InstructionSet.LValue && !afterCall)
							item.Value = callName + "::" + item.Value;

						if (item.Command == InstructionSet.LValue && afterCall)
							item.Value = labelName + "::" + item.Value;

						if (item.Command == InstructionSet.RValue && !afterCall)
							item.Value = labelName + "::" + item.Value;

						if (item.Command == InstructionSet.RValue && afterCall)
							item.Value = callName + "::" + item.Value;
					}
					else
					{
						if (item.Command == InstructionSet.LValue)
							item.Value = callName + "::" + item.Value;

						if (item.Command == InstructionSet.RValue)
							item.Value = "::" + item.Value;
					}
				}
				else
				{
					if ((item.Command == InstructionSet.LValue || item.Command == InstructionSet.RValue) && !string.IsNullOrWhiteSpace(labelName))
						item.Value = labelName + "::" + item.Value;
				}

				if (item.Command == InstructionSet.End)
				{
					withinBegin = false;
					afterCall = false;
					if (callQueue.Count > 0)
						callName = callQueue.Dequeue();
				}

				tempInstructions.Add(item);
			}

			return tempInstructions;
		}

		private void DetermineIfRecursionExists(List<Instruction> function, string functionName, Guid functionGUID)
		{
			bool withinBegin = false;

			foreach (var item in function)
			{
				if (item.Command == InstructionSet.Begin)
					withinBegin = true;

				if (withinBegin)
				{
					if (item.Command == InstructionSet.Call && item.Value == functionName)
					{
						this._currentlyInRecursion = true;

						int recursiveReturnIndex = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == item.GUID) + 1;
						Instruction recursiveReturnValue = this._instructionsToBeExecuted[recursiveReturnIndex];

						int returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == functionGUID) + 1;
						Instruction returnValue = this._instructionsToBeExecuted[returnIndex];

						this._recursionQueue.Enqueue(new RecursionDatum { RecursionID = this._instructionsToBeExecuted[recursiveReturnIndex - 1].GUID, RecursionFunctionName = functionName, RecursiveReturnValue = recursiveReturnValue.GUID, OriginalReturnValue = returnValue.GUID });

						this._currentRecursionDatum = this._recursionQueue.Peek();
					}

					if (item.Command == InstructionSet.End)
						withinBegin = false;
				}
			}
		}

		private void SaveVariableStates(string fromInstruction)
		{
			List<Instruction> function = (List<Instruction>)this._symbolTable[this._currentRecursionDatum.RecursionFunctionName];

			bool withinBegin = false;
			bool afterCall = false;
			foreach (var instr in function)
			{
				if (fromInstruction == InstructionSet.Begin)
				{
					if (instr.Command == InstructionSet.RValue)
					{
						if (!this._currentRecursionDatum.RecursiveValueStorage.ContainsKey(instr.Value))
							this._currentRecursionDatum.RecursiveValueStorage.Add(instr.Value, new Stack<int>());

						this._currentRecursionDatum.RecursiveValueStorage[instr.Value].Push(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));
					}
					if (instr.Command == InstructionSet.Begin)
						break;
				}
				else if (fromInstruction == InstructionSet.GoTo)
				{
					if (instr.Command == InstructionSet.Begin)
						withinBegin = true;

					if (instr.Command == InstructionSet.Call)
						afterCall = true;

					if (withinBegin && afterCall && instr.Command == InstructionSet.RValue)
					{
						if (!this._currentRecursionDatum.RecursiveValueStorage.ContainsKey(instr.Value))
							this._currentRecursionDatum.RecursiveValueStorage.Add(instr.Value, new Stack<int>());

						this._currentRecursionDatum.RecursiveValueStorage[instr.Value].Push(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));
					}
					if (withinBegin && instr.Command == InstructionSet.End)
						withinBegin = false;

					if (afterCall && !withinBegin && instr.Command == InstructionSet.LValue)
						this._currentRecursionDatum.RecursiveValueStorage[instr.Value].Push(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));
				}
			}
		}

		#endregion Helpers
	}
}