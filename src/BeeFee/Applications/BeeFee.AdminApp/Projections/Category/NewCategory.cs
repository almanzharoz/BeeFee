using BeeFee.Model.Helpers;
using Core.ElasticSearch.Domain;
using SharpFuncExt;

namespace BeeFee.AdminApp.Projections.Category
{
	internal class NewCategory : BaseNewEntity, IProjection<Model.Models.Category>
	{
		public string Url { get; }
		public string Name { get; }

		public NewCategory(string url, string name)
		{
			Url = url.IfNull(name, CommonHelper.UriTransliterate);
			Name = name;
		}

	}
}