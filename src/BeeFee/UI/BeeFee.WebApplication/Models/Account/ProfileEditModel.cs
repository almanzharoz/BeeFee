using BeeFee.LoginApp.Projections.User;

namespace BeeFee.WebApplication.Models.Account
{
	public class ProfileEditModel
	{
		public string Name { get; set; }

		public ProfileEditModel() { }

		public ProfileEditModel(UserProjection user)
		{
			Name = user.Name;
		}
	}
}