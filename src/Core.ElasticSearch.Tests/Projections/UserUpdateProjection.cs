using Core.ElasticSearch.Domain;
using Newtonsoft.Json;

namespace Core.ElasticSearch.Tests.Projections
{
    public class UserUpdateProjection: BaseEntityWithVersion, IProjection<Models.User>, IGetProjection, IUpdateProjection
    {
        public string Login { get; private set; }
        public string Email { get; set; }
        public string Password { get; set; }

	    public UserUpdateProjection(string id) : base(id)
	    {
	    }

	    public UserUpdateProjection(string id, int version) : base(id, version)
	    {
	    }
    }
}