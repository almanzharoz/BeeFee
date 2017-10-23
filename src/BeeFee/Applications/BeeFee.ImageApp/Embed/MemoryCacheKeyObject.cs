namespace BeeFee.ImageApp.Embed
{
	internal class MemoryCacheKeyObject
	{
		public EKeyType Type { get; set; }
		public string Directory { get; set; }
		public bool HasAccessToSubdirectories { get; set; }

		public MemoryCacheKeyObject(EKeyType keyType, string directory, bool hasAccessToSubdirectories)
		{
			Type = keyType;
			Directory = directory;
			HasAccessToSubdirectories = hasAccessToSubdirectories;
		}
	}

	public enum EKeyType // добавить ключ "server"
	{
		User,
		Moderator,
		Company
	}
}