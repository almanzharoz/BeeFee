using System;
using System.Linq;
using BeeFee.AdminApp.Services;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.Model.Models;
using BeeFee.TestsApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.AdminApp.Tests
{
	[TestClass]
	public class EventServiceTests : BaseTestClass<EventService>
	{
		public EventServiceTests()
			: base(BuilderExtensions.AddBeefeeAdminApp, BuilderExtensions.UseBeefeeAdminApp, new[] { EUserRole.Admin })
		{
		}

		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		[TestMethod]
		public void RemoveEventTest()
		{
			var cid = AddCompany("test");
			var eventId = AddEvent(cid, AddCategory("Category 1"), "Event 1", new EventDateTime(DateTime.Now, DateTime.Now.AddDays(1)));

			Assert.IsNotNull(eventId);

			Assert.IsTrue(Service.RemoveEvent(eventId, cid, 1));
		}

		[TestMethod]
		public void SetCategoryTest()
		{
			var cid = AddCompany("test");
			var eventId = AddEvent(cid, AddCategory("Category 1"), "Event 1", new EventDateTime(DateTime.Now, DateTime.Now.AddDays(1)));
			Assert.IsNotNull(eventId);
			var categoryId = AddCategory("Category 2");
			Assert.IsNotNull(categoryId);

			Assert.IsTrue(Service.SetCategoryToEvent(eventId, cid, categoryId, 1));
		}

		[TestMethod]
		public void SerachEvent()
		{
			var eventId = AddEvent(AddCompany("test"), AddCategory("Category 1"), "Event 1", new EventDateTime(DateTime.Now, DateTime.Now.AddDays(1)));
			Assert.IsNotNull(eventId);

			var searched = Service.SearchByName("Event 1").SingleOrDefault();

			Assert.IsNotNull(searched);
			Assert.AreEqual(searched.Id, eventId);
		}

	}
}

