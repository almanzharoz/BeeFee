namespace BeeFee.Model.Jobs.Data
{
	public struct CreateTicket
	{
		public string Filename { get; }

		public CreateTicket(string filename)
		{
			Filename = filename;
		}
	}
}