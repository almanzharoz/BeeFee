using System;
using SharpFuncExt;

namespace BeeFee.Model.Embed
{
    public struct TicketPrice
    {
		public Guid Id { get; }
		public string Name { get; }
		public string Description { get; }
		public decimal Price { get; }
		/// <summary>
		/// Общее количество билетов
		/// </summary>
		public int Count { get; }
		/// <summary>
		/// Осталось билетов
		/// </summary>
		public int Left { get; }

		public TicketPrice(Guid id, string name, string description, decimal price, int count, int left)
		{
			Id = id.IfNull(Guid.NewGuid);
			Name = name;
			Description = description;
			Price = price;
			Count = count;
			Left = left;
		}
    }
}
