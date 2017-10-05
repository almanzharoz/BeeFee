namespace BeeFee.ImageApp.Embed
{
	internal class MemoryCacheKeyObject
	{
		public EKeyType Type { get; set; }
		public string Directory { get; set; }

		public MemoryCacheKeyObject(EKeyType keyType, string directory)
		{
			Type = keyType;
			Directory = directory;
		}
	}

	public enum EKeyType
	{
		User,
		Moderator,
		Company
	}
}