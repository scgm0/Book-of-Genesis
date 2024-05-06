using System;
using System.Text;
using Godot;

// ReSharper disable MemberCanBePrivate.Global

namespace 创世记;

public struct WavFileHeader {
	public uint ChunkId;
	public uint ChunkSize;
	public uint Format;
	public uint SubChunk1Id;
	public uint SubChunk1Size;
	public ushort AudioFormat;
	public ushort NumChannels;
	public uint SampleRate;
	public uint ByteRate;
	public ushort BlockAlign;
	public ushort BitsPerSample;
	public ushort ExtraParamSize;
	public ushort Samples;
	public uint ChannelMask;
	public Guid GuidSubFormat;
	public uint SubChunk2Id;
	public uint SubChunk2Size;
	public bool IsExtensible;
	public int HeaderSize;
	public double Duration;
	public long TotalSamples;
	public static readonly Guid SubTypePcm = new("00000001-0000-0010-8000-00aa00389b71");
	public static readonly Guid SubTypeIeeeFloat = new("00000003-0000-0010-8000-00aa00389b71");

	public static WavFileHeader CreateFromFileAccess(FileAccess file) {
		file.Seek(0);
		var header = new WavFileHeader {
			ChunkId = file.Get32(),
			ChunkSize = file.Get32(),
			Format = file.Get32()
		};
		header.HeaderSize += 12;

		header.SubChunk1Id = file.Get32();
		header.SubChunk1Size = file.Get32();
		header.AudioFormat = file.Get16();
		header.NumChannels = file.Get16();
		header.SampleRate = file.Get32();
		header.ByteRate = file.Get32();
		header.BlockAlign = file.Get16();
		header.BitsPerSample = file.Get16();
		header.HeaderSize += 24;

		switch (header) {
			case { SubChunk1Size: 16, AudioFormat: 1 }:
				header.IsExtensible = false;
				while (file.Peek() != 0) {
					var chunk = file.Get32();
					header.HeaderSize += 4;
					if (chunk == 0x61746164) {
						header.SubChunk2Id = chunk;
						header.SubChunk2Size = file.Get32();
						header.HeaderSize += 4;
						break;
					}

					var chunkSize = file.Get32();
					header.HeaderSize += 4;
					file.Skip(chunkSize);
					header.HeaderSize += (int)chunkSize;
				}

				break;
			case { SubChunk1Size: > 16, AudioFormat: 0xFFFE }:
				header.ExtraParamSize = file.Get16();
				header.HeaderSize += 2;
				if (header.ExtraParamSize == 22) {
					header.IsExtensible = true;
					header.Samples = file.Get16();
					header.ChannelMask = file.Get32();
					var subFormat = file.GetBuffer(16);
					header.HeaderSize += 22;
					header.GuidSubFormat = new Guid(subFormat);
					if (header.GuidSubFormat != SubTypePcm &&
						header.GuidSubFormat != SubTypeIeeeFloat) {
						throw new Exception($"不支持的WAV文件类型: {header.GuidSubFormat}");
					}

					while (file.Peek() != 0) {
						var chunk = file.Get32();
						header.HeaderSize += 4;
						if (chunk == 0x61746164) {
							header.SubChunk2Id = chunk;
							header.SubChunk2Size = file.Get32();
							header.HeaderSize += 4;
							break;
						}

						var chunkSize = file.Get32();
						header.HeaderSize += 4;
						file.Skip(chunkSize);
						header.HeaderSize += (int)chunkSize;
					}
				} else {
					throw new Exception("不支持的WAV文件头");
				}

				break;
			default: throw new Exception("不支持的WAV文件头");
		}

		header.TotalSamples = (long)(header.SubChunk2Size /
			(header.NumChannels * (double)header.BitsPerSample / 8));
		header.Duration = 1 / (double)header.SampleRate * header.TotalSamples;
		return header;
	}
}