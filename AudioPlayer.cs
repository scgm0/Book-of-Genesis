using System;
using Godot;
using Puerts;
using FileAccess = Godot.FileAccess;

namespace 创世记;

public sealed class AudioPlayer {
	private readonly AudioStreamPlayer _player = new();
	private AudioStream? _audioStream;

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
		var filePath = Main.CurrentWorldInfo!.GlobalPath.PathJoin(path).SimplifyPath();
		_audioStream?.Dispose();

		if (!FileAccess.FileExists(filePath)) {
			World.ThrowException($"{path}文件不存在");
			return this;
		}

		using var file = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);

		switch (AudioFileFormatFinder.GetAudioFormat(file.Get32())) {
			case AudioFormat.Ogg:
				_audioStream = AudioStreamOggVorbis.LoadFromFile(filePath);
				break;
			case AudioFormat.Mp3:
				_audioStream = new AudioStreamMP3();
				((AudioStreamMP3)_audioStream).Data = FileAccess.GetFileAsBytes(filePath);
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
				World.ThrowException("不支持的音频格式，仅支持ogg、mp3与wav");
				return this;
		}

		if (_audioStream?.InstantiatePlayback() == null) {
			_audioStream = null;
			World.ThrowException("不支持的音频格式，仅支持ogg、mp3与wav");
			return this;
		}

		_player.Stream = _audioStream;
		return this;
	}

	public AudioPlayer Pause() {
		if (_audioStream == null) {
			World.ThrowException("未设置音频文件");
			return this;
		}

		_player.StreamPaused = true;
		return this;
	}

	public AudioPlayer Play(float? fromPosition = null) {
		if (_audioStream == null) {
			World.ThrowException("未设置音频文件");
			return this;
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

	public AudioPlayer Seek(float position) {
		if (_audioStream == null) {
			World.ThrowException("未设置音频文件");
			return this;
		}

		_player.Seek(position);
		return this;
	}

	public AudioPlayer Stop() {
		if (_audioStream != null && IsPlaying) {
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