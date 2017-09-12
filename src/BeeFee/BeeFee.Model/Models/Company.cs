using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Projections;

namespace BeeFee.Model.Models
{
	public class Company : BaseEntity
	{
		public string Name { get; set; }
		public CompanyUser[] Users { get; set; }

		public Company(string id) : base(id)
		{
		}
	}

	public struct CompanyUser
	{
		public BaseUserProjection User { get; set; }
		public ECompanyUserRole Role { get; set; }
	}
}