using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Nest;

namespace BeeFee.Model.Models
{
	public abstract class Company : IModel, IWithVersion, IWithName, IWithUrl
	{
		public string Id { get; set; }
		public int Version { get; set; }
		[Keyword]
		public string Name { get; set; }
		[Keyword]
		public string Url { get; set; }
		public CompanyUser[] Users { get; set; }
	}

	public struct CompanyUser
	{
		[Keyword]
		public BaseUserProjection User { get; }
		[Keyword]
		public ECompanyUserRole Role { get; }

		public CompanyUser(BaseUserProjection user, ECompanyUserRole role)
		{
			User = user;
			Role = role;
		}
	}
}