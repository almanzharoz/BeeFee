using WebApplication3.Models;
using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Moderator.Models
{
    public class EventIdModel : IIdModel, IParentModel
    {
		public string Id { get; set; }
		public string ParentId { get; set; }
	}
}
