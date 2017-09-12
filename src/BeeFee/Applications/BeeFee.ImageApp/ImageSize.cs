using ImageSharp.Processing;

namespace BeeFee.ImageApp
{
	public struct ImageSize
	{
		public int Width { get; }
		public int Height { get; }

		public ImageSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public override string ToString()
		{
			return $"{Width}_{Height}";
		}
	}
}