using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using SharpFuncExt;

namespace BeeFee.Model.Helpers
{
    public class CommonHelper
    {
		private static readonly Regex EmailValidator = new Regex(@"^(?:[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+\.)*[\w\!\#\$\%\&\'\*\+\-\/\=\?\^\`\{\|\}\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!\.)){0,61}[a-zA-Z0-9]?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\[(?:(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\.){3}(?:[01]?\d{1,2}|2[0-4]\d|25[0-5])\]))$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

	    private static readonly Dictionary<char, string> TransliterationDict = new Dictionary<char, string>
	    {
		    { 'А', "A" },
		    { 'Б', "B" },
		    { 'В', "V" },
		    { 'Г', "G" },
		    { 'Д', "D" },
		    { 'Е', "E" },
		    { 'Ё', "Yo" },
		    { 'Ж', "Zh" },
		    { 'З', "Z" },
		    { 'И', "I" },
		    { 'Й', "Y" },
		    { 'К', "K" },
		    { 'Л', "L" },
		    { 'М', "M" },
		    { 'Н', "N" },
		    { 'О', "O" },
		    { 'П', "P" },
		    { 'Р', "R" },
		    { 'С', "S" },
		    { 'Т', "T" },
		    { 'У', "U" },
		    { 'Ф', "F" },
		    { 'Х', "Kh" },
		    { 'Ц', "Ts" },
		    { 'Ч', "Ch" },
		    { 'Ш', "Sh" },
		    { 'Щ', "Shch" },
		    { 'Ъ', "" },
			{ 'Ы', "Y" },
		    { 'Ь', "" },
		    { 'Э', "E" },
		    { 'Ю', "Yu" },
		    { 'Я', "Ya" },
		    { 'а', "a" },
		    { 'б', "b" },
		    { 'в', "v" },
		    { 'г', "g" },
		    { 'д', "d" },
		    { 'е', "e" },
		    { 'ё', "yo" },
		    { 'ж', "zh" },
		    { 'з', "z" },
		    { 'и', "i" },
		    { 'й', "y" },
		    { 'к', "k" },
		    { 'л', "l" },
		    { 'м', "m" },
		    { 'н', "n" },
		    { 'о', "o" },
		    { 'п', "p" },
		    { 'р', "r" },
		    { 'с', "s" },
		    { 'т', "t" },
		    { 'у', "u" },
		    { 'ф', "f" },
		    { 'х', "kh" },
		    { 'ц', "ts" },
		    { 'ч', "ch" },
		    { 'ш', "sh" },
		    { 'щ', "shch" },
		    { 'ъ', "" },
			{ 'ы', "y" },
		    { 'ь', "" },
		    { 'э', "e" },
		    { 'ю', "yu" },
		    { 'я', "ya" },
	    };


		public static bool IsValidEmail(string email)
		    => email.NotNullOrDefault(EmailValidator.IsMatch);

	    public static string Transliterate(string str, Func<char, bool> isRightChar = null)
	    {
			var builder = new StringBuilder();
		    foreach (var symbol in str)
		    {
			    if (isRightChar != null && !isRightChar(symbol))
				    continue;
			    builder.Append(TransliterationDict[symbol]);
		    }
		    return builder.ToString();
	    }

	    public static string UriTransliterate(string str)
	    {
		    str = str.ToLower().Trim().Replace("  ", " ").Replace("  ", " ").Replace(" ", "-");
		    return Transliterate(str, x => char.IsLetterOrDigit(x) || x == '-');
		}
	}
}
