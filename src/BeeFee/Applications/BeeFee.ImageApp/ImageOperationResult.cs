namespace BeeFee.ImageApp
{
	public enum EAddImageResut
	{
		Ok,
		Error
	}

	public enum EErrorType
	{
		None,
		FileNotFound,
		FileAlreadyExists,
		FileDoesNotExists,
		SettingNotFound,
		SaveImageError
	}

	public struct ImageOperationResult
	{
		public EAddImageResut Result { get; }
		public EErrorType ErrorType { get; }
		public string Error { get; }
		public string Path { get; }

		public ImageOperationResult(EAddImageResut result, string path, string error = null, EErrorType errorType = EErrorType.None)
		{
			Result = result;
			Path = path;
			Error = error;
			ErrorType = errorType;
		}

	}
}