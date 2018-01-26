using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeeFee.LoginApp.Projections.User;
using BeeFee.LoginApp.Services;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpFuncExt;
using WebApplication3.Models.Account;

namespace WebApplication3.Controllers
{
    public class AccountController : BaseController<AuthorizationService>
    {
		private readonly BeeFeeWebAppSettings _settings;

		public AccountController(AuthorizationService service, BeeFeeWebAppSettings settings) : base(service)
		{
			_settings = settings;
		}

		#region Login
		[HttpGet]
		public IActionResult Login(string returnUrl = null)
			=> View(new LoginModel { ReturnUrl = returnUrl });

		[HttpPost]
		public async Task<IActionResult> Login(LoginModel model)
		{
			if (model.Login == null || model.Pass == null)
				return TryAjaxView("_LoginForm", model);

			var user = await Service.TryLoginAsync(model.Login.Trim().ToLower(), model.Pass.Trim());
			if (user == null)
			{
				ModelState.AddModelError("error", "Неверный логин или пароль");
				return TryAjaxView("_LoginForm", model);
			}

			await Login(user);

			//_logger.LogInformation(4, "User logged in.");

			return IsAjax ? (IActionResult)Json(new { url = model.ReturnUrl }) : Redirect(model.ReturnUrl ?? "/");
		}
		#endregion

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> LogOff()
		{
			await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");

			//_logger.LogInformation(4, "User logged out.");

			return Redirect(/*Request.Headers["Referer"].FirstOrDefault() ?? */"/");
		}

		public IActionResult AccessDenied()
		{
			return Content("Access Denied");
		}

		#region Recover
		[HttpGet]
		public IActionResult Recover()
			=> TryAjaxView("Recover", new RecoverModel());

		
		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<IActionResult> Recover(RecoverModel model)
			=> ModelStateIsValid(model, 
				m => Service.RecoverLinkAsync(m.Email, _settings.WebAppUrl),
				m => TryAjaxView("RecoverDone", m),
				m => TryAjaxView("Recover", m));

		[HttpGet]
		public IActionResult SetPassword(string id)
			=> View(new SetPasswordModel(id.Fluent(x => Service.VerifyEmailForRecover(x)
				.ThrowIfNull<UserProjection, NotFoundException>())));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<IActionResult> SetPassword(SetPasswordModel model)
			=> ModelStateIsValid(model,
				m => Service.RecoverAsync(model.VerifyEmail, model.Password),
				m => View("SetPasswordSuccess"), 
				m => View(m.Fluent(x => ModelState.AddModelError("error", "Новый пароль должен отличатся от текущего"))));
		#endregion

		#region Register
		[HttpGet]
		public ActionResult Register()
			=> View(new RegisterModel());

		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<IActionResult> Register(RegisterModel model)
			=> ModelStateIsValid(model, 
				async m => await Service.RegisterAsync(m.Email, m.Name, m.Password) && await Login(await Service.TryLoginAsync(m.Email, m.Password)) != null,
				m => View("RegisterDone"),
				View);
		#endregion

		[Authorize]
		public async Task<IActionResult> Relogin(string returnUrl)
		{
			await Login(Service.GetUser<UserProjection>());

			return Redirect(returnUrl ?? "/");
		}

		private async Task<UserProjection> Login(UserProjection user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim("IP", Request.HttpContext.Connection.RemoteIpAddress.ToString(), ClaimValueTypes.String),
				//new Claim("permission-foo", "grant")
			};
			claims.AddRange((user.Roles ?? new[] { EUserRole.Anonym }).Select(x => new Claim(ClaimTypes.Role, x.ToString().ToLower())));

			var identity = new ClaimsIdentity("MyCookieAuthenticationScheme");
			identity.AddClaims(claims);

			var principal = new ClaimsPrincipal(identity);

			await HttpContext.SignInAsync("MyCookieAuthenticationScheme", principal, new AuthenticationProperties
			{
				ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
			});

			return user;
		}
	}
}