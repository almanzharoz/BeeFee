using System;
using BeeFee.Model.Projections;
using Core.ElasticSearch.Domain;

namespace BeeFee.Model.Exceptions
{
	public class EntityAccessException<T> : Exception where T : IEntity
	{
		public string User { get; }
		public string Id { get; }

		public EntityAccessException(UserName user, string id)
			: base($"Access dined for userId: \"{user.Id}\" to {typeof(T).Name} with Id: \"{id}\"")
		{
			User = user.Id;
			Id = id;
		}
	}
}