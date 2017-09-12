using Core.ElasticSearch.Domain;
using Nest;

namespace Core.ElasticSearch.Tests.Models
{
    public class User : BaseEntity, IModel, IProjection<User>, IGetProjection
    {
        public string Login { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }

	    public User(string id) : base(id)
	    {
	    }
    }

	public class NewUser : BaseNewEntity, IProjection<User>
	{
		public string Login { get; set; }
		public string Email { get; set; }
		public string Password { get; set; }
		public string Salt { get; set; }
	}
}