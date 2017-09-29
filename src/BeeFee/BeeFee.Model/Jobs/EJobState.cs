namespace BeeFee.Model.Jobs
{
	public enum EJobState : short
	{
		None = 0,
		New = 1,
		Doing,
		Done,
		Error
	}
}