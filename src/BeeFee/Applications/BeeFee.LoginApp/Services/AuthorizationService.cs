using System;
using System.Linq;
using System.Threading.Tasks;
using Core.ElasticSearch;
using BeeFee.LoginApp.Projections.User;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Jobs.Data;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
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

		public async Task<UserProjection> TryLoginAsync(string email, string password)
			=> (await FilterAsync<User, UserProjection>(q => q.Term(x => x.Email, email), null, 0, 1))
				.FirstOrDefault(x => x.CheckPassword(password));

		public UserProjection TryLogin(UserName userName, string password)
			=> GetById<UserProjection>(userName.Id).HasNotNullArg("user").If(u => u.CheckPassword(password), u => u, u => null);

		public Task<bool> RegisterAsync(string email, string name, string password)
			=> InsertAsync(new RegisterUserProjection(
				email.ThrowIf(e => FilterCount<UserProjection>(q => q.Term(x => x.Email, e.ToLowerInvariant())) > 0,
					e => new EntityAlreadyExistsException()), name, password,
				email == "admin@dk.ru"
					? new[] {EUserRole.Admin}
					: new[] {EUserRole.User}), true);

		public Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
			=> TryLogin(User, oldPassword)
				.NotNullOrDefault(
					user => UpdateByIdAsync<UserUpdateProjection>(user.Id, x => x.ChangePassword(newPassword), true));

		public bool RecoverLink(string email, string host)
			=> Filter<Model.Models.User, UserUpdateProjection>(
					q => q.Term(p => p.Email, email.HasNotNullArg(nameof(email))), null, 0, 1).FirstOrDefault()
				.NotNullOrDefault(u => Update(u, f => f.Recover(), false)
					.IfTrue(() => AddJob(new SendMail(null, email,
						"<a href='"+host+"/Account/SetPassword/" + u.VerifyEmail + "'>ссылка для восстановления пароля</a>",
						"Восстановлене пароля", null), DateTime.UtcNow)));

		public async Task<bool> RecoverLinkAsync(string email, string host)
			=> await (await FilterFirstAsync<Model.Models.User, UserUpdateProjection>(
					q => q.Term(p => p.Email, email.HasNotNullArg(nameof(email)))))
				.NotNullOrDefault(async u =>
					await UpdateAsync(u, f => f.Recover(), false) &&
					await AddJobAsync(new SendMail(null, email,
						"<a href='" + host + "/Account/SetPassword/" + u.VerifyEmail + "'>ссылка для восстановления пароля</a>",
						"Восстановлене пароля", null), DateTime.UtcNow));

		public UserProjection VerifyEmailForRecover(string verifyEmail)
			=> Filter<User, UserProjection>(
					q => q.Term(p => p.VerifyEmail, verifyEmail.HasNotNullArg(nameof(verifyEmail)).Trim()), null, 0, 1)
				.FirstOrDefault();

		public bool Recover(string verifyEmail, string newPassword)
			=> UpdateWithFilter<UserUpdateProjection>(
				q => q.Term(p => p.VerifyEmail, verifyEmail.HasNotNullArg(nameof(verifyEmail)).Trim()), null,
				u => u.ChangePassword(newPassword), true);

		public Task<bool> RecoverAsync(string verifyEmail, string newPassword)
			=> UpdateWithFilterAsync<UserUpdateProjection>(
				q => q.Term(p => p.VerifyEmail, verifyEmail.HasNotNullArg(nameof(verifyEmail)).Trim()), null,
				u => u.ChangePassword(newPassword), true);

	}
}