#nullable enable
using System;
using System.Text;
using Godot;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable CheckNamespace

public struct WavFileHeader(uint subChunk1Size) {
	public uint ChunkId;
	public uint ChunkSize;
	public uint Format;
	public uint SubChunk1Id;
	public uint SubChunk1Size = subChunk1Size;
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

	public override string ToString() {
		return "[WAVE]\n" + $"ChunkID:\t\t{Encoding.ASCII.GetString(BitConverter.GetBytes(ChunkId))}\n" +
			$"ChunkSize:\t\t{ChunkSize}\n" + $"Format:\t\t{Encoding.ASCII.GetString(BitConverter.GetBytes(Format))}\n" +
			"[fmt]\n" + $"Subchunk1ID:\t\t{Encoding.ASCII.GetString(BitConverter.GetBytes(SubChunk1Id))}\n" +
			$"SubChunk1Size:\t{SubChunk1Size}\n" +
			$"AudioFormat:\t\t{AudioFormat switch { 1 => "1 : PCM", 0xFFFE => "0xFFFE : WAVEFORMATEXTENSIBLE", _ => AudioFormat.ToString() }}\n" +
			$"NumChannels:\t\t{NumChannels}\n" + $"SampleRate:\t\t{SampleRate}\n" + $"ByteRate:\t\t{ByteRate}\n" +
			$"BlockAlign:\t\t{BlockAlign}\n" + $"BitsPerSample:\t{BitsPerSample}\n" + "[extra]\n" +
			$"ExtraParamSize:\t{ExtraParamSize}\n" + "[extensible]\n" + $"Samples:\t\t{Samples}\n" +
			$"ChannelMask:\t\t{ChannelMask}\n" + $"GuidSubFormat:\t{GuidSubFormat + " : " + (GuidSubFormat == SubTypePcm
				? "PCM"
				: GuidSubFormat == SubTypeIeeeFloat ? "IEEE FLOAT" : "Unknown")}\n" + "[data]\n" +
			$"Subchunk2ID:\t\t{Encoding.ASCII.GetString(BitConverter.GetBytes(SubChunk2Id))}\n" +
			$"SubChunk2Size:\t{SubChunk2Size}\n" + "[info]\n" + $"IsExtensible:\t\t{IsExtensible}\n" +
			$"HeaderSize:\t\t{HeaderSize}\n" + $"Duration:\t\t{TimeSpan.FromSeconds(Duration)}\n" +
			$"TotalSamples:\t\t{TotalSamples}";
	}
}

public static class FieldAccessException {
	public static uint Peek(this FileAccess file) {
		var pos = file.GetPosition();
		var @char = file.Get8();
		file.Seek(pos);
		return @char;
	}

	public static void Skip(this FileAccess file, ulong length) {
		file.Seek(file.GetPosition() + length);
	}
}