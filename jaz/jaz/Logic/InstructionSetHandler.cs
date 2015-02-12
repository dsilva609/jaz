using System;

namespace jaz.Logic
{
	public class InstructionSetHandler
	{
		#region Stack Manipulation

		public virtual void Push()
		{ throw new NotImplementedException(); }

		public virtual void RValue()
		{ throw new NotImplementedException(); }

		public virtual void LValue()
		{ throw new NotImplementedException(); }

		public virtual void Pop()
		{ throw new NotImplementedException(); }

		public virtual void Top()
		{ throw new NotImplementedException(); }

		public virtual void Copy()
		{ throw new NotImplementedException(); }

		#endregion Stack Manipulation

		#region Control Flow

		public virtual void Label()
		{ throw new NotImplementedException(); }

		public virtual void GoTo()
		{ throw new NotImplementedException(); }

		public virtual void GoFalse()
		{ throw new NotImplementedException(); }

		public virtual void GoTrue()
		{ throw new NotImplementedException(); }

		public virtual void Halt()
		{ throw new NotImplementedException(); }

		#endregion Control Flow

		#region Arithmetic Operators

		public virtual void Addition()
		{ throw new NotImplementedException(); }

		public virtual void Subtraction()
		{ throw new NotImplementedException(); }

		public virtual void Multuplication()
		{ throw new NotImplementedException(); }

		public virtual void Division()
		{ throw new NotImplementedException(); }

		public virtual void Remainder()
		{ throw new NotImplementedException(); }

		#endregion Arithmetic Operators

		#region Logical Operators

		public virtual void AND()
		{ throw new NotImplementedException(); }

		public virtual void NOT()
		{ throw new NotImplementedException(); }

		public virtual void OR()
		{ throw new NotImplementedException(); }

		#endregion Logical Operators

		#region Relational Operators

		public virtual void NotEqual()
		{ throw new NotImplementedException(); }

		public virtual void LesserOrEqual()
		{ throw new NotImplementedException(); }

		public virtual void GreaterOrEqual()
		{ throw new NotImplementedException(); }

		public virtual void Lesser()
		{ throw new NotImplementedException(); }

		public virtual void Greater()
		{ throw new NotImplementedException(); }

		public virtual void Equal()
		{ throw new NotImplementedException(); }

		#endregion Relational Operators

		#region Output

		public virtual void Print()
		{ throw new NotImplementedException(); }

		public virtual void Show()
		{ throw new NotImplementedException(); }

		#endregion Output

		#region Subprogram Control

		public virtual void Begin()
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