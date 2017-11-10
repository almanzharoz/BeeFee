using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeeFee.LoginApp.Projections.User;
using BeeFee.LoginApp.Services;
using BeeFee.Model.Embed;
using BeeFee.Model.Exceptions;
using BeeFee.Model.Services;
using BeeFee.WebApplication.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using BeeFee.WebApplication.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using SharpFuncExt;

namespace BeeFee.WebApplication.Controllers
{
    public class AccountController : BaseController<AuthorizationService>
	{
		private readonly BeeFeeWebAppSettings _settings;

		public AccountController(AuthorizationService service, CategoryService categoryService, BeeFeeWebAppSettings settings) : base(service, categoryService)
		{
			_settings = settings;
		}

		[HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel() { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
	        if (vm.Login == null || vm.Pass == null)
		        return TryAjaxView("_LoginForm", vm);

            var user = Service.TryLogin(vm.Login.Trim().ToLower(), vm.Pass.Trim());
			if (user == null)
			{
				ModelState.AddModelError("error", "Неверный логин или пароль");
				return TryAjaxView("_LoginForm", vm);
			}

			await Login(user);

			//_logger.LogInformation(4, "User logged in.");

			return IsAjax ? (IActionResult)Json(new {url = vm.ReturnUrl}) : Redirect(vm.ReturnUrl ?? "/");
        }

		private async Task<UserProjection> Login(UserProjection user)
		{
			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Name),
				new Claim(ClaimTypes.Email, user.Email),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim("IP", Request.Host.Host, ClaimValueTypes.String),
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

		public async Task<IActionResult> Relogin(string returnUrl)
		{
			var user = Service.GetUser<UserProjection>();

			await Login(user);

			return Redirect(returnUrl ?? "/");
		}

		[Authorize]
        [HttpGet]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync("MyCookieAuthenticationScheme");

			//_logger.LogInformation(4, "User logged out.");

			return Redirect(Request.Headers["Referer"].FirstOrDefault() ?? "/");
        }

        public IActionResult AccessDenied()
        {
            return Content("Access Denied");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = Service.Register(model.Email, model.Name, model.Password);
                switch (result.Item1)
                {
                    case UserRegistrationResult.Ok:
                        model.Result = "Пользователь успешно зарегистрирован.";
						await Login(result.Item2);
                        break;
                    case UserRegistrationResult.EmailAlreadyExists:
                        ModelState.AddModelError("", "Указанный email уже существует");
                        break;
                    case UserRegistrationResult.NameIsEmpty:
                    case UserRegistrationResult.EmailIsEmpty:
                    case UserRegistrationResult.UnknownError:
                    case UserRegistrationResult.PasswordIsEmpty:
                    case UserRegistrationResult.WrongEmail:
                        ModelState.AddModelError("", "При регистрации пользователя произошла неизвестная ошибка");
                        break;
                }
            }
            return View(model);
        }

		[HttpGet]
		public IActionResult Recover()
			=> TryAjaxView("Recover", new RecoverEditModel());

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Recover(RecoverEditModel model)
			=> ModelStateIsValid(model, m => Service.RecoverLink(m.Email, _settings.WebAppUrl), 
				m => TryAjaxView("RecoverDone", m),
				m => TryAjaxView("Recover", m));

		[HttpGet]
		public IActionResult SetPassword(string id)
			=> View(new SetPasswordEditModel(id.Fluent(x => Service.VerifyEmailForRecover(x)
				.ThrowIfNull<UserProjection, NotFoundException>())));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult SetPassword(SetPasswordEditModel model)
			=> ModelStateIsValid(model,
				m => Service.Recover(model.VerifyEmail, model.Password),
				m => View("SetPasswordSuccess"), m => View(m.Fluent(x => ModelState.AddModelError("error", "Новый пароль должен отличатся от текущего"))));

		[Authorize]
		[HttpGet]
		public IActionResult ChangePassword() => View(new ChangePasswordEditModel());

		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ChangePassword(ChangePasswordEditModel model)
			=> ModelStateIsValid(model, m => Service.ChangePassword(m.OldPassword, m.Password), m => View("ChangePasswordSuccess"), m => View(m.Fluent(x => ModelState.AddModelError("error", "Новый пароль должен отличатся от текущего"))));

		[Authorize]
		[HttpGet]
		public IActionResult Profile()
			=>  View(new ProfileEditModel(Service.GetUser<UserProjection>()));

		[Authorize]
		[HttpPost]
        [ValidateAntiForgeryToken]
		public IActionResult Profile(ProfileEditModel model)
		{
			if (ModelState.IsValid)
				Service.UpdateUser(model.Name);
			return View(model);
		}
	}
}