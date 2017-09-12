using BeeFee.LoginApp.Helpers;
using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using Microsoft.AspNetCore.WebUtilities;
using SharpFuncExt;

namespace BeeFee.LoginApp.Projections.User
{
	internal class RegisterUserProjection : BaseNewEntity, IProjection<Model.Models.User>
	{
		public string Email { get; }
		public string Name { get; }
		public string Password { get; }
		public string Salt { get; }
		public EUserRole[] Roles { get; }

		public RegisterUserProjection(string email, string name, string password, EUserRole[] roles)
		{
			Roles = roles.HasNotNullArg(nameof(roles));
			Email = email.ToLowerInvariant();
			Name = name.Trim();
			var salt = HashPasswordHelper.GenerateSalt();
			Salt = Base64UrlTextEncoder.Encode(salt);
			Password = HashPasswordHelper.GetHash(password, salt);
		}
	}
}