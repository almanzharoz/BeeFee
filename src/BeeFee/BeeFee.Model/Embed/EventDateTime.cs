using System;
using Newtonsoft.Json;

namespace BeeFee.Model.Embed
{
	public struct EventDateTime
	{
		public DateTime Start { get; }
		public DateTime Finish { get; }
		public string Timezone { get; }

		public EventDateTime(DateTime start, DateTime finish, string timezone = null)
		{
			Start = start;
			Finish = finish;
			Timezone = timezone;
			if (start >= finish) throw new IndexOutOfRangeException("start >= finish");
		}

		public override string ToString()
			=> String.Concat(Start.ToString("g"), " - ", Finish.ToString("g"));
	}
}