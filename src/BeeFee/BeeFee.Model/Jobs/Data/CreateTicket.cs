namespace BeeFee.Model.Jobs.Data
{
	public struct CreateTicket
	{
		public string Filename { get; }
		public string Name { get; }
		public string Date { get; }

		public CreateTicket(string name, string date, string filename)
		{
			Name = name;
			Date = date;
			Filename = filename;
		}
	}
}