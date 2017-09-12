﻿namespace BeeFee.ImageApp
{
	public enum EAddImageResut
	{
		Ok,
		Error,
		Exists
	}
	public struct AddImageResult
	{
		public EAddImageResut Result { get; }
		public string Error { get; }
		public string Path { get; }

		public AddImageResult(EAddImageResut result, string path, string error)
		{
			Result = result;
			Path = path;
			Error = error;
		}

	}
}