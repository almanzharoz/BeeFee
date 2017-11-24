using Nest;

namespace BeeFee.Model.Embed
{
    public struct Contact
    {
		[Keyword]
		public string Name { get; }
		[Keyword]
		public string Email { get; }
		[Keyword]
		public string Phone { get; }

		public Contact(string name, string email, string phone)
		{
			Name = name;
			Email = email;
			Phone = phone;
		}
    }
}
