namespace jaz.Objects
{
	public class InstructionSet
	{
		#region Stack Manipulation

		public const string Push = "push";
		public const string RValue = "rvalue";
		public const string LValue = "lvalue";
		public const string Pop = "pop";
		public const string Top = ":=";
		public const string Copy = "copy";

		#endregion Stack Manipulation

		#region Control Flow

		public const string Label = "label";
		public const string GoTo = "goto";
		public const string GoFalse = "gofalse";
		public const string GoTrue = "gotrue";
		public const string Halt = "halt";

		#endregion Control Flow

		#region Arithmetic Operators

		public const string Addition = "+";
		public const string Subtraction = "-";
		public const string Multuplication = "*";
		public const string Division = "/";
		public const string Remainder = "div";

		#endregion Arithmetic Operators

		#region Logical Operators

		public const string AND = "&";
		public const string NOT = "!";
		public const string OR = "|";

		#endregion Logical Operators

		#region Relational Operators

		public const string NotEqual = "<>";
		public const string LesserOrEqual = "<=";
		public const string GreaterOrEqual = ">=";
		public const string Lesser = "<";
		public const string Greater = ">";
		public const string Equal = "=";

		#endregion Relational Operators

		#region Output

		public const string Print = "print";
		public const string Show = "show";

		#endregion Output

		#region Subprogram Control

		public const string Begin = "begin";
		public const string End = "end";
		public const string Return = "return";
		public const string Call = "call";

		#endregion Subprogram Control
	}
}