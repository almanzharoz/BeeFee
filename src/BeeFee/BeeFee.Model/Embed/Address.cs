
namespace BeeFee.Model.Embed
{
    public struct Address
    {
        public string City { get; private set; }
        public string AddressString { get; private set; }
        //public GeoCoordinate Coordinates { get; set; }

	    public Address(string city, string addressstring)
	    {
		    City = city;
		    AddressString = addressstring;
	    }
    }
}