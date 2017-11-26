using System;
using System.Runtime.Serialization;

namespace BeeFee.ImageApp2.Exceptions
{
	public class FileNotSupportedException : Exception
	{
		public FileNotSupportedException()
		{
		}

		public FileNotSupportedException(string message) : base(message)
		{
		}

		public FileNotSupportedException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FileNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}