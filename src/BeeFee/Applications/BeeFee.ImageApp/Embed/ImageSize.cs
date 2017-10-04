using System;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace BeeFee.ImageApp
{
	public struct ImageSize
	{
		[JsonProperty]
		public int Width { get; }
		[JsonProperty]
		public int Height { get; }

		private const char Delimiter = 'x';

		public ImageSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		internal ImageSize(string resolution)
		{
			var sizes = resolution.Split(Delimiter).Select(int.Parse).ToArray();
			if(sizes.Length != 2) throw new ArgumentException();
			Width = sizes[0];
			Height = sizes[1];
		}

		public override string ToString()
		{
			return $"{Width}{Delimiter}{Height}";
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ImageSize))
			{
				return false;
			}

			var size = (ImageSize)obj;
			return Width == size.Width &&
				   Height == size.Height;
		}

		public override int GetHashCode()
		{
			var hashCode = 859600377;
			hashCode = hashCode * -1521134295 + base.GetHashCode();
			hashCode = hashCode * -1521134295 + Width.GetHashCode();
			hashCode = hashCode * -1521134295 + Height.GetHashCode();
			return hashCode;
		}

		internal static ImageSize FromString(string input)
		{
			var regex = $"(\\d+){Delimiter}(\\d+)";
			if(!Regex.IsMatch(input, regex)) throw new ArgumentException();
			var result = Regex.Match(input, regex).Groups.Skip(1).Select(x => int.Parse(x.Value)).ToArray();
			return new ImageSize(result[0], result[1]);
		}
	}
}