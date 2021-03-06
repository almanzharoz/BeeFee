using System;
using System.Linq;
using BeeFee.ClientApp.Services;
using BeeFee.Model.Embed;
using BeeFee.TestsApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.ClientApp.Tests
{
    [TestClass]
    public class EventServiceTests : BaseTestClass<EventService>
    {
        public EventServiceTests()
            : base(BuilderExtensions.AddBeefeeClientApp, BuilderExtensions.UseBeefeeClientApp, new[] { EUserRole.User })
        {
        }

        [TestInitialize]
        public override void Setup()
        {
            base.Setup();
        }

        [TestMethod]
        public void GetEventPage()
        {
            var dateTimeNow = DateTime.UtcNow;
            var eventId = AddEvent(AddCompany("test"), AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)));

            Assert.IsNotNull(eventId);

            var @event = Service.GetEventByUrl("test", "event-1").Result;

            Assert.AreEqual(@event.Page.Caption, "Event 1");
            Assert.AreEqual(@event.Page.Category, "Category 1");
        }

        [TestMethod]
        public void SearchByDate()
        {
            var dateTimeNow = DateTime.UtcNow;
			var cid = AddCompany("test");

			AddEvent(cid, AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 0);
            AddEvent(cid, AddCategory("Category 2"), "Event 2", new EventDateTime(dateTimeNow.AddDays(10), dateTimeNow.AddDays(11)), EEventState.Open, new Address("Sverdlovsk", ""), 0);

            var events = Service.SearchEvents(startDateTime: dateTimeNow.AddDays(-1), endDateTime: dateTimeNow.AddDays(1));
			events.Wait();
            Assert.IsTrue(events.Result.Any());

            events = Service.SearchEvents(startDateTime: dateTimeNow.AddDays(1), endDateTime: dateTimeNow.AddDays(2));

            Assert.IsTrue(!events.Result.Any());

            events = Service.SearchEvents(startDateTime: dateTimeNow.AddDays(-3), endDateTime: dateTimeNow.AddDays(-2));

            Assert.IsTrue(!events.Result.Any());
        }

        [TestMethod]
        public void SearchByText()
        {
            var dateTimeNow = DateTime.UtcNow;
			var cid = AddCompany("test");
            AddEvent(cid, AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 0);
            AddEvent(cid, AddCategory("Category 2"), "Event 2", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Sverdlovsk", ""), 0);

            var events = Service.SearchEvents("Event 1");
			events.Wait();

			Assert.IsTrue(events.Result.Any());

            events = Service.SearchEvents("bla-bla");

            Assert.IsTrue(!events.Result.Any());
        }

        [TestMethod]
        public void ValidateSearchOnlyOpenOrClose()
        {
            var dateTimeNow = DateTime.UtcNow;
            var cid = AddCompany("test");
            var eventId1= AddEvent(cid, AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Created, new Address("Yekaterinburg", ""), 0);
            var eventId2 = AddEvent(cid, AddCategory("Category 2"), "Event 2", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Sverdlovsk", ""), 0);
            var eventId3 = AddEvent(cid, AddCategory("Category 3"), "Event 3", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Close, new Address("Moskva", ""), 0);

            var events = Service.SearchEvents();
            events.Wait();

            Assert.IsTrue(events.Result.All(e => e.Id != eventId1));

            Assert.IsTrue(events.Result.Any(e => e.Id == eventId2));

            Assert.IsTrue(events.Result.Any(e => e.Id == eventId3));
        }

        [TestMethod]
        public void SearchByCity()
        {
            var dateTimeNow = DateTime.UtcNow;
			var cid = AddCompany("test");
            AddEvent(cid, AddCategory("Category 1"), "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 0);
            AddEvent(cid, AddCategory("Category 2"), "Event 2", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Sverdlovsk", ""), 0);

            var events = Service.SearchEvents(city: "Yekaterinburg");
			events.Wait();

			Assert.IsTrue(events.Result.Any());

            events = Service.SearchEvents(city: "Moscow");

            Assert.IsTrue(!events.Result.Any());
        }

        [TestMethod]
        public void SearchByCategory()
        {
            var dateTimeNow = DateTime.UtcNow;
            var category1Id = AddCategory("Category 1");
            var category2Id = AddCategory("Category 2");
			var cid = AddCompany("test");
            var eventid1 = AddEvent(cid, category1Id, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 0);
            AddEvent(cid, category2Id, "Event 3", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Sverdlovsk", ""), 0);

            var events = Service.SearchEvents(categories: new [] { category1Id });
			events.Wait();

			Assert.AreEqual(events.Result.Count, 1);

            Assert.IsTrue(events.Result.FirstOrDefault().Id == eventid1);

            events = Service.SearchEvents(categories: new [] { "bla-bla" });

            Assert.IsTrue(!events.Result.Any());
        }

        //����� ��������� �� �����/�������? � ��� ������ ������?
        [TestMethod]
        public void SearchByMaxPrice()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = AddCategory("Category 1");
			var cid = AddCompany("test");
            AddEvent(cid, categoryId, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 1);
            AddEvent(cid, categoryId, "Event 2", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 7);

            var events = Service.SearchEvents(maxPrice: 5);
			events.Wait();

			Assert.AreEqual(events.Result.Count, 1);

            events = Service.SearchEvents(maxPrice: -2);

            Assert.IsTrue(!events.Result.Any());
        }

        [TestMethod]
        public void SearchByMaxPriceOrFree()
        {
            var dateTimeNow = DateTime.UtcNow;
            var categoryId = AddCategory("Category 1");
			var cid = AddCompany("test");
            AddEvent(cid, categoryId, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 0);
            AddEvent(cid, categoryId, "Event 2", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 7);

            var events = Service.SearchEvents(maxPrice: 5);
			events.Wait();

			Assert.AreEqual(events.Result.Count, 1);

            events = Service.SearchEvents(maxPrice: -2);

            Assert.IsTrue(events.Result.Any());
        }

        [TestMethod]
        public void GetAllCities()
        {
            var dateTimeNow = DateTime.UtcNow;
            var category1Id = AddCategory("Category 1");
			var cid = AddCompany("test");

			AddEvent(cid, category1Id, "Event 1", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 0);
            AddEvent(cid, category1Id, "Event 2", new EventDateTime(dateTimeNow, dateTimeNow.AddDays(1)), EEventState.Open, new Address("Moscow", ""), 0);

            var cities = Service.GetAllCities().Where(c => !string.IsNullOrEmpty(c)).ToList();

            Assert.AreEqual(cities.Count, 2);

            Assert.IsTrue(cities.Any(c => c.Equals("Yekaterinburg", StringComparison.OrdinalIgnoreCase)));
        }

		[TestMethod]
		public void RegisterToEvent()
		{
			var categoryId = AddCategory("Category 1");
			var companyId = AddCompany("test");

			var eventId = AddEvent(companyId, categoryId, "Event 1", new EventDateTime(DateTime.Now, DateTime.Now.AddDays(1)), EEventState.Open, new Address("Yekaterinburg", ""), 0, 1);

			var e = Service.GetEventByUrl("test", "event-1");
			e.Wait();
			Assert.IsNotNull(e.Result);
			Assert.AreEqual(e.Result.Id, eventId);
			Assert.AreEqual(e.Result.Prices.First().Left, 1);

			Assert.IsTrue(Service.RegisterToEvent(eventId, companyId, "test@email.ru", "my name", "12345", e.Result.Prices.First().Id));

			var fe = GetEventTransactionById(eventId, companyId);
			Assert.AreEqual(fe.TicketsLeft, 0);
			Assert.AreEqual(fe.Prices.First().Left, 0);
			Assert.AreEqual(fe.Transactions.Count(), 1);
			Assert.AreEqual(fe.Transactions.First().State, ETransactionState.New);

			Assert.IsFalse(Service.RegisterToEvent(eventId, companyId, "test@email.ru", "my name", "12345", e.Result.Prices.First().Id));

			var fe2 = GetEventTransactionById(eventId, companyId);
			Assert.IsFalse(Object.ReferenceEquals(fe, fe2));
			Assert.AreEqual(fe2.TicketsLeft, 0);
			Assert.AreEqual(fe2.Prices.First().Left, 0);
			Assert.AreEqual(fe2.Transactions.Count(), 1);
			Assert.AreEqual(fe2.Transactions.First().State, ETransactionState.New);
		}
	}
}
