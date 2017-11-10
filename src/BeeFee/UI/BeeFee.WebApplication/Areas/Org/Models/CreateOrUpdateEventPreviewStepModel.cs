using BeeFee.Model.Helpers;
using BeeFee.Model.Interfaces;

namespace BeeFee.WebApplication.Areas.Org.Models
{
    public class CreateOrUpdateEventPreviewStepModel : CreateOrUpdateEventModel
    {
        public IEventPageProjection Preview { get; }

        public CreateOrUpdateEventPreviewStepModel()
        {
            Step = 3;
        }

        public CreateOrUpdateEventPreviewStepModel(IEventPageProjection @event, int version) : base(@event.Id, @event.Parent.Id, 3)
        {
            Version = version;
            Preview = @event.HasNotNullEntity("Event");
        }
    }
}
