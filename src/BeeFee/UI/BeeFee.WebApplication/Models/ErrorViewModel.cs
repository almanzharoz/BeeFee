using System;

namespace BeeFee.WebApplication.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public string Message { get; set; }

		public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

	    public ErrorViewModel(string message)
	    {
		    Message = message;
	    }
    }
}