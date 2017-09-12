using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;

namespace BeeFee.AdminApp.Projections.User
{
	public class UserProjection : BaseEntity, IProjection<Model.Models.User>, IGetProjection, ISearchProjection, IRemoveProjection
	{
		public string Email { get; }
		public string Name { get; }
		public EUserRole[] Roles { get; }

		public UserProjection(string id, string name, string email, EUserRole[] roles) : base(id)
		{
			Email = email;
			Name = name;
			Roles = roles;
		}
	}
}