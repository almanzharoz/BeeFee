using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using Nest;

namespace BeeFee.Model.Models

{
	public abstract class User : IModel, IWithName
    {
		public string Id { get; set; }
        [Keyword]
        public string Email { get; set; }
        [Keyword]
        public string Name { get; set; }
		[Keyword(Index = false, Store = true)]
        public string Password { get; set; }
		[Keyword(Index = false, Store = true)]
        public string Salt { get; set; }
        [Keyword]
        public EUserRole[] Roles { get; set; }

        [Keyword]
		public string VerifyEmail { get; set; }
		[Keyword]
		public string VerifyPhone { get; set; }
		[Keyword]
		public string Phone { get; set; }
	}
}
