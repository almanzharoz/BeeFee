using System;
using BeeFee.JobsApp.Services;
using BeeFee.Model.Jobs.Data;
using BeeFee.TestsApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.JobsApp.Tests
{
    [TestClass]
    public class MailServiceTests : BaseTestClass<MailService>
	{
		public MailServiceTests() : base(BuilderExtensions.AddBeefeeJobsApp, BuilderExtensions.UseBeefeeJobsApp, null)
		{
		}

		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
			=> serviceCollection.AddSingleton(new MailServiceSettings(){PickupDirectory = "d:\\mails", From="test@mail.ru"});
			//=> serviceCollection.AddSingleton(new MailServiceSettings() { Host="mail.ru", Port = 465, Ssl = true, From = "test@mail.ru" });

		[TestMethod]
		public void SendEmailTest()
		{
			Assert.IsTrue(AddSendMailJob(new SendMail(null, "mail@mail.ru", "Привет!", "Hi", null), DateTime.UtcNow));
			Assert.IsTrue(Service.SendNextMail());
		}
	}
}
