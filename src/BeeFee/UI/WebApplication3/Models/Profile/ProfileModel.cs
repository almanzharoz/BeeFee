using BeeFee.LoginApp.Projections.User;

namespace WebApplication3.Models.Profile
{
	public class ProfileModel
	{
		public string Name { get; set; }

		public ProfileModel() { }

		public ProfileModel(UserProjection user)
		{
			Name = user.Name;
		}

	}
}