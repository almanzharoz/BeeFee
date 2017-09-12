using System.Linq;
using BeeFee.AdminApp.Services;
using BeeFee.Model.Embed;
using BeeFee.TestsApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.AdminApp.Tests
{
	[TestClass]
	public class UsersServiceTests : BaseTestClass<UserService>
	{
		public UsersServiceTests()
			: base(BuilderExtensions.AddBeefeeAdminApp, BuilderExtensions.UseBeefeeAdminApp, new []{ EUserRole.Admin })
		{
		}

		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void AddUserTest()
		{
			var userId = Service.AddUser("test@test", "test", new[] { EUserRole.Organizer});
			Assert.IsNotNull(userId);

			var user = Service.GetUser(userId);
			Assert.IsNotNull(user);
			Assert.AreEqual("test@test", user.Email);
			Assert.AreEqual("test", user.Name);
			Assert.IsFalse(user.Roles.Except(new[] { EUserRole.Organizer }).Any());
		}

		[TestMethod]
		public void DeleteUserTest()
		{
			var userId = Service.AddUser("test@test", "test", new[] { EUserRole.Organizer});
			Assert.IsNotNull(userId);

			Assert.IsTrue(Service.DeleteUser(userId));

			Assert.IsNull(Service.GetUser(userId));
		}

		[TestMethod]
		public void SearchUserByEmailTest()
		{
			var userId = Service.AddUser("test@mail.ru", "test", new[] { EUserRole.Organizer });
			Assert.IsNotNull(userId);

			var searched1 = Service.SearchUsersByEmail("test").SingleOrDefault();
			Assert.IsNotNull(searched1);
			Assert.AreEqual(searched1.Id, userId);

			var searched2 = Service.SearchUsersByEmail("TEST@MAIL.RU").SingleOrDefault();
			Assert.IsNotNull(searched2);
			Assert.AreEqual(searched2.Id, userId);

			var searched3 = Service.SearchUsersByEmail("@");
			Assert.IsTrue(searched3.Any(x => x.Email == "test@mail.ru"));

			var searched4 = Service.SearchUsersByEmail("132").SingleOrDefault();
			Assert.IsNull(searched4);
		}

		[TestMethod]
		public void EditUserText()
		{
			var userId = Service.AddUser("test@mail.ru", "test", new[] { EUserRole.Organizer });
			Assert.IsNotNull(userId);

			var edited = Service.EditUser(userId, "newmail@mail.ru", "newName", new[] { EUserRole.User });
			Assert.AreEqual(true, edited);

			var searched = Service.GetUser(userId);
			Assert.IsNotNull(searched);

			Assert.AreEqual(userId, searched.Id);
			Assert.AreEqual("newmail@mail.ru", searched.Email);
			Assert.AreEqual("newName", searched.Name);
			Assert.IsFalse(searched.Roles.Except(new[] { EUserRole.User }).Any());
		}

	}
}