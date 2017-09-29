using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeeFee.Model.Embed
{
	[JsonConverter(typeof(StringEnumConverter))]
	public enum EUserRole : byte
	{
		[EnumMember(Value = "anonym")]
		Anonym = 0,
		[EnumMember(Value = "admin")]
		Admin = 1,
		[EnumMember(Value = "organizer")]
		Organizer,
		[EnumMember(Value = "user")]
		User,
		[EnumMember(Value = "eventmoderator")]
		EventModerator
	}
}