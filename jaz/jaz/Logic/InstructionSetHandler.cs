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
		private bool _populatingFunction = false;//really needed?
		private List<Instruction> _currentFunctionToBePopulated;//is this used?
		private bool _newVariablesAreLocal = false;//really needed?

		private int _numMainCallsRemaining;
		private int _recursiveCallsRemaining;
		private Queue<RecursionDatum> _recursionQueue;
		private RecursionDatum _currentRecursionDatum;

		public InstructionSetHandler()
		{
			this._operationStack = new Stack();
			this._symbolTable = new Dictionary<string, object>();
			this._numMainCallsRemaining = 0;
			this._recursionQueue = new Queue<RecursionDatum>();
			this._recursiveCallsRemaining = 0;
		}

		public void Run(List<Instruction> instructions)
		{
			this._instructionsToBeExecuted = instructions;

			this.SetInitialRun(this._instructionsToBeExecuted);//////////

			//	this.IterateThrough(this._instructionsToBeExecuted, false);
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

			if (this._currentRecursionDatum != null)
				this.SaveVariableStates(InstructionSet.End);
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

			if (this._numMainCallsRemaining == 0)
				subroutine = AssignScope(subroutine);

			if (this._currentRecursionDatum != null)//needs to be refactored
			{
				this.SaveVariableStates(InstructionSet.Begin);
			}
			//--------------------------------------------------------------------------------------old hacky logic
			//	this._newVariablesAreLocal = true;
			//	Instruction previousInstr = new Instruction();
			//subroutine.ForEach(x =>
			//{
			//	string origValue = x.Value;

			//	if (x.Command == InstructionSet.RValue && previousInstr.Command == InstructionSet.LValue && !this._symbolTable.ContainsKey("passed::" + previousInstr.Value) && this._symbolTable.ContainsKey(origValue))
			//	{
			//		this._symbolTable.Add("passed::" + previousInstr.Value, this._symbolTable[origValue]);

			//		//x.Value = "local::" + x.Value;
			//		this._symbolTable.Add("local::" + previousInstr.Value, 0);
			//		//x.Value = "local::" + x.Value;
			//	}

			//	if (x.Command == InstructionSet.RValue && this._symbolTable.ContainsKey("passed::" + previousInstr.Value) && !this._symbolTable.ContainsKey("local::" + previousInstr.Value)) //is there is a passed value then a local variable of that name and value of 0 needs to be added to dictionary
			//	{
			//		this._symbolTable.Add("local::" + previousInstr.Value, 0);
			//		x.Value = "local::" + x.Value;
			//	}

			//	previousInstr = x;
			//});
			this.IterateThrough(subroutine, this._newVariablesAreLocal);
		}

		private void End()
		{
			this._newVariablesAreLocal = false;

			if (this._numMainCallsRemaining > 0)
				this._numMainCallsRemaining--;

			Console.WriteLine("CALLS REMAINING: " + this._numMainCallsRemaining);
		}

		private void Return(Guid returnToInstruction)//use guid to return to call index + 1
		{
			//need to make sure that it goes back to the correct instruction
			//this._operationStack.Pop();/////////////////////////////////////////////probably needed to keep stack clean
			int returnIndex = -1;

			if (this._recursiveCallsRemaining == 0 && this._recursionQueue.Count != 0)
				this._recursionQueue.Dequeue();

			if (this._recursionQueue.Count != 0)//maybe if recursion calls remaining is 0?
			{
				Console.WriteLine("RETURNING FROM RECURSION");
				RecursionDatum currentRecursionDatum = this._recursionQueue.Peek();
				returnToInstruction = currentRecursionDatum.RecursiveReturnValue;
				this._recursiveCallsRemaining--;

				returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.GUID == returnToInstruction);//can be removed?

				if (this._currentRecursionDatum != null && this._currentRecursionDatum.RecursiveValueStorage.Count > 0)
				{
					foreach (var item in this._currentRecursionDatum.RecursiveValueStorage)
					{
						this._symbolTable[item.Key] = item.Value.Pop();
					}
				}
				//List<Instruction> function = (List<Instruction>)this._symbolTable[functionName];

				//foreach (var instr in function) //this should probably be done during runtime so that the values actually exist
				//{
				//	if (instr.Command == InstructionSet.RValue)
				//	{
				//		if (!this._currentRecursionDatum.RecursiveValueStorage.ContainsKey(instr.Value))
				//		{
				//			this._currentRecursionDatum.RecursiveValueStorage.Add(instr.Value, new Stack<int>());
				//		}
				//		this._currentRecursionDatum.RecursiveValueStorage[instr.Value].Push(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));
				//	}
				//	if (instr.Command == InstructionSet.Begin)
				//		break;
				//}

				//need to decrement the recursion call count and pop the recursion value stack
			}

			if (this._recursionQueue.Count == 0)
			{
				returnToInstruction = this._currentRecursionDatum.OriginalReturnValue;
				returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.GUID == returnToInstruction);
				//this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == returnToInstruction) + 1;
			}
			if (this._currentRecursionDatum == null)/////////////////////////////////need to handle base case
				returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == returnToInstruction) + 1;
			Instruction returnValue = this._instructionsToBeExecuted[returnIndex];//currently returns null
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
			 *						separate stack that pops top after each recursive call?
			 *		should return to the originating call, most likely main
			 */

			//this._operationStack.Push(this._instructionsToBeExecuted[this._instructionsToBeExecuted.FindIndex(x => x.GUID == functionGUID) + 1].GUID);

			if (!this._symbolTable.ContainsKey(functionName))
			{
				this.SearchAndPopulateFunction(functionName, functionGUID, true);

				//count number of recursive calls remaining so that the return returns to the inner recursive call, when remaining recursive calls reaches 0, return value is set back to originating caller

				//string returnValue = DetermineReturnLocation(functionName, functionGUID);//move

				//				this._operationStack.Push(returnValue);/////////////////////////////////////////////////////move
			}
			//else
			else //--function should already exist for recursion to occur
			{
				if (this._recursionQueue.Count != 0)
				{
					if (functionGUID == this._currentRecursionDatum.RecursionID)
					{
						this._recursiveCallsRemaining++;
					}
				}
			}

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

		private void SetInitialRun(List<Instruction> instructions)
		{
			List<Instruction> mainInstructions = new List<Instruction>();
			bool nextInstructionFound = true;
			bool mainIsPopulated = false;
			int index = 0;
			Queue<string> searchTerms = new Queue<string>();
			Queue<int> returnIndices = new Queue<int>();
			string currentSearchTerm = string.Empty;
			//int returnIndex = 0;
			//bool labelReturnFound = true;
			int returnLabelsNotFound = 0;
			bool isInBeginBlock = false;//--marked for removal
			bool labelHasEnded = false;

			//if (searchTerms.Count > 0)
			//	currentSearchTerm = searchTerms.Dequeue();

			while (!mainIsPopulated && index < instructions.Count)
			{
				//Console.WriteLine(instructions[index].Command);

				if (instructions[index].Command == InstructionSet.GoTo)
				{
					nextInstructionFound = false;
					mainInstructions.Add(instructions[index]);
					searchTerms.Enqueue(instructions[index].Value);
					currentSearchTerm = searchTerms.Dequeue();
					//if (index + 1 <= instructions.Count - 1)
					returnIndices.Enqueue(index + 1);
					//returnIndex = index + 1;
					//else
					//	returnIndex = instructions.Count - 1;
					Console.WriteLine("found go to: " + instructions[index].Value);
				}

				if (!nextInstructionFound && (instructions[index].Command == InstructionSet.Label && instructions[index].Value == currentSearchTerm))
				{
					nextInstructionFound = true;
					//labelReturnFound = false;
					returnLabelsNotFound++;//---------------not all labels have return statements, goto label usually has a goto at the end again, not always

					//	mainInstructions.Add(instructions[index]);
					Console.WriteLine("found go to label");
				}

				if (!labelHasEnded && (instructions[index].Command == InstructionSet.Return || instructions[index].Command == InstructionSet.GoTo) && returnLabelsNotFound > 0)
				{
					//labelReturnFound = true;
					labelHasEnded = true;
					returnLabelsNotFound--;
					Console.WriteLine("RETURN LABELS NOT FOUND: " + returnLabelsNotFound);
					mainInstructions.Add(instructions[index]);
					index = returnIndices.Dequeue();
					Console.WriteLine("INDEX IS NOW: " + index);
					if (searchTerms.Count > 0)
						currentSearchTerm = searchTerms.Dequeue();
					Console.WriteLine("found label return");
					continue;
				}

				if (nextInstructionFound && instructions[index].Command == InstructionSet.Begin)
				{
					isInBeginBlock = true;
					this._numMainCallsRemaining++;
					Console.WriteLine("INCREMENTED");
				}

				if (nextInstructionFound && instructions[index].Command == InstructionSet.End)
				{
					isInBeginBlock = false;
				}

				//if (nextInstructionFound && (instructions[index].Command == InstructionSet.LValue || instructions[index].Command == InstructionSet.RValue) && !isInBeginBlock)
				//{
				//	instructions[index].Value = "main::" + instructions[index].Value;
				//}
				//else if (nextInstructionFound && instructions[index].Command == InstructionSet.LValue && isInBeginBlock)
				//{
				//	instructions[index].Value = "passed::" + instructions[index].Value;
				//}

				//if (nextInstructionFound && instructions[index].Command == InstructionSet.RValue && isInBeginBlock)
				//{
				//	instructions[index].Value = "main::" + instructions[index].Value;
				//}

				if (nextInstructionFound && instructions[index].Command == InstructionSet.Halt)
				{
					//	mainInstructions.Add(instructions[index]);
					mainIsPopulated = true;
					mainInstructions.Add(instructions[index]);
					//nextInstructionFound = true;
					Console.WriteLine("MAIN IS POPULATED");//this should probably just break here
					break;
				}

				if (nextInstructionFound)
				{
					mainInstructions.Add(instructions[index]);
					Console.WriteLine("added: " + instructions[index].Command + " " + instructions[index].Value);
					//index++;
					//continue;
				}
				index++;
			}

			this.AssignScope(mainInstructions, "main");
			this.IterateThrough(mainInstructions, false);
		}

		private void IterateThrough(List<Instruction> instructions, bool newVariablesAreLocal)
		{
			foreach (var item in instructions)
			{
				var test = item.Command;//for testing
				var test2 = item.Value;
				if (!this._populatingFunction)/////most likely redundant when populateFunction is removed
					this.ExecuteInstruction(item);
				else
					this.PopulateFunction(item);
			}
		}

		private void PopulateFunction(Instruction instruction)/////////////probably not needed
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

		private void SearchAndPopulateFunction(string functionName, Guid functionGUID, bool populateCall = false)//is the guid even used?
		{
			var tempInstructions = this._instructionsToBeExecuted;
			List<Instruction> function;
			int start = tempInstructions.FindIndex(x => x.Command == InstructionSet.Label && x.Value == functionName) + 1;
			int end = tempInstructions.FindIndex(y => y.Command == InstructionSet.Return && y.GUID == tempInstructions[start - 1].GUID);

			Instruction previousInstr = new Instruction();//is this used anymore?
			function = tempInstructions.GetRange(start, end - start + 1);

			//string returnVal;
			//if (populateCall)//-----------------what was this bool supposed to be?
			//returnVal =

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

			//-----------------------------------------------------------------------------------------------------old hacky logic
			// function.ForEach(x =>
			//{
			//	string origValue = x.Value;
			//	string currentCmd = x.Command;

			//	if (x.Command == InstructionSet.RValue && !this._symbolTable.ContainsKey("local::" + x.Value))
			//	{
			//		this._symbolTable.Add("local::" + x.Value, 0);
			//		// Console.WriteLine("rvalue is: " + x.Value);
			//		x.Value = "local::" + x.Value;
			//	}

			//	if (x.Command == InstructionSet.RValue && previousInstr.Command == InstructionSet.LValue && !this._symbolTable.ContainsKey("local::" + origValue))//local value needs to be set to the passed balue
			//	{
			//		this._symbolTable.Add("local::" + origValue, this._symbolTable["passed::" + origValue]);

			//	}
			//	previousInstr = x;
			//});
			this._symbolTable.Add(functionName, function);
		}

		private List<Instruction> AssignScope(List<Instruction> instructions, string labelName = "")////can labelName be optional? or should this be a in begin only bool?
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
				{
					callQueue.Enqueue(item.Value);
				}
			}

			if (callQueue.Count > 0)
				callName = callQueue.Dequeue();

			bool withinBegin = false;
			bool afterCall = false;
			foreach (var item in instructions)
			{
				if (item.Command == InstructionSet.GoFalse || item.Command == InstructionSet.GoTrue)
				{
					Console.WriteLine("FOUND A GO FUNCTION");

					int goStart = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Label && x.Value == item.Value);
					while (this._instructionsToBeExecuted[goStart].Command != InstructionSet.Return)
					{
						if (this._instructionsToBeExecuted[goStart].Command == InstructionSet.LValue)
						{
							this._instructionsToBeExecuted[goStart].Value = labelName + "::" + this._instructionsToBeExecuted[goStart].Value;
						}
						goStart++;
					}
				}

				if (item.Command == InstructionSet.Begin)
					withinBegin = true;

				if (withinBegin)
				{
					if (item.Command == InstructionSet.Call)
					{
						//if (item.Value == labelName)
						//	Console.WriteLine("RECURSIVE CALL FOUND");

						afterCall = true;
						Console.WriteLine("AFTER CALL");
					}

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

						if (item.Command == InstructionSet.RValue)//necessary?
							item.Value = "::" + item.Value;
					}
				}
				else
				{
					//if ((item.Command == InstructionSet.LValue || item.Command == InstructionSet.RValue) && string.IsNullOrWhiteSpace(labelName))
					//	item.Value = "::" + item.Value;//check is this is correct

					if ((item.Command == InstructionSet.LValue || item.Command == InstructionSet.RValue) && !string.IsNullOrWhiteSpace(labelName))
						item.Value = labelName + "::" + item.Value;//check if this is correct
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

			Console.WriteLine("instructions for: " + labelName);
			//function.ForEach(x =>
			//{
			//	if (x.Command == InstructionSet.LValue || x.Command == InstructionSet.RValue)
			//		x.Value = functionName + "::" + x.Value;
			//});

			foreach (var item in tempInstructions)
				Console.WriteLine("instruction: " + item.Command + " " + item.Value);

			return tempInstructions;
		}

		private void DetermineIfRecursionExists(List<Instruction> function, string functionName, Guid functionGUID)//probably can be moved to assign scope function
		{
			bool functionIsRecursive;  ///really needed?
			bool withinBegin = false;
			//int numberRecursiveCalls = 0;//count number of recursive calls remaining so that the return returns to the inner recursive call, when remaining recursive calls reaches 0, return value is set back to originating caller

			foreach (var item in function)
			{
				if (item.Command == InstructionSet.Begin)
					withinBegin = true;

				if (withinBegin)
				{
					if (item.Command == InstructionSet.Call && item.Value == functionName)
					{
						Console.WriteLine("RECURSION");
						functionIsRecursive = true;

						int recursiveReturnIndex = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == item.GUID) + 1;//logic is wrong, should get index from global instruction list of recursive call name and guid -- fixed
						Instruction recursiveReturnValue = this._instructionsToBeExecuted[recursiveReturnIndex];

						int returnIndex = this._instructionsToBeExecuted.FindIndex(x => x.Command == InstructionSet.Call && x.GUID == functionGUID) + 1;
						Instruction returnValue = this._instructionsToBeExecuted[returnIndex];

						this._recursionQueue.Enqueue(new RecursionDatum { RecursionID = this._instructionsToBeExecuted[recursiveReturnIndex - 1].GUID, RecursionFunctionName = functionName, RecursiveReturnValue = recursiveReturnValue.GUID, OriginalReturnValue = returnValue.GUID });

						this._currentRecursionDatum = this._recursionQueue.Peek();//////////how should I go about this?
					}

					if (item.Command == InstructionSet.End)
						withinBegin = false;
				}
			}

			//return string.Empty;
		}

		private void SaveVariableStates(string fromInstruction)
		{
			List<Instruction> function = (List<Instruction>)this._symbolTable[this._currentRecursionDatum.RecursionFunctionName];

			bool withinBegin = false;
			bool afterCall = false;
			foreach (var instr in function) //this should probably be done during runtime so that the values actually exist
			{
				if (fromInstruction == InstructionSet.Begin)
				{
					if (instr.Command == InstructionSet.RValue)
					{
						if (!this._currentRecursionDatum.RecursiveValueStorage.ContainsKey(instr.Value))
						{
							this._currentRecursionDatum.RecursiveValueStorage.Add(instr.Value, new Stack<int>());
						}

						Console.WriteLine("---------------------------" + instr.Value);
						Console.WriteLine(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));
						this._currentRecursionDatum.RecursiveValueStorage[instr.Value].Push(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));
					}
					if (instr.Command == InstructionSet.Begin)
						break;
				}
				else if (fromInstruction == InstructionSet.End)
				{
					if (instr.Command == InstructionSet.Begin)
						withinBegin = true;
					if (instr.Command == InstructionSet.Call)
						afterCall = true;
					if (withinBegin && afterCall && instr.Command == InstructionSet.RValue)
					{
						if (!this._currentRecursionDatum.RecursiveValueStorage.ContainsKey(instr.Value))
						{
							this._currentRecursionDatum.RecursiveValueStorage.Add(instr.Value, new Stack<int>());
						}
						Console.WriteLine("---------------------------" + instr.Value);
						Console.WriteLine(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));

						this._currentRecursionDatum.RecursiveValueStorage[instr.Value].Push(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));//this value is zero, is it local when it is placed in the dictionary?
					}
					if (withinBegin && instr.Command == InstructionSet.End)
						withinBegin = false;
					if (afterCall && !withinBegin && instr.Command == InstructionSet.LValue)
					{
						Console.WriteLine("---------------------------" + instr.Value);
						Console.WriteLine(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));
						this._currentRecursionDatum.RecursiveValueStorage[instr.Value].Push(Convert.ToInt32(this._symbolTable[instr.Value].ToString()));//this value is zero, is it local when it is placed in the dictionary?
					}
				}
			}
		}

		#endregion Helpers
	}
}