using BeeFee.Model.Embed;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeeFee.Model.Exceptions
{
    public class EventStatusException : Exception
    {
		public EEventState State { get; }
		public EventStatusException(EEventState state)
		{
			State = state;
		}
    }
}
