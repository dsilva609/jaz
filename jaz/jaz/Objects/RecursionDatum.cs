using System;
using System.Collections.Generic;

namespace jaz.Objects
{
	public class RecursionDatum
	{
		public Guid RecursionID { get; set; }

		public int CallsRemaining { get; set; }

		public int RecursiveCalls { get; set; }//probably not needed

		public Guid RecursiveReturnValue { get; set; }

		public Guid OriginalReturnValue { get; set; }

		public Stack<int> RecursiveValueStorage { get; set; }//////////////////how should this be used?

		public RecursionDatum()
		{
			CallsRemaining = 0;
		}
	}
}