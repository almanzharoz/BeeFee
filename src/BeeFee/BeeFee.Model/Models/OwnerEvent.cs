using System;
using Core.ElasticSearch.Domain;
using BeeFee.Model.Embed;
using BeeFee.Model.Interfaces;
using BeeFee.Model.Models;
using Nest;

namespace BeeFee.Model
{
	/// <summary>
	/// Настройки мероприятия, доступные только создателю
	/// </summary>
	public class OwnerEvent : BaseEntityWithVersion, IModel, IWithCreated
	{
        public string Email { get; set; }
        public DateTime Created { get; set; }

		public OwnerEvent(string id, int version) : base(id, version)
		{
		}
	}
}