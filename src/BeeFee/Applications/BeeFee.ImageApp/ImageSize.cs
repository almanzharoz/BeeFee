using System;
using System.Linq;
using Newtonsoft.Json;

namespace BeeFee.ImageApp
{
	public struct ImageSize
	{
		[JsonProperty]
		public int Width { get; private set; }
		[JsonProperty]
		public int Height { get; private set; }

		public ImageSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		internal ImageSize(string resolution)
		{
			var sizes = resolution.Split('x').Select(int.Parse).ToArray();
			if(sizes.Length != 2) throw new ArgumentException();
			Width = sizes[0];
			Height = sizes[1];
		}

		public override string ToString()
		{
			return $"{Width}x{Height}";
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
	}
}