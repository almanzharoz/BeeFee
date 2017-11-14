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
		public ProfileController(AuthorizationService service) : base(service)
		{
		}

		#region Edit

		[HttpGet]
		public IActionResult Edit()
			=> View(new ProfileModel(Service.GetUser<UserProjection>()));

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ProfileModel model)
			=> ModelStateIsValid(model, m => Service.UpdateUser(m.Name), m => View("EditError"), m => View(m));

		#endregion

		#region ChangePassword

		[HttpGet]
		public IActionResult ChangePassword() => View(new ChangePasswordModel());

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult ChangePassword(ChangePasswordModel model)
			=> ModelStateIsValid(model,
				m => Service.ChangePassword(m.OldPassword, m.Password),
				m => View("ChangePasswordSuccess"),
				m => View(m.Fluent(x => ModelState.AddModelError("error", "Новый пароль должен отличатся от текущего"))));

		#endregion

		#region Registrations

		[HttpGet]
		public ActionResult Registrations(object model)
		{
			return View();
		}

		#endregion

	}
}