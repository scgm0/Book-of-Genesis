#nullable enable
using Godot;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Jint.Runtime;
using 创世记;
// ReSharper disable UnusedMethodReturnValue.Global
// ReSharper disable UnassignedField.Global

// ReSharper disable MemberCanBePrivate.Global

// ReSharper disable NotAccessedField.Local

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
// ReSharper disable once ClassNeverInstantiated.Global
public partial class AudioPlayer : AudioStreamPlayer {
	private AudioStream? _audioStream;
	private string? _audioStreamPath;

	public JsValue? FinishedCallback;

	public double Duration {
		get => _audioStream?.GetLength() ?? 0;
	}

	public float CurrentPosition {
		get => GetPlaybackPosition();
	}

	public bool IsPlaying {
		get => Playing;
	}

	public bool Loop {
		set => _audioStream?.Set("loop", value);
		get => (bool)(_audioStream?.Get("loop") ?? false);
	}

	public AudioPlayer() {
		Utils.Tree.Root.AddChild(this);
		AddToGroup("Audio");
		Finished += () => {
			if(FinishedCallback is FunctionInstance) {
				FinishedCallback?.Call(thisObj: JsValue.FromObject(Main.CurrentEngine, this), []);
			}
		};
	}

	// jint暂不支持
	/* public async Task<bool> Prepare() {
		await Task.Run(() => {
			if (FileAccess.FileExists(_audioStreamPath)) {
				switch (_audioStreamPath!.GetExtension()) {
					case "ogg":
						_audioStream = AudioStreamOggVorbis.LoadFromFile(_audioStreamPath);
						break;
					case "mp3": {
						_audioStream = new AudioStreamMP3();
						(_audioStream as AudioStreamMP3)!.Data = FileAccess.GetFileAsBytes(_audioStreamPath);
						break;
					}
					default:
						throw new JavaScriptException("不支持的音频格式，仅支持 ogg 和 mp3");
				}

			} else {
				throw new JavaScriptException($"{_audioStreamPath} 文件不存在");
			}
		});
		_streamPlayer.Stream = _audioStream;
		GD.PrintS(0, _audioStream);
		return true;
	} */

	public AudioPlayer SetAudioPath(string path) {
		path = (Main.CurrentModInfo.IsUser ? Utils.UserModsPath : Utils.ResModsPath).PathJoin(Main.CurrentModInfo.Path)
			.PathJoin(path).SimplifyPath();
		_audioStream?.Dispose();

		if (!FileAccess.FileExists(path)) {
			throw new JavaScriptException($"{path} 文件不存在");
		}

		switch (path.GetExtension()) {
			case "ogg":
				_audioStream = AudioStreamOggVorbis.LoadFromFile(path);
				break;
			case "mp3": {
				_audioStream = new AudioStreamMP3();
				(_audioStream as AudioStreamMP3)!.Data = FileAccess.GetFileAsBytes(path);
				break;
			}
			default:
				throw new JavaScriptException("不支持的音频格式，仅支持 ogg 和 mp3");
		}

		if (_audioStream.InstantiatePlayback() == null) {
			_audioStream = null;
			throw new JavaScriptException("不支持的音频格式，仅支持 ogg 和 mp3");
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

	public static AudioPlayer PlayFile(string path, bool loop = false, float? fromPosition = null, JsValue? finishCallback = null){
		var audioPlayer = new AudioPlayer();
		audioPlayer.FinishedCallback = finishCallback;
		audioPlayer.SetAudioPath(path);
		audioPlayer.Loop = loop;
		audioPlayer.Play(fromPosition);
		return audioPlayer;
	}
}