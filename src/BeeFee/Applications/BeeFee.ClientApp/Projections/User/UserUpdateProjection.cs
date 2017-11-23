using System;
using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.ClientApp.Projections.User
{
	internal class UserUpdateProjection : BaseEntity, IProjection<Model.Models.User>, IUpdateProjection, IGetProjection, ISearchProjection
	{
		public string Name { get; private set; }
		public string VerifyEmail { get; private set; }
		public EUserRole[] Roles { get; }

		internal UserUpdateProjection ChangeName(string name)
		{
			Name = name.HasNotNullArg(nameof(name));
			return this;
		}

		internal UserUpdateProjection Recover()
		{
			VerifyEmail = Guid.NewGuid().ToString();
			return this;
		}

		public UserUpdateProjection(string id, string name, string verifyEmail, EUserRole[] roles) : base(id)
		{
			Name = name;
			Roles = roles;
			VerifyEmail = verifyEmail;
		}
	}
}