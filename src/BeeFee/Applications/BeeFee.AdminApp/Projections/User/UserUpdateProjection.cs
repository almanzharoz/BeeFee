using System;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using SharpFuncExt;

namespace BeeFee.AdminApp.Projections.User
{
	internal class UserUpdateProjection : BaseEntity, IProjection<Model.Models.User>, IGetProjection, IUpdateProjection
	{
		public string Email { get; private set; }
		public string Name { get; private set; }
		private string Password { get; }
		public EUserRole[] Roles { get; private set; }
		private string Salt { get; }

		public UserUpdateProjection ChangeUser(string email, string name, EUserRole[] roles)
		{
			Email = email.HasNotNullArg(nameof(name)).Trim().ToLowerInvariant();
			Roles = roles.HasNotNullArg(nameof(roles));
			Name = name.HasNotNullArg(nameof(name)).Trim();
			return this;
		}

		public UserUpdateProjection(string id, string email, string name, string password, string salt, EUserRole[] roles) : base(id)
		{
			Name = name;
			Email = email;
			Roles = roles;
			Password = password;
			Salt = salt;
		}
	}
}