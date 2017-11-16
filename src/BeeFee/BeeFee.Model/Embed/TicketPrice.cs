using System;
using Core.ElasticSearch;
using Nest;

namespace BeeFee.Model.Embed
{
    public class TicketPrice
    {
		[Keyword]
		public Guid Id { get; }
		public string Name { get; private set; }
		[Keyword(Index = false)]
		public string Description { get; private set; }
		public decimal Price { get; private set; }
		/// <summary>
		/// Общее количество билетов
		/// </summary>
		public int Count { get; private set; }
		/// <summary>
		/// Осталось билетов
		/// </summary>
		public int Left { get; private set; }
		/// <summary>
		/// HTML-шаблон билета для PDF
		/// </summary>
		public string TicketTemplate { get; private set; }

		public TicketPrice(string name, string description, string tickettemplate, decimal price, int count)
		{
			Id = Guid.NewGuid();
			Name = name;
			Description = description;
			Price = price;
			Count = count;
			Left = count;
			TicketTemplate = tickettemplate;
		}

		[DeserializeConstructor]
		public TicketPrice(Guid id, string name, string description, string tickettemplate, decimal price, int count, int left)
		{
			Id = id;
			Name = name;
			Description = description;
			Price = price;
			Count = count;
			Left = left;
			TicketTemplate = tickettemplate;
		}

		public TicketPrice Update(string name, string description, decimal price, int count, string template)
		{
			Name = name;
			Description = description;
			Price = price;
			Left = Left + (count - Count);
			if (Left < 0)
				throw new Exception("Отрицательное значение оставшихся билетов");
			Count = count;
			TicketTemplate = template;
			return this;
		}
	}
}
