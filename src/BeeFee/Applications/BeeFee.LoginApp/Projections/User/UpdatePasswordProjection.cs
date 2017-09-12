using BeeFee.LoginApp.Helpers;
using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;

namespace BeeFee.LoginApp.Projections.User
{
	internal class UpdatePasswordProjection : BaseEntity, IProjection<Model.Models.User>, IUpdateProjection, IGetProjection
	{
		private string Password { get; set; }
		private string Salt { get; }
		public string Name { get; }
		public EUserRole[] Roles { get; }

		internal UpdatePasswordProjection ChangePassword(/*string oldPassword, */string newPassword)
		{
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

		public UpdatePasswordProjection(string id, string name, string passwod, string salt, EUserRole[] roles) : base(id)
		{
			Name = name;
			Password = passwod;
			Salt = salt;
			Roles = roles;
		}
	}
}