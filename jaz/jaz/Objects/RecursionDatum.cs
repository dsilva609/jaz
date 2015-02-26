using System;
using System.Collections.Generic;

namespace jaz.Objects
{
	public class RecursionDatum
	{
		public Guid RecursionID { get; set; }

		public string RecursionFunctionName { get; set; }

		public Guid RecursiveReturnValue { get; set; }

		public Guid OriginalReturnValue { get; set; }

		public Dictionary<string, Stack<int>> RecursiveValueStorage { get; set; }

		public RecursionDatum()
		{
			RecursiveValueStorage = new Dictionary<string, Stack<int>>();
		}
	}
}