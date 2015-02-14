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