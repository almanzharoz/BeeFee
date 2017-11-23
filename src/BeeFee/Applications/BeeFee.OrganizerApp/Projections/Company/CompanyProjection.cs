using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;

namespace BeeFee.OrganizerApp.Projections.Company
{
	public class CompanyProjection: BaseEntityWithVersion, IProjection<Model.Models.Company>, IGetProjection, ISearchProjection, IUpdateProjection, IRemoveProjection, IWithName, IWithUrl
	{
		public string Url { get; private set; }
		public string Name { get; private set; }
		public string Email { get; private set; }
		public string Logo { get; private set; }

		public CompanyUser[] Users { get; }

		public CompanyProjection(string id, int version, CompanyUser[] users, string name, string url, string email, string logo) : base(id, version)
		{
			Url = url;
			Name = name;
			Users = users;
			Email = email;
			Logo = logo;
		}

		internal CompanyProjection Update(string name, string url, string email, string logo)
		{
			Url = url;
			Name = name;
			Email = email;
			Logo = logo;
			return this;
		}
	}

	public class CompanyJoinProjection : BaseEntityWithVersion, IProjection<Model.Models.Company>, IJoinProjection, IGetProjection, ISearchProjection, IWithName, IWithUrl
	{
		public string Url { get; private set; }
		public string Name { get; private set; }

		public CompanyUser[] Users { get; private set; }

		internal CompanyJoinProjection(string id) : base(id)
		{
		}

	}

}