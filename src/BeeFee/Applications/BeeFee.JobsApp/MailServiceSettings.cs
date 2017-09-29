namespace BeeFee.JobsApp
{
	public class MailServiceSettings
	{
		public string AttachmentsFolder { get; set; }
		public string PickupDirectory { get; set; }
		public string Host { get; set; }
		public int Port { get; set; }
		public string User { get; set; }
		public string Password { get; set; }
		public string From { get; set; }
		public bool Ssl { get; set; }
	}
}