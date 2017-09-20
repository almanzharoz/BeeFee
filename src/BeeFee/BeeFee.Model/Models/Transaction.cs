using BeeFee.Model.Embed;
using Core.ElasticSearch.Domain;
using System;

namespace BeeFee.Model.Models
{
	//TODO: Переопределить проекции для изменения статуса и для создания
	public abstract class Transaction
	{
		public DateTime Date { get; }
		public Contact Contact { get; }
		public float Sum { get; }
		public ETransactionType Type { get; }
		public ETransactionState State { get; private set; }
	}
}