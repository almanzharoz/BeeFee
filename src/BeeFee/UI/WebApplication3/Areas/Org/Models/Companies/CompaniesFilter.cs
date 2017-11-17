using System.Collections.Generic;
using BeeFee.OrganizerApp.Projections.Company;

namespace WebApplication3.Areas.Org.Models.Companies
{
	public class CompaniesFilter
	{
		public KeyValuePair<CompanyProjection, int>[] Items { get; private set; }

		public CompaniesFilter Load(KeyValuePair<CompanyProjection, int>[] items)
		{
			Items = items;
			return this;
		}
	}
}