using jaz.Logic;

namespace jaz
{
	public class MainWrapper
	{
		public static void Main(string[] args)
		{
			/*	Flow should be:
			 *		Create Interpreter object
			 *			pass file to interpret
			 *		interpreter parses instruction
			 *			saves to Instruction object
			 *		list of parsed instructions are then executed
			 *		output of executed instructions are displayed
			 *			file or console?
			 */
			new Interpreter(@"Resources\demo.jaz").Execute();//--remove parameter
		}
	}
}

/*
 * currently not working
 *	demo.jaz -- probably local vs global variable issue, last number is off by 1, also repeat print out
 *	recFact.jaz -- null reference exception
 *	foo.jaz -- local vs global variable issue
 *	factProc.jaz -- DOA
*/