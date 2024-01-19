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

public sealed class AudioPlayer {
	private readonly AudioStreamPlayer _player = new();
	private AudioStream? _audioStream;
	private string? _audioStreamPath;

	public JsValue? FinishedCallback;

	public double Duration { get => _audioStream?.GetLength() ?? 0; }

	public float CurrentPosition { get => _player.GetPlaybackPosition(); }

	public bool IsPlaying { get => _player.Playing; }

	public bool Loop { set => _player.Set("parameters/looping", value); get => _player.Get("parameters/looping").AsBool(); }

	public AudioPlayer() {
		Utils.Tree.Root.AddChild(_player); 
		_player.Finished += () => {
			if (FinishedCallback is Function function) {
				function.Call(thisObj: JsValue.FromObject(Main.CurrentEngine!, this), []);
			}
		};
		Utils.AudioPlayerCache.Add(this);
	}

	public AudioPlayer SetAudioPath(string path) {
		path = Main.CurrentWorldInfo!.GlobalPath.PathJoin(path).SimplifyPath();
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
				throw new JavaScriptException("不支持的音频格式，仅支持ogg、mp3与wav");
		}

		if (_audioStream?.InstantiatePlayback() == null) {
			_audioStream = null;
			throw new JavaScriptException("不支持的音频格式，仅支持ogg、mp3与wav");
		}

		_audioStreamPath = path;
		_player.Stream = _audioStream;
		return this;
	}

	public AudioPlayer Pause() {
		if (_audioStream == null) {
			throw new JavaScriptException("未设置音频文件");
		}

		_player.StreamPaused = true;
		return this;
	}

	public AudioPlayer Play(float? fromPosition = null) {
		if (_audioStream == null) {
			throw new JavaScriptException("未设置音频文件");
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
			throw new JavaScriptException("未设置音频文件");
		}

		_player.Seek(pos);
		return this;
	}

	public AudioPlayer Stop() {
		if (_audioStream == null) {
			throw new JavaScriptException("未设置音频文件");
		}

		if (!IsPlaying) {
			return this;
		}

		_player.Stop();
		return this;
	}

	public void Dispose() {
		Stop();
		_audioStream?.Dispose();
		_player.QueueFree(); 
	}

	public static AudioPlayer PlayFile(
		string path,
		bool loop = false,
		float? fromPosition = null,
		JsValue? finishCallback = null) {
		var audioPlayer = new AudioPlayer {
			FinishedCallback = finishCallback
		};
		audioPlayer.SetAudioPath(path);
		audioPlayer.Loop = loop;
		audioPlayer.Play(fromPosition);
		return audioPlayer;
	}
}