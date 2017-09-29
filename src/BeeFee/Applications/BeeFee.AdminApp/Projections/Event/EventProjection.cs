using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;
using Nest;

namespace BeeFee.AdminApp.Projections.Event
{
	public class EventProjection : BaseEntityWithParentAndVersion<BaseCompanyProjection>, IProjection<Model.Models.Event>, IGetProjection, ISearchProjection, IRemoveProjection, IUpdateProjection, IWithName, IWithOwner
	{
		[Keyword]
		public BaseCategoryProjection Category { get; private set; }
		public string Name { get; }
		public EventDateTime DateTime { get; }
		public Address Address { get; }
		public EEventState State { get; }

		[Keyword]
		public BaseUserProjection Owner { get; }

		internal EventProjection ChangeCategory(BaseCategoryProjection newCategory)
		{
			Category = newCategory;
			return this;
		}

		public EventProjection(string id, BaseCompanyProjection parent, int version, BaseUserProjection owner, BaseCategoryProjection category, string name, EventDateTime datetime, Address address, EEventState state) : base(id, parent, version)
		{
			Owner = owner;
			Category = category;
			Name = name;
			DateTime = datetime;
			Address = address;
			State = state;
		}
	}
}