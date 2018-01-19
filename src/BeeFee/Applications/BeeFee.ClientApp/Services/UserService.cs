using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeeFee.ClientApp.Projections.Event;
using BeeFee.ClientApp.Projections.User;
using BeeFee.Model;
using BeeFee.Model.Models;
using BeeFee.Model.Projections;
using Core.ElasticSearch;
using Microsoft.Extensions.Logging;

namespace BeeFee.ClientApp.Services
{
	public class UserService : BaseBeefeeService
	{
		public UserService(ILoggerFactory loggerFactory, BeefeeElasticConnection settings, ElasticScopeFactory<BeefeeElasticConnection> factory, UserName user) 
			: base(loggerFactory, settings, factory, user)
		{
		}

		public Task<bool> UpdateUserAsync(string name)
			=> UpdateByIdAsync<UserUpdateProjection>(User.Id, x => x.ChangeName(name), true);

		public Task<Pager<EventTicketTransaction>> GetRegistrations(int page, int take)
			=> FilterNestedAsync<EventTransaction, TicketTransaction, EventTransactionProjection, EventTicketTransaction>(
				q => q.MatchAll()/*Term(p => p.Transactions.First().User, User.Id)*/,
				p => p.Transactions,
				q => q.Term(p => p.Transactions.First().User, User.Id),
				s => s.Descending(p => p.Transactions.First().Date),
				page, take);
	}
}