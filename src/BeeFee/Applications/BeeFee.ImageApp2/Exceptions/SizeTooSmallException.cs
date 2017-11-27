using System;
using System.Runtime.Serialization;

namespace BeeFee.ImageApp2.Exceptions
{
	public class SizeTooSmallException : Exception
	{
		public SizeTooSmallException()
		{
		}

		public SizeTooSmallException(string message) : base(message)
		{
		}

		public SizeTooSmallException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected SizeTooSmallException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}