using BeeFee.Model.Embed;
using System;

namespace BeeFee.Model.Exceptions
{
	public class EventStateException : Exception
    {
		public EEventState State { get; }

		public EventStateException(EEventState state) : base()
		{
			State = state;
		}
		public EventStateException(EEventState state, string message) : base(message)
		{
			State = state;
		}
    }
}
