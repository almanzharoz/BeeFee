namespace BeeFee.ImageApp.Embed
{
	internal class MemoryCacheKeyObject
	{
		public EKeyType Type { get; set; }
		public string Directory { get; set; }
		public bool HasAccessToSubdirectories { get; set; }

		internal MemoryCacheKeyObject(EKeyType keyType, string directory, bool hasAccessToSubdirectories)
		{
			Type = keyType;
			Directory = directory;
			HasAccessToSubdirectories = hasAccessToSubdirectories;
		}

		internal bool IsServerKey
			=> Type == EKeyType.Server;
	}

	public enum EKeyType // 
	{
		User,
		Moderator,
		Company,
		Server
	}
}