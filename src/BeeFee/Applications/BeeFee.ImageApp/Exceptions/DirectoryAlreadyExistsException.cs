using System;
using System.Runtime.Serialization;

namespace BeeFee.ImageApp.Exceptions
{
	public class DirectoryAlreadyExistsException : Exception
	{
		public DirectoryAlreadyExistsException()
		{
		}

		public DirectoryAlreadyExistsException(string message) : base(message)
		{
		}

		public DirectoryAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected DirectoryAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}