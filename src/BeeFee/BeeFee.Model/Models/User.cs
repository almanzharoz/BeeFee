﻿using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using Nest;
using Newtonsoft.Json;

namespace BeeFee.Model.Models

{
	public abstract class User : BaseEntity, IModel, IWithName
    {
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

		protected User() : base(null)
		{
		}
	}
}
