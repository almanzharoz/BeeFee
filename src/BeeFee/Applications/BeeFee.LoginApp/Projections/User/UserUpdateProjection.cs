using BeeFee.LoginApp.Helpers;
using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using SharpFuncExt;

namespace BeeFee.LoginApp.Projections.User
{
	internal class UserUpdateProjection : BaseEntity, IProjection<Model.Models.User>, IUpdateProjection, IGetProjection
	{
		private string Password { get; set; }
		private string Salt { get; }
		public string Name { get; private set; }
		public EUserRole[] Roles { get; }

		internal UserUpdateProjection ChangePassword(/*string oldPassword, */string newPassword)
		{
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

		internal UserUpdateProjection Change(string name)
		{
			Name = name.HasNotNullArg(nameof(name));
			return this;
		}

		public UserUpdateProjection(string id, string name, string passwod, string salt, EUserRole[] roles) : base(id)
		{
			Name = name;
			Password = passwod;
			Salt = salt;
			Roles = roles;
		}
	}
}