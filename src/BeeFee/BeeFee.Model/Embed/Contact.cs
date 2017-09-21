namespace BeeFee.Model.Embed
{
    public struct Contact
    {
		public string Name { get; }
		public string Email { get; }
		public string Phone { get; }

		public Contact(string name, string email, string phone)
		{
			Name = name;
			Email = email;
			Phone = phone;
		}
    }
}
