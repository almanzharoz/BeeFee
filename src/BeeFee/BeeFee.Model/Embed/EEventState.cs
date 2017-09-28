namespace BeeFee.Model.Embed
{
    public enum EEventState : short
    {
		None = 0,
		Created = 1,
        Open,
        Close,
        Cancel,
		Archive,
		Moderating,
		NotModerated
    }
}