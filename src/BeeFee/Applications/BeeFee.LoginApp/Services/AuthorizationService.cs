using System;
using System.Linq;
using Core.ElasticSearch;
using BeeFee.LoginApp.Projections.User;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Microsoft.Extensions.Logging;
using SharpFuncExt;

namespace BeeFee.LoginApp.Services
{
    public class AuthorizationService : BaseBeefeeService
    {
        public AuthorizationService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings,
            ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) : base(loggerFactory, settings, factory,
            user)
        {
        }

	    public UserProjection TryLogin(string email, string password)
		    => Filter<User, UserProjection>(q => q.Term(x => x.Email, email), null, 1)
			    .FirstOrDefault(x => x.CheckPassword(password));

        public UserRegistrationResult Register(string email, string name, string password)
        {
            if (String.IsNullOrEmpty(email))
                return UserRegistrationResult.EmailIsEmpty;

            if (!CommonHelper.IsValidEmail(email))
                return UserRegistrationResult.WrongEmail;

            if (String.IsNullOrWhiteSpace(password))
                return UserRegistrationResult.PasswordIsEmpty;

            if (String.IsNullOrEmpty(name))
                return UserRegistrationResult.NameIsEmpty;

            if (FilterCount<UserProjection>(q => q.Term(x => x.Email, email.ToLowerInvariant())) > 0)
                return UserRegistrationResult.EmailAlreadyExists;

	        return Insert(new RegisterUserProjection(email, name, password, new[] {EUserRole.User, EUserRole.Organizer, EUserRole.Admin}), true)
		        ? UserRegistrationResult.Ok
		        : UserRegistrationResult.UnknownError;
        }

	    public bool ChangePassword(string email, string oldPassword, string newPassword)
		    => TryLogin(email, oldPassword)
			    .NotNullOrDefault(
				    user => Update<UserUpdateProjection>(user.Id, x => x.ChangePassword(/*oldPassword, */newPassword), true));

		public T GetUser<T>() where T : BaseEntity, IProjection<User>, IGetProjection
			=> Get<T>(User.Id);

		public bool UpdateUser(string name)
			=> Update<UserUpdateProjection>(User.Id, x => x.Change(name), true);
	}
}