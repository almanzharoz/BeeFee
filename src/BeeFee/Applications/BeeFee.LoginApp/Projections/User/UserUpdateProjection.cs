using System;
using BeeFee.LoginApp.Helpers;
using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using SharpFuncExt;

namespace BeeFee.LoginApp.Projections.User
{
	internal class UserUpdateProjection : BaseEntity, IProjection<Model.Models.User>, IUpdateProjection, IGetProjection, ISearchProjection
	{
		public string Password { get; private set; }
		private string Salt { get; }
		public string Name { get; private set; }
		public string VerifyEmail { get; private set; }
		public EUserRole[] Roles { get; }

		internal UserUpdateProjection ChangePassword(string newPassword)
		{
			Password = HashPasswordHelper.GetHash(newPassword, Base64UrlTextEncoder.Decode(Salt));
			return this;
		}

		internal UserUpdateProjection Change(string name)
		{
			Name = name.HasNotNullArg(nameof(name));
			return this;
		}

		internal UserUpdateProjection Recover()
		{
			VerifyEmail = Guid.NewGuid().ToString();
			return this;
		}

		public UserUpdateProjection(string id, string name, string password, string salt, string verifyEmail, EUserRole[] roles) : base(id)
		{
			Name = name;
			Password = password;
			Salt = salt;
			Roles = roles;
			VerifyEmail = verifyEmail;
		}
	}
}