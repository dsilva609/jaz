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
			new Interpreter(@"Resources\guesstheanswer.jaz").Execute();//--remove parameter
		}
	}
}

/*
 * currently not working
 *	demo.jaz -- probably local vs global variable issue, one value wont update locally or passed --- works?
 *	recFact.jaz -- null reference exception, now stack overflow exception, now key not found exception
 *	foo.jaz -- local vs global variable issue --- ?
 *	factProc.jaz -- local vs global cariable issue --- works?
 *
 *  guessTheAnswer.jaz -- same key already added issue
*/