using WebApplication3.Models.Interfaces;

namespace WebApplication3.Areas.Org.Models.Company
{
    public class CompanyIdModel : IIdModel, IModelWithVersion
    {
		public string Id { get; set; }
		public int Version { get; set; }
	}
}
