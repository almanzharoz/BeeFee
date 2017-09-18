using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using SharpFuncExt;

namespace BeeFee.TestsApp.Projections
{
	internal class NewCompany : BaseNewEntity, IProjection<Company>
	{
		public string Url { get; }
		public string Name { get; }
		public CompanyUser[] Users { get; }

		public NewCompany(string url, string name, BaseUserProjection owner)
		{
			Name = name.HasNotNullArg(nameof(name));
			Url = url.IfNull(() => CommonHelper.UriTransliterate(name));
			Users = new [] { new CompanyUser(owner.HasNotNullArg(nameof(owner)), ECompanyUserRole.Owner) };
		}

	}
}