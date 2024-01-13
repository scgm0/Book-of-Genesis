using System.Linq;
using Godot;

namespace 创世记;

public static class ImageFileFormatFinder {
	static private readonly byte[] Bmp = "BM"u8.ToArray();
	static private readonly byte[] Webp = "RIFF"u8.ToArray();
	static private readonly byte[] Png = [0x89, 0x50, 0x4E, 0x47];
	static private readonly byte[] Jpeg = [0xFF, 0xD8, 0xFF, 0xE0];
	static private readonly byte[] Jpeg2 = [0xFF, 0xD8, 0xFF, 0xE1];

	public static ImageFormat GetImageFormat(byte[] fileHeader) {
		var buffer = new byte[4];
		System.Buffer.BlockCopy(fileHeader, 0, buffer, 0, 4);
		GD.PrintS(buffer.Stringify());

		if (Bmp.SequenceEqual(buffer.Take(Bmp.Length)))
			return ImageFormat.Bmp;

		if (Png.SequenceEqual(buffer.Take(Png.Length)))
			return ImageFormat.Png;

		if (Jpeg.SequenceEqual(buffer.Take(Jpeg.Length)))
			return ImageFormat.Jpg;

		if (Jpeg2.SequenceEqual(buffer.Take(Jpeg2.Length)))
			return ImageFormat.Jpg;

		return Webp.SequenceEqual(buffer.Take(Webp.Length)) ? ImageFormat.Webp : ImageFormat.Unknown;
	}
}