﻿using System;
using System.Linq;
using Core.ElasticSearch;
using BeeFee.LoginApp.Projections.User;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Jobs.Data;
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
		    => Filter<User, UserProjection>(q => q.Term(x => x.Email, email), null, 0,1)
			    .FirstOrDefault(x => x.CheckPassword(password));

        public (UserRegistrationResult, UserProjection) Register(string email, string name, string password)
        {
            if (String.IsNullOrEmpty(email))
                return (UserRegistrationResult.EmailIsEmpty, null);

            if (!CommonHelper.IsValidEmail(email))
                return (UserRegistrationResult.WrongEmail, null);

			if (String.IsNullOrWhiteSpace(password))
                return (UserRegistrationResult.PasswordIsEmpty, null);

			if (String.IsNullOrEmpty(name))
                return (UserRegistrationResult.NameIsEmpty, null);

			if (FilterCount<UserProjection>(q => q.Term(x => x.Email, email.ToLowerInvariant())) > 0)
                return (UserRegistrationResult.EmailAlreadyExists, null);

			var result = Insert<RegisterUserProjection, UserProjection>(
				new RegisterUserProjection(email, name, password,
					email == "admin@dk.ru"
						? new[] {EUserRole.Admin}
						: new[] {EUserRole.User /*, EUserRole.Organizer, EUserRole.Admin, EUserRole.EventModerator*/}));

			return (result != null
		        ? UserRegistrationResult.Ok
		        : UserRegistrationResult.UnknownError, result);
        }

	    public bool ChangePassword(string email, string oldPassword, string newPassword)
		    => TryLogin(email, oldPassword)
			    .NotNullOrDefault(
				    user => UpdateById<UserUpdateProjection>(user.Id, x => x.ChangePassword(newPassword), true));

		public T GetUser<T>() where T : BaseEntity, IProjection<User>, IGetProjection
			=> GetById<T>(User.Id);

		public bool UpdateUser(string name)
			=> UpdateById<UserUpdateProjection>(User.Id, x => x.Change(name), true);

		public bool Recover(string email)
			=> Filter<Model.Models.User, UserUpdateProjection>(
					q => q.Term(p => p.Email, email.HasNotNullArg(nameof(email))), null, 0, 1).FirstOrDefault()
				.NotNullOrDefault(u => Update(u, f => f.Recover(), false)
					.IfTrue(() => AddJob(new SendMail(null, email,
						"<a href='http://localhost:55793/Account/SetPassword/" + u.VerifyEmail + "'></a>",
						"Восстановлене пароля", null), DateTime.UtcNow)));

		public UserProjection VerifyEmailForRecover(string verifyEmail)
			=> Filter<User, UserProjection>(
					q => q.Term(p => p.VerifyEmail, verifyEmail.HasNotNullArg(nameof(verifyEmail)).Trim()), null, 0, 1)
				.FirstOrDefault();

		public bool Recover(string verifyEmail, string newPassword)
			=> UpdateWithFilter<UserUpdateProjection>(
				q => q.Term(p => p.VerifyEmail, verifyEmail.HasNotNullArg(nameof(verifyEmail)).Trim()), null,
				u => u.ChangePassword(newPassword), true);
	}
}