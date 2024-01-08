#nullable enable
using System;
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;
using 创世记;
using FileAccess = Godot.FileAccess;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnassignedField.Global

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable NotAccessedField.Local

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
// ReSharper disable once ClassNeverInstantiated.Global

public enum AudioFormat {
	Ogg,
	Wav,
	Mp3,
	Unknown
}

public static class AudioFileFormatFinder {
	public static AudioFormat GetAudioFormat(byte[] fileHeader) {
		return fileHeader switch {
			[0x4F, 0x67, 0x67, 0x53] => AudioFormat.Ogg, // OggS
			[0x49, 0x44, 0x33, 0x3] => AudioFormat.Mp3, // ID3
			[0x52, 0x49, 0x46, 0x46] => AudioFormat.Wav, // RIFF
			_ => AudioFormat.Unknown
		};
	}

	public static AudioFormat GetAudioFormat(uint fileHeader) {
		return fileHeader switch {
			0x5367674F => AudioFormat.Ogg, // OggS
			0x03334449 => AudioFormat.Mp3, // ID3
			0x46464952 => AudioFormat.Wav, // RIFF
			_ => AudioFormat.Unknown
		};
	}
}

public partial class AudioPlayer : AudioStreamPlayer {
	private AudioStream? _audioStream;
	private string? _audioStreamPath;

	public JsValue? FinishedCallback;

	public double Duration { get => _audioStream?.GetLength() ?? 0; }

	public float CurrentPosition { get => GetPlaybackPosition(); }

	public bool IsPlaying { get => Playing; }

	public bool Loop { set => _audioStream?.Set("loop", value); get => (bool)(_audioStream?.Get("loop") ?? false); }

	public AudioPlayer() {
		Utils.Tree.Root.AddChild(this);
		AddToGroup("Audio");
		Finished += () => {
			if (FinishedCallback is Function) {
				FinishedCallback?.Call(thisObj: JsValue.FromObject(Main.CurrentEngine, this), []);
			}
		};
	}

	public AudioPlayer SetAudioPath(string path) {
		path = (Main.CurrentModInfo.IsUser ? Utils.UserModsPath : Utils.ResModsPath).PathJoin(Main.CurrentModInfo.Path)
			.PathJoin(path).SimplifyPath();
		_audioStream?.Dispose();

		if (!FileAccess.FileExists(path)) {
			throw new JavaScriptException($"{path}文件不存在");
		}

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);

		switch (AudioFileFormatFinder.GetAudioFormat(file.Get32())) {
			case AudioFormat.Ogg:
				_audioStream = AudioStreamOggVorbis.LoadFromFile(path);
				break;
			case AudioFormat.Mp3:
				_audioStream = new AudioStreamMP3();
				(_audioStream as AudioStreamMP3)!.Data = FileAccess.GetFileAsBytes(path);
				break;
			case AudioFormat.Wav:
				_audioStream = new AudioStreamWav();
				var wav = (AudioStreamWav)_audioStream;
				var header = WavFileHeader.CreateFromFileAccess(file);
				wav.Format = header.BitsPerSample switch {
					8 => AudioStreamWav.FormatEnum.Format8Bits, 16 => AudioStreamWav.FormatEnum.Format16Bits, _ => wav.Format
				};
				wav.Data = file.GetBuffer((long)(file.GetLength() - (ulong)header.HeaderSize));
				break;
			case AudioFormat.Unknown:
			default:
				throw new JavaScriptException("不支持的音频格式，仅支持ogg、mp3与wav");
		}

		if (_audioStream?.InstantiatePlayback() == null) {
			_audioStream = null;
			throw new JavaScriptException("不支持的音频格式，仅支持ogg、mp3与wav");
		}

		_audioStreamPath = path;
		Stream = _audioStream;
		return this;
	}

	public AudioPlayer Pause() {
		if (_audioStream == null) {
			throw new JavaScriptException("未设置音频文件");
		}

		StreamPaused = true;
		return this;
	}

	public AudioPlayer Play(float? fromPosition = null) {
		if (_audioStream == null) {
			throw new JavaScriptException("未设置音频文件");
		}

		if (IsPlaying) {
			return this;
		}

		if (StreamPaused) {
			StreamPaused = false;
			return this;
		}

		StreamPaused = false;
		base.Play(fromPosition ?? CurrentPosition);
		return this;
	}

	public new AudioPlayer Seek(float pos) {
		if (_audioStream == null) {
			throw new JavaScriptException("未设置音频文件");
		}

		base.Seek(pos);
		return this;
	}

	public new AudioPlayer Stop() {
		if (_audioStream == null) {
			throw new JavaScriptException("未设置音频文件");
		}

		if (!IsPlaying) {
			return this;
		}

		base.Stop();
		return this;
	}

	public new void Dispose() {
		_audioStream?.Dispose();
		Stop();
		QueueFree();
	}

	public static AudioPlayer PlayFile(
		string path,
		bool loop = false,
		float? fromPosition = null,
		JsValue? finishCallback = null) {
		var audioPlayer = new AudioPlayer();
		audioPlayer.FinishedCallback = finishCallback;
		audioPlayer.SetAudioPath(path);
		audioPlayer.Loop = loop;
		audioPlayer.Play(fromPosition);
		return audioPlayer;
	}
}