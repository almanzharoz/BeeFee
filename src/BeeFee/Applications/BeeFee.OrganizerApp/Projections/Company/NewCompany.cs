using BeeFee.Model.Embed;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.OrganizerApp.Projections.Company
{
	internal class NewCompany : BaseNewEntity, IProjection<Model.Models.Company>
	{
		public string Name { get; }
		public string Url { get; }
		public string Logo { get; }
		public CompanyUser[] Users { get; set; }


		public NewCompany(BaseUserProjection owner, string name, string url, string logo)
		{
			Name = name.HasNotNullArg(nameof(name));
			Url = url.IfNull(name, CommonHelper.UriTransliterate);
			Logo = logo;
			Users = new [] { new CompanyUser(owner.HasNotNullArg(nameof(owner)), ECompanyUserRole.Owner) };
		}

	}
}