using System;

namespace SharpFuncExt
{
    public static class StringFunc
    {
		public static bool IsNull(this string arg) => String.IsNullOrWhiteSpace(arg);
		public static bool NotNull(this string arg) => !String.IsNullOrWhiteSpace(arg);
	    public static int ToInt32(this string arg) => int.Parse(arg);
	    public static long ToInt64(this string arg) => long.Parse(arg);
	}
}
