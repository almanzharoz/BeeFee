using BeeFee.Model.Embed;
using Nest;

namespace BeeFee.Model
{
	/// <summary>
	/// Настройки мероприятия, отображаемые посетителю
	/// </summary>
	public struct EventPage
	{
		/// <summary>
		/// Внутренний HTML страницы мероприятия
		/// </summary>
		public string Html { get; private set; }
		/// <summary>
		/// Заголовок title страницы
		/// </summary>
		[Keyword(Index = false)]
		public string Title { get; private set; }
		public string Caption { get; private set; }
		[Keyword(Index = false)]
		public string Cover { get; private set; }
		/// <summary>
		/// Галерея
		/// </summary>
		[Keyword(Index = false)]
		public string[] Images { get; private set; }

		public Address Address { get; private set; }
		[Keyword(Index = false)]
		public string Date { get; private set; }
		[Keyword(Index = false)]
		public string Category { get; private set; }
		[Keyword(Index = false)]
		public string Company { get; private set; }

		public EventPage SetHtml(string html)
		{
			Html = html;
			return this;
		}

		public EventPage(string caption, string category, string cover, string date, Address address, string html)
		{
			Title = caption;
			Caption = caption;
			Category = category;
			Cover = cover;
			Address = address;
			Date = date;
			Html = html;

			Company = null;
			Images = null;
		}

		public EventPage Change(string caption, string category, string cover, EventDateTime date, Address address)
		{
			Title = caption;
			Caption = caption;
			Category = category;
			Cover = cover;
			Address = address;
			Date = date.ToString();
			return this;
		}
	}
}