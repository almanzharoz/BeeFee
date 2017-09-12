using Core.ElasticSearch.Domain;
using BeeFee.Model.Helpers;
using BeeFee.Model.Models;
using SharpFuncExt;

namespace BeeFee.TestsApp.Projections
{
	internal class NewCategory : BaseNewEntity, IProjection<Category>
	{
		public string Url { get; }
		public string Name { get; }

		public NewCategory() { } //Hack for where new()

		public NewCategory(string url, string name)
		{
			Name = name.HasNotNullArg(nameof(name));
			Url = url.IfNull(() => CommonHelper.UriTransliterate(name));
		}

	}
}