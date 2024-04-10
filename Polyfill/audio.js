const CSAudioPlayer = puer.loadType('创世记.AudioPlayer');

export class AudioPlayer {
    #audio_player = new CSAudioPlayer(this);
    #finishedCallback = null;

    get duration() {
        return this.#audio_player.Duration;
    }

    get currentPosition() {
        return this.#audio_player.CurrentPosition;
    }

    get isPlaying() {
        return this.#audio_player.IsPlaying;
    }

    set loop(loop) {
        this.#audio_player.Loop = loop;
        return this;
    }

    get loop() {
        return this.#audio_player.Loop;
    }

    set finishedCallback(callback) {
        this.#finishedCallback = callback?.bind(this);
    }

    get finishedCallback() {
        return this.#finishedCallback;
    }

    setAudioPath(path) {
        if (global.__puer_path__.isRelative(path)) {
            let file = World.callSites()[1].getFileName();
            path = global.__puer_path__.normalize(global.__puer_path__.dirname(file) + "/" + path);
        }
        this.#audio_player.SetAudioPath(path);
        return this;
    }

    play(fromPosition) {
        this.#audio_player.Play(fromPosition);
        return this;
    }

    seek(position) {
        this.#audio_player.Seek(position);
        return this;
    }

    stop() {
        this.#audio_player.Stop();
        return this;
    }

    pause() {
        this.#audio_player.Pause();
        return this;
    }

    dispose() {
        this.#audio_player.Dispose();
    }

    static playFile(path, loop = false, fromPosition = null, finishedCallback = null) {
        if (global.__puer_path__.isRelative(path)) {
            let file = World.callSites()[1].getFileName();
            path = global.__puer_path__.normalize(global.__puer_path__.dirname(file) + "/" + path);
        }
        let audio_player = new AudioPlayer();
        audio_player.finishedCallback = finishedCallback ?? (() => audio_player.dispose());
        audio_player.setAudioPath(path);
        audio_player.loop = loop;
        audio_player.play(fromPosition);
        return audio_player;
    }
}

export default AudioPlayer;