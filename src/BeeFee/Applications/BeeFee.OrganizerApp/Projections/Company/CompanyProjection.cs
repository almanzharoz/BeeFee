using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;

namespace BeeFee.OrganizerApp.Projections.Company
{
	public class CompanyProjection: BaseEntityWithVersion, IProjection<Model.Models.Company>, IGetProjection, ISearchProjection, IUpdateProjection, IRemoveProjection, IWithName, IWithUrl
	{
		public string Url { get; private set; }
		public string Name { get; private set; }

		public CompanyUser[] Users { get; }

		public CompanyProjection(string id, int version, CompanyUser[] users, string name, string url) : base(id, version)
		{
			Url = url;
			Name = name;
			Users = users;
		}

		internal CompanyProjection Update(string name, string url)
		{
			Url = url;
			Name = name;
			return this;
		}
	}

	public class CompanyJoinProjection : BaseEntityWithVersion, IProjection<Model.Models.Company>, IJoinProjection, IGetProjection, ISearchProjection, IWithName, IWithUrl
	{
		public string Url { get; private set; }
		public string Name { get; private set; }

		public CompanyUser[] Users { get; private set; }

		public CompanyJoinProjection(string id) : base(id)
		{
		}

	}

}