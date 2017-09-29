using System;
using System.IO;
using BeeFee.JobsApp.Services;
using BeeFee.Model.Jobs.Data;
using BeeFee.TestsApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.JobsApp.Tests
{
	[TestClass]
	public class TicketServiceTests : BaseTestClass<TicketService>
	{
		public TicketServiceTests() : base(BuilderExtensions.AddBeefeeJobsApp, BuilderExtensions.UseBeefeeJobsApp, null)
		{
		}

		[TestInitialize]
		public override void Setup()
		{
			base.Setup();
		}

		protected override IServiceCollection AddServices(IServiceCollection serviceCollection)
			=> serviceCollection.AddSingleton(new TicketServiceSettings() { Folder = "d:\\pdf", Url = "http://localhost:58219" });

		[TestMethod]
		public void CreateTicketTest()
		{
			var filename = Guid.NewGuid().ToString();
			Assert.IsTrue(AddCreateTicketJob(new CreateTicket("My name", "event date", filename), DateTime.UtcNow));
			var r = Service.CreateNextTicket();
			r.Wait();
			Assert.IsTrue(r.Result);
			Assert.IsTrue(File.Exists(Path.Combine("d:\\pdf\\", filename+".pdf")));
		}
	}
}
