using System;
using Godot;
using Puerts;
using FileAccess = Godot.FileAccess;

// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnassignedField.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable NotAccessedField.Local
// ReSharper disable UnusedMember.Global

namespace 创世记;

public sealed class AudioPlayer {
	private readonly AudioStreamPlayer _player = new();
	private AudioStream? _audioStream;
	private string? _audioStreamPath;

	public JSObject JsObject;

	public double Duration { get => _audioStream?.GetLength() ?? 0; }

	public float CurrentPosition { get => _player.GetPlaybackPosition(); }

	public bool IsPlaying { get => _player.Playing; }

	public bool Loop { set => _player.Set("parameters/looping", value); get => _player.Get("parameters/looping").AsBool(); }

	public AudioPlayer(JSObject jsObject) {
		JsObject = jsObject;
		Utils.Tree.Root.AddChild(_player);
		_player.Finished += () => {
			JsObject.Get<Action?>("finishedCallback")?.Invoke();
		};
		Utils.AudioPlayerCache.Add(this);
	}

	public AudioPlayer SetAudioPath(string path) {
		path = Main.CurrentWorldInfo!.GlobalPath.PathJoin(path).SimplifyPath();
		_audioStream?.Dispose();

		if (!FileAccess.FileExists(path)) {
			throw new Exception($"{path}文件不存在");
		}

		using var file = FileAccess.Open(path, FileAccess.ModeFlags.Read);

		switch (AudioFileFormatFinder.GetAudioFormat(file.Get32())) {
			case AudioFormat.Ogg:
				_audioStream = AudioStreamOggVorbis.LoadFromFile(path);
				break;
			case AudioFormat.Mp3:
				_audioStream = new AudioStreamMP3();
				((AudioStreamMP3)_audioStream).Data = FileAccess.GetFileAsBytes(path);
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
				throw new Exception("不支持的音频格式，仅支持ogg、mp3与wav");
		}

		if (_audioStream?.InstantiatePlayback() == null) {
			_audioStream = null;
			throw new Exception("不支持的音频格式，仅支持ogg、mp3与wav");
		}

		_audioStreamPath = path;
		_player.Stream = _audioStream;
		return this;
	}

	public AudioPlayer Pause() {
		if (_audioStream == null) {
			throw new Exception("未设置音频文件");
		}

		_player.StreamPaused = true;
		return this;
	}

	public AudioPlayer Play(float? fromPosition = null) {
		if (_audioStream == null) {
			throw new Exception("未设置音频文件");
		}

		if (IsPlaying) {
			return this;
		}

		if (_player.StreamPaused) {
			_player.StreamPaused = false;
			return this;
		}

		_player.StreamPaused = false;
		_player.Play(fromPosition ?? CurrentPosition);
		return this;
	}

	public AudioPlayer Seek(float pos) {
		if (_audioStream == null) {
			throw new Exception("未设置音频文件");
		}

		_player.Seek(pos);
		return this;
	}

	public AudioPlayer Stop() {
		if (IsPlaying && _audioStream != null) {
			_player.Stop();
		}

		return this;
	}

	public void Dispose() {
		Stop();
		_audioStream?.Dispose();
		_player.QueueFree();
	}
}