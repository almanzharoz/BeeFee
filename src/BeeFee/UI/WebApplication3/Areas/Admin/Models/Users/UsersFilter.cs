using BeeFee.AdminApp.Projections.User;
using Core.ElasticSearch;
using WebApplication3.Models;

namespace WebApplication3.Areas.Admin.Models.Users
{
	public class UsersFilter : PagerFilter<UserProjection>
	{
		public UsersFilter() : base(10)
		{
		}

		public UsersFilter Load(Pager<UserProjection> items)
		{
			LoadItems(items);
			return this;
		}
	}
}