﻿using System;
using System.Collections.Generic;

namespace jaz.Objects
{
	public class RecursionDatum
	{
		public Guid RecursionID { get; set; }

		public string RecursionFunctionName { get; set; }

		//public int CallsRemaining { get; set; }//probably not needed

		//public int RecursiveCalls { get; set; }//probably not needed

		public Guid RecursiveReturnValue { get; set; }

		public Guid OriginalReturnValue { get; set; }

		public Dictionary<string, Stack<int>> RecursiveValueStorage { get; set; }//////////////////how should this be used?

		public RecursionDatum()
		{
			RecursiveValueStorage = new Dictionary<string, Stack<int>>();
			//CallsRemaining = 0;
		}
	}
}