using BeeFee.ModeratorApp.Services;
using BeeFee.TestsApp;
using System;
using BeeFee.Model.Embed;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace BeeFee.ModeratorApp.Tests
{
	[TestClass]
	public class EventModeratorServiceTests : BaseTestClass<EventModeratorService>
	{
		public EventModeratorServiceTests() 
			: base(BuilderExtensions.AddBeefeeModeratorApp, BuilderExtensions.UseBeefeeModeratorApp, new EUserRole[] { EUserRole.User, EUserRole.EventModerator })
		{
		}

		[TestInitialize]
		public override void Setup()
			=> base.Setup();

		[TestMethod]
		public void ModerateEvent()
		{
			var id = AddCategory("test");
			var cid = AddCompany("test");
			var eventId = AddEvent(cid, id, "test", new EventDateTime(DateTime.Now, DateTime.Now.AddMinutes(20)), EEventState.Moderating);

			Assert.IsTrue(Service.GetEvents(0, 1).Any());
			Assert.IsTrue(Service.ModerateEvent(eventId, cid, 1, true));
			Assert.IsFalse(Service.GetEvents(0, 1).Any());
		}
	}
}
