namespace jaz.Objects
{
	public class Instruction
	{
		public string Name { get; set; }
		public string Value { get; set; }
		//add property for InstructionSetValue from a new InstructionSet object that has public const string properties

		/*If you are using C#, Why not create an enum and set string based Description attribute for the enum values as below:

		public enum CustomerType
		{			
			[System.ComponentModel.Description("Customer Type 1")]
			Type1,

			[System.ComponentModelDescription("Customer Type 2")]
			Type2
		}
		
		 * Then, you can get the Description value of enum values as below:
	
			int value = CustermType.Type1;
			string type1Description = Enums.GetDescription((CustomerType)value);
		  
		 */
	}
}