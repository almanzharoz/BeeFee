using System.Collections.Generic;
using Core.ElasticSearch;
using BeeFee.AdminApp.Projections.User;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.AdminApp.Services
{
	public class UserService : BaseBeefeeService
	{
		public UserService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
			ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
			user)
		{
		}

		///<exception cref="EntityAlreadyExistsException"></exception>
		public string AddUser(string email, string name, EUserRole[] roles)
		{
			if (FilterCount<UserProjection>(q => q.Term(x => x.Email.ToLowerInvariant(), email.ToLowerInvariant())) != 0)
				throw new EntityAlreadyExistsException();

			return new NewUserProjection(email, name, roles).Fluent(x => Insert(x, true)).Id;
		}

		public bool EditUser(string id, string email, string name, EUserRole[] roles)
			=> UpdateById<UserUpdateProjection>(id, u => u.ChangeUser(email, name, roles), true);

		public UserProjection GetUser(string id)
			=> GetById<UserProjection>(id);

		public IReadOnlyCollection<UserProjection> SearchUsersByEmail(string query)
			=> Search<User, UserProjection>(q => q
				.Wildcard(w => w
					.Field(x => x.Email)
					.Value($"*{query.ToLowerInvariant()}*")));

		public bool DeleteUser(string id) => Remove<UserProjection>(id, true);
	}
}