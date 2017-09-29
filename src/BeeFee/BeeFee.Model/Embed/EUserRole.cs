using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BeeFee.Model.Embed
{
	public static class RoleNames
	{
		public const string Anonym = "anonym";
		public const string Admin = "admin";
		public const string Organizer = "organizer";
		public const string User = "user";
		public const string EventModerator = "eventmoderator";
	}

	[JsonConverter(typeof(StringEnumConverter))]
	public enum EUserRole : byte
	{
		[EnumMember(Value = RoleNames.Anonym)]
		Anonym = 0,
		[EnumMember(Value = RoleNames.Admin)]
		Admin = 1,
		[EnumMember(Value = RoleNames.Organizer)]
		Organizer,
		[EnumMember(Value = RoleNames.User)]
		User,
		[EnumMember(Value = RoleNames.EventModerator)]
		EventModerator
	}
}