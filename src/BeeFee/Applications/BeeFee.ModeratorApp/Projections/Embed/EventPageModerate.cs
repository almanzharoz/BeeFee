using BeeFee.Model.Embed;

namespace BeeFee.ModeratorApp.Projections.Embed
{
	public struct EventPageModerate
	{
		public string Html { get; private set; }

		public string Title { get; private set; }

		public string Label { get; private set; }

		public string Caption { get; private set; }

		public string Cover { get; private set; }

		public string[] Images { get; private set; }

		public Address Address { get; private set; }

		public string Date { get; private set; }

		public string Category { get; private set; }

		public string Company { get; private set; }

		public void Change(string title, string label, string caption, Address address, string category, string html)
		{
			Title = title;
			Label = label;
			Caption = caption;
			Category = category;
			Html = html;
			Address = address;
		}
	}
}