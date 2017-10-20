namespace BeeFee.Model.Jobs.Data
{
	public struct CreateTicket
	{
		public string Filename { get; }
		public string Event { get; }
		public string Name { get; }
		public string Date { get; }
		public string Email { get; }
		public string Cover { get; }

		public CreateTicket(string @event, string name, string date, string email, string cover, string filename)
		{
			Event = @event;
			Name = name;
			Date = date;
			Email = email;
			Filename = filename;
			Cover = cover;
		}
	}
}