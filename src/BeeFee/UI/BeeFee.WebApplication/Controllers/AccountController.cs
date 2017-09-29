using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BeeFee.LoginApp.Projections.User;
using BeeFee.LoginApp.Services;
using BeeFee.Model.Embed;
using BeeFee.Model.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using BeeFee.WebApplication.Models.Account;
using Microsoft.AspNetCore.Authorization;

namespace BeeFee.WebApplication.Controllers
{
    public class AccountController : BaseController<AuthorizationService>
    {
	    public AccountController(AuthorizationService service, CategoryService categoryService) : base(service, categoryService)
	    {
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

			var claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Name),
			    new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim("IP", Request.Host.Host, ClaimValueTypes.String),
				new Claim("permission-foo", "grant")
			};
			claims.AddRange((user.Roles ?? new[] { EUserRole.Anonym }).Select(x => new Claim(ClaimTypes.Role, x.ToString().ToLower())));

            var identity = new ClaimsIdentity("MyCookieAuthenticationScheme");
            identity.AddClaims(claims);

            var principal = new ClaimsPrincipal(identity);

	        await HttpContext.SignInAsync("MyCookieAuthenticationScheme", principal, new AuthenticationProperties
	        {
		        ExpiresUtc = DateTime.UtcNow.AddMinutes(20)
	        });

			//_logger.LogInformation(4, "User logged in.");

			return IsAjax ? (IActionResult)Json(new {url = vm.ReturnUrl}) : Redirect(vm.ReturnUrl ?? "/");
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
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = Service.Register(model.Email, model.Name, model.Password);
                switch (result)
                {
                    case UserRegistrationResult.Ok:
                        model.Result = "Пользователь успешно зарегистрирован.";
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

		[Authorize]
		[HttpGet]
		public IActionResult Profile()
		{
			return View(new ProfileEditModel(Service.GetUser<UserProjection>()));
		}
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