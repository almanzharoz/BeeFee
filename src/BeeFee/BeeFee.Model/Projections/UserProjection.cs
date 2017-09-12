using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Projections
{
	public class BaseUserProjection : BaseEntity, IProjection<User>, IJoinProjection, IGetProjection
	{
		public string Name { get; private set; }
		public EUserRole[] Roles { get; private set; }

		public BaseUserProjection(string id) : base(id)
		{
		}
	}

	public class UserName
	{
		public string Id { get; }

		public UserName(string id)
		{
			Id = id;
		}

		public UserName(BaseUserProjection user)
		{
			Id = user.Id;
		}
	}
}