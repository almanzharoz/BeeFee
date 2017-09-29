namespace BeeFee.Model.Jobs.Data
{
	/// <summary>
	/// Отправка email
	/// </summary>
	public struct SendMail
	{
		public string From { get; }
		public string To { get; }
		public string Message { get; }
		public string Subject { get; }
		public string[] Files { get; }

		public SendMail(string from, string to, string message, string subject, string[] files)
		{
			From = from;
			To = to;
			Message = message;
			Subject = subject;
			Files = files;
		}
	}
}