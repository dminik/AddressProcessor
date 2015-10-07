using System;

namespace AddressProcessing.Exceptions
{
	public class WrongFieldsNumberException : Exception
	{
		public uint LineNumber { get; private set; }
		public uint ExpectedNum { get; private set; }
		public uint ActualNum { get; private set; }		

		public WrongFieldsNumberException(uint lineNumber, uint expectedNum, uint actualNum)
		{
			LineNumber = lineNumber;
			ExpectedNum = expectedNum;
			ActualNum = actualNum;			
		}

		public override string Message
		{
			get
			{
				return string.Format("Wrong fields number in line {0}. Expected {1} but was found {2}.",
						LineNumber,
						ExpectedNum,
						ActualNum);
			}

		}
	}
}
