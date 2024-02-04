// ReSharper disable CheckNamespace

// ReSharper disable UnusedMember.Global
public static class AudioFileFormatFinder {
	public static AudioFormat GetAudioFormat(byte[] fileHeader) {
		return fileHeader switch {
			[0x4F, 0x67, 0x67, 0x53] => AudioFormat.Ogg,
			[0x49, 0x44, 0x33, 0x3] => AudioFormat.Mp3,
			[0x52, 0x49, 0x46, 0x46] => AudioFormat.Wav,
			_ => AudioFormat.Unknown
		};
	}

	public static AudioFormat GetAudioFormat(uint fileHeader) {
		return fileHeader switch {
			0x5367674F => AudioFormat.Ogg,
			0x03334449 => AudioFormat.Mp3,
			0x46464952 => AudioFormat.Wav,
			_ => AudioFormat.Unknown
		};
	}
}