using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using SharpFuncExt;

namespace BeeFee.AdminApp.Projections.User
{
	internal class NewUserProjection : BaseNewEntity, IProjection<Model.Models.User>
	{
		public string Email { get; }
		public string Name { get; }
		public EUserRole[] Roles { get; }

		public NewUserProjection(string email, string name, EUserRole[] roles)
		{
			Email = email.HasNotNullArg(nameof(email)).ToLowerInvariant();
			Name = name.HasNotNullArg(nameof(name)).Trim();
			Roles = roles.HasNotNullArg(nameof(roles));
		}
	}
}