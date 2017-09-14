﻿using System;
using System.Runtime.Serialization;

namespace BeeFee.ImageApp
{
	public class FileAlreadyExistsException : Exception
	{
		public FileAlreadyExistsException()
		{
		}

		public FileAlreadyExistsException(string message) : base(message)
		{
		}

		public FileAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected FileAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}