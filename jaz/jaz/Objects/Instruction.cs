using System;

namespace jaz.Objects
{
	public class Instruction
	{
		public string Command { get; set; }

		public string Value { get; set; }

		public Guid GUID { get; set; }

		//--add a IsLocalVariable boolean?
	}
}