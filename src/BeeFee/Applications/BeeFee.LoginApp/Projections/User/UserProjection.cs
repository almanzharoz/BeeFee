using BeeFee.LoginApp.Helpers;
using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace BeeFee.LoginApp.Projections.User
{
	public class UserProjection : BaseEntity, IProjection<Model.Models.User>, IGetProjection, ISearchProjection
	{
		public string Email { get; }
		public string Name { get; }
		private string Password { get; }
		private string Salt { get; }
		public EUserRole[] Roles { get; }

		internal bool CheckPassword(string password)
			=> HashPasswordHelper.GetHash(password, Base64UrlTextEncoder.Decode(Salt)) == Password;

		public UserProjection(string id, string name, string email, string password, string salt, EUserRole[] roles) : base(id)
		{
			Name = name;
			Email = email;
			Password = password;
			Salt = salt;
			Roles = roles;
		}
	}
}