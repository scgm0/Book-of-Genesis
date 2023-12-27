
interface EventEmitter {
	get defaultMaxListeners(): number;
	set defaultMaxListeners(n: number);
	addListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
	emit(eventName: string | symbol, ...args: any[]): boolean;
	setMaxListeners(n: number): this;
	getMaxListeners(): number;
	listenerCount(eventName: string | symbol): number;
	eventNames(): (string | symbol)[];
	listeners(eventName: string | symbol): Function[];
	off(eventName: string | symbol, listener: (...args: any[]) => void): this;
	on(eventName: string | symbol, listener: (...args: any[]) => void, prepend?: boolean): this;
	removeAllListeners(eventName: string | symbol): this;
	removeListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
	once(eventName: string | symbol, listener: (...args: any[]) => void): this;
	prependListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
	prependOnceListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
	rawListeners(eventName: string | symbol): Function[];
}

declare module "events" {
	export class EventEmitter {
		get defaultMaxListeners(): number;
		set defaultMaxListeners(n: number);
		addListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
		emit(eventName: string | symbol, ...args: any[]): boolean;
		setMaxListeners(n: number): this;
		getMaxListeners(): number;
		listenerCount(eventName: string | symbol): number;
		eventNames(): (string | symbol)[];
		listeners(eventName: string | symbol): Function[];
		off(eventName: string | symbol, listener: (...args: any[]) => void): this;
		on(eventName: string | symbol, listener: (...args: any[]) => void, prepend?: boolean): this;
		removeAllListeners(eventName: string | symbol): this;
		removeListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
		once(eventName: string | symbol, listener: (...args: any[]) => void): this;
		prependListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
		prependOnceListener(eventName: string | symbol, listener: (...args: any[]) => void): this;
		rawListeners(eventName: string | symbol): Function[];
	}
	export interface WrappedFunction extends Function {
		listener: (...args: any[]) => void;
	}
	export default EventEmitter;
}

declare module "audio" {
	export class AudioPlayer {
		finishedCallback: () => void;
		get duration(): number;
		get currentPosition(): number;
		get isPlaying(): number;
		get loop(): boolean;
		set loop(value: boolean);
		play(fromPosition: number): this;
		pause(): this;
		seek(position: number): this;
		stop(): this;
		setAudioPath(path: string): this;
		dispose(): void;

		static playFile(path: string, loop?: boolean, fromPosition?: number, finishedCallback?: () => void): AudioPlayer;
	}
	export default AudioPlayer;
}

declare function setTimeout(callback: (...args: any[]) => void, ms: number, ...args: any[]): number;
declare function clearTimeout(timeoutId: number): void;

declare function setInterval(callback: (...args: any[]) => void, ms: number, ...args: any[]): number;
declare function clearInterval(intervalId: number): void;

declare interface ImportMeta {
	url: string;
}

/** 世界事件 */
interface WorldEvent extends EventEmitter {
	/**
	 * 世界准备就绪事件，内容已显示。
	 * @event ready
	 */
	on(event: 'ready', listener: () => void): this;
	/**
	 * 世界帧事件，每秒60次，在ready后开始。
	 * @event tick
	 */
	on(event: 'tick', listener: () => void): this;
	/**
	 * 世界退出事件。
	 * @event exit
	 */
	on(event: 'exit', listener: (exitCode: number) => void): this;
	on(event: 'command', listener: (command: string) => void): this;
	on(event: 'left_button_click', listener: (text: string, index: number) => void): this;
	on(event: 'right_button_click', listener: (text: string, index: number) => void): this;
	on(event: 'text_url_click', listener: (meta: string | object, index: number) => void): this;
}

/** 世界对象 */
interface WorldObject {
	/** 世界事件 */
	get event(): WorldEvent;
	/** 世界信息 */
	get info(): {
		/** 名称 */
		name: string;
		/** 作者 */
		author: string;
		/** 主脚本 */
		main: string;
		/** 图标 */
		icon: string;
		/** 版本 */
		version: string;
		/** 介绍 */
		description: string;
		/** 是否加密 */
		is_encrypt: boolean;
	};
	print(...args: any[]): void;
	/**
	 * 设置世界标题
	 * @param title 标题
	 */
	setTitle(title: string): void;
	/**
	 * 设置背景颜色
	 * @param colorHex 16进制颜色字符串
	 */
	setBackgroundColor(colorHex: string): void;
	/**
	 * 设置背景纹理
	 * @param texturePath 纹理路径
	 */
	setBackgroundTexture(texturePath: string): void;
	/**
	 * 设置命令行提示文本
	 * @param placeholderText 提示文本
	 */
	setCommandPlaceholderText(placeholderText: string): void;
	setLeftButtons(buttons: string[]): number[];
	setRightButtons(buttons: string[]): number[];
	addLeftButton(button: string): number;
	addRightButton(button: string): number;
	removeLeftButton(index: number): void;
	removeRightButton(index: number): void;
	setLeftText(text: string): void;
	setCenterText(text: string): void;
	setRightText(text: string): void;
	addLeftText(text: string): void;
	addCenterText(text: string): void;
	addRightText(text: string): void;
	setLeftStretchRatio(ratio: number): void;
	setCenterStretchRatio(ratio: number): void;
	setRightStretchRatio(ratio: number): void;
	setSaveValue(section: string, key: string, value: any): void;
	setGlobalSaveValue(section: string, key: string, value: any): void;
	getSaveValue<T>(section: string, key: string, defaultValue?: T): T;
	getGlobalSaveValue<T>(section: string, key: string, defaultValue?: T): T;
}


//@ts-ignore
declare namespace globalThis {
	/** 世界对象 */
	const World: WorldObject
}
