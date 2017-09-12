using System;
using System.Linq;
using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Services;
using BeeFee.TestsApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.OrganizerApp.Tests
{
    [TestClass]
    public class EventServiceTests : BaseTestClass<EventService>
	{
		public EventServiceTests() 
			: base(BuilderExtensions.AddBeefeeOrganizerApp, BuilderExtensions.UseBeefeeOrganizerApp, new [] { EUserRole.Organizer })
		{
		}

		[TestInitialize]
		public override void Setup()
			=> base.Setup();

		[TestMethod]
		public void AddEventTest()
		{
			var id = AddCategory("test");
			Assert.IsTrue(Service.AddEvent(id, "test", "test", null, EEventType.Concert,
				new EventDateTime(DateTime.Now, DateTime.Now.AddMinutes(20)), new Address(), new TicketPrice[0], "html"));
		}

		[TestMethod]
		public void RemoveEventTest()
		{
			var id = AddCategory("test");
			Assert.IsTrue(Service.AddEvent(id, "test", "test", null, EEventType.Concert,
				new EventDateTime(DateTime.Now, DateTime.Now.AddMinutes(20)), new Address(), new TicketPrice[0], "html"));
			var myEvents = Service.GetMyEvents().ToList();
			Assert.AreEqual(1, myEvents.Count);
			Assert.IsTrue(Service.RemoveEvent(myEvents[0].Id, myEvents[0].Version));
			myEvents = Service.GetMyEvents().ToList();
			Assert.AreEqual(0, myEvents.Count);
		}

		[TestMethod]
		public void SaveAllDataTest()
		{
			var id = AddCategory("test");
			var start = DateTime.Now;
			var end = start.AddMinutes(20);
			Assert.IsTrue(Service.AddEvent(id, "test", "test", null, EEventType.Concert, new EventDateTime(start, end),
				new Address("Ekaterinburg", "asd"), new[] {new TicketPrice {Price = 100m}}, "html"));
			var @event = Service.GetMyEvents().ToList()[0];
			Assert.AreEqual("test", @event.Name);
			Assert.AreEqual("test", @event.Url);
			Assert.AreEqual(EEventType.Concert, @event.Type);
			Assert.AreEqual(start, @event.DateTime.Start);
			Assert.AreEqual(end, @event.DateTime.Finish);
			Assert.AreEqual("Ekaterinburg", @event.Address.City);
			Assert.AreEqual("asd", @event.Address.AddressString);
			Assert.AreEqual(100m, @event.Prices[0].Price);
			Assert.AreEqual("html", @event.Page.Html);
		}

		[TestMethod]
		public void UpdateEventTest()
		{
			var id = AddCategory("test");
			var start = DateTime.Now;
			var end = start.AddMinutes(20);
			Assert.IsTrue(Service.AddEvent(id, "test", "test", null, EEventType.Concert, new EventDateTime(start, end),
				new Address("Ekaterinburg", "asd"), new[] { new TicketPrice { Price = 100m } }, "html"));
			var @event = Service.GetMyEvents().ToList()[0];
			Assert.AreEqual("test", @event.Name);
			Assert.AreEqual("test", @event.Url);
			Assert.AreEqual(EEventType.Concert, @event.Type);
			Assert.AreEqual(start, @event.DateTime.Start);
			Assert.AreEqual(end, @event.DateTime.Finish);
			Assert.AreEqual("Ekaterinburg", @event.Address.City);
			Assert.AreEqual("asd", @event.Address.AddressString);
			Assert.AreEqual(100m, @event.Prices[0].Price);
			Assert.AreEqual("html", @event.Page.Html);

			start = DateTime.Now;
			end = start.AddMinutes(20);
			Assert.IsTrue(Service.UpdateEvent(@event.Id, @event.Version, "asd", "asd", null, new EventDateTime(start, end),
				new Address("Ekaterinburg", "dsa"), EEventType.Excursion, id, new TicketPrice[0], "asd"));
			@event = Service.GetMyEvents().ToList()[0];
			Assert.AreEqual("asd", @event.Name);
			Assert.AreEqual("asd", @event.Url);
			Assert.AreEqual(EEventType.Excursion, @event.Type);
			Assert.AreEqual(start, @event.DateTime.Start);
			Assert.AreEqual(end, @event.DateTime.Finish);
			Assert.AreEqual("Ekaterinburg", @event.Address.City);
			Assert.AreEqual("dsa", @event.Address.AddressString);
			Assert.AreEqual(0, @event.Prices.Length);
			Assert.AreEqual("asd", @event.Page.Html);
		}
	}
}
