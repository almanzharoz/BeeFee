using System.Threading.Tasks;
using BeeFee.ClientApp.Services;
using BeeFee.LoginApp.Projections.User;
using BeeFee.LoginApp.Services;
using BeeFee.Model.Embed;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models.Profile;
using SharpFuncExt;

namespace WebApplication3.Controllers
{
	[Authorize(Roles = RoleNames.User)]
	public class ProfileController : BaseController<AuthorizationService>
	{
		private readonly UserService _userService;

		public ProfileController(AuthorizationService service, UserService userService) : base(service)
		{
			_userService = userService;
		}

		#region Edit

		[HttpGet]
		public IActionResult Edit()
			=> View(new ProfileModel(Service.GetUser<UserProjection>()));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<IActionResult> Edit(ProfileModel model)
			=> ModelStateIsValid(model, m => _userService.UpdateUserAsync(m.Name), m => View("EditError"));

		#endregion

		#region ChangePassword

		[HttpGet]
		public IActionResult ChangePassword() => View(new ChangePasswordModel());

		[HttpPost]
		[ValidateAntiForgeryToken]
		public Task<IActionResult> ChangePassword(ChangePasswordModel model)
			=> ModelStateIsValid(model,
				m => Service.ChangePasswordAsync(m.OldPassword, m.Password),
				m => View("ChangePasswordSuccess"),
				m => View(m.Fluent(x => ModelState.AddModelError("error", "Новый пароль должен отличатся от текущего"))));

		#endregion

		#region Registrations

		[HttpGet]
		public async Task<IActionResult> Registrations(RegistrationsFilter model)
			=> View(model.Load(await _userService.GetRegistrations(model.Page, model.Size)));

		#endregion

	}
}