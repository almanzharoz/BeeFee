using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.TestsApp.Helpers;
using BeeFee.Model.Models;
using Microsoft.AspNetCore.WebUtilities;

namespace BeeFee.TestsApp.Projections
{
	internal class NewUser : BaseNewEntity, IProjection<User>
	{
		public string Email { get; }
		public string Name { get; }
		public string Password { get; }
		public string Salt { get; }
		public EUserRole[] Roles { get; set; }

		public NewUser() { } //Hack

		public NewUser(string email, string name, string password)
		{
			Email = email.ToLowerInvariant();
			Name = name.Trim();
			var salt = HashPasswordHelper.GenerateSalt();
			Salt = Base64UrlTextEncoder.Encode(salt);
			Password = HashPasswordHelper.GetHash(password, salt);
		}
	}
}