using System;
using System.Linq;
using BeeFee.Model;
using BeeFee.Model.Embed;
using BeeFee.OrganizerApp.Services;
using BeeFee.TestsApp;
using Core.ElasticSearch;
using Core.ElasticSearch.Mapping;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFee.OrganizerApp.Tests
{
	[TestClass]
	public class CompanyServiceTests : BaseTestClass<CompanyService>
	{
		public CompanyServiceTests() 
			: base(BuilderExtensions.AddBeefeeOrganizerApp, BuilderExtensions.UseBeefeeOrganizerApp, new[] { EUserRole.Organizer })
		{
		}

		[TestInitialize]
		public override void Setup()
			=> base.Setup();

		[TestMethod]
		public void AddCompany()
		{
			Assert.IsTrue(Service.AddCompany("test comapny", "test"));

			var loaded = Service.GetMyCompanies();
			Assert.IsTrue(loaded.Any());
			Assert.AreEqual("test comapny", loaded.First().Key.Name);
			Assert.AreEqual("test", loaded.First().Key.Url);
		}
	}
}