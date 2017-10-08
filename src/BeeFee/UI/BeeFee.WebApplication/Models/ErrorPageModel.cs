using System;

namespace BeeFee.WebApplication.Models
{
    public class ErrorPageModel
    {
        public ErrorPageModel(string message)
        {
            this.Message = message;
        }

        public string RequestId { get; set; }

        public string Message { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}