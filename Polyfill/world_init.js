import Console from "console";
import EventEmitter from "event";
import AudioPlayer from "audio";
import sourceMapSupport from 'source-map-support';

var global = global || globalThis || (function () {
    return this;
}());

globalThis.console = new Console(puer.loadType('创世记.Log'));
globalThis.EventEmitter = EventEmitter;
globalThis.AudioPlayer = AudioPlayer;

const World = global.World = global.World || {};
const { isRelative, normalize, dirname } = global.__puer_path__;
const loadFile = puer.loadFile;
const loader = puer.loader;
const world = loader.World;
const worldInfo = loader.WorldInfo;
const getLastException = puerts.getLastException;
const Utils = puer.loadType('创世记.Utils');

puer.on('unhandledRejection', (error, promise) => {
    if (World.event.emit('unhandledRejection', error, promise)) return;
    console.error('unhandledRejection', error);
});

sourceMapSupport.install({
    retrieveFile: (path) => {
        return loadFile(path).content;
    }
});

World.event = new EventEmitter();
World.playFile = AudioPlayer.playFile;
World.getLastException = function () {
    let error = getLastException();
    if (error instanceof SyntaxError) {
        var match = /\n    at (.*?):(\d+):(\d+)/.exec(arguments[0]);
        let filePath = match[1];
        let lineNumber = match[2];
        let columnNumber = match[3];
        error.stack = Error.prepareStackTrace(error, [new SyntaxCallSite(filePath, lineNumber * 1, ++columnNumber)]).toString();
    }
    return error;
}

class SyntaxCallSite {
    #fileName;
    #lineNumber;
    #columnNumber;

    constructor(fileName, lineNumber, columnNumber) {
        this.#fileName = fileName;
        this.#lineNumber = lineNumber;
        this.#columnNumber = columnNumber;
    }

    getFileName() {
        return this.#fileName;
    }

    getLineNumber() {
        return this.#lineNumber;
    }

    getColumnNumber() {
        return this.#columnNumber;
    }

    getFunctionName() {
        return "";
    }

    isNative() {
        return false;
    }

    isConstructor() {
        return false;
    }

    isToplevel() {
        return true;
    }
}

(function (FilterType) {
    const _filterType = puer.loadType('创世记.FilterType');
    FilterType[FilterType["Linear"] = _filterType.Linear] = "Linear";
    FilterType[FilterType["Nearest"] = _filterType.Nearest] = "Nearest";
})(World.FilterType = {});

(function (EventType) {
    const _eventType = puer.loadType('创世记.EventType');
    EventType[EventType["Ready"] = _eventType.Ready] = "Ready";
    EventType[EventType["Exit"] = _eventType.Exit] = "Exit";
    EventType[EventType["Command"] = _eventType.Command] = "Command";
    EventType[EventType["LeftButtonClick"] = _eventType.LeftButtonClick] = "LeftButtonClick";
    EventType[EventType["RightButtonClick"] = _eventType.RightButtonClick] = "RightButtonClick";
    EventType[EventType["TextUrlClick"] = _eventType.TextUrlClick] = "TextUrlClick";
})(World.EventType = {});

(function (TextType) {
    const _textType = puer.loadType('创世记.TextType');
    TextType[TextType["Title"] = _textType.Title] = "Title";
    TextType[TextType["LeftText"] = _textType.LeftText] = "LeftText";
    TextType[TextType["CenterText"] = _textType.CenterText] = "CenterText";
    TextType[TextType["RightText"] = _textType.RightText] = "RightText";
    TextType[TextType["All"] = _textType.All] = "All";
})(World.TextType = {});

World.gameVersion = Utils.GameVersion;
World.info = {
    name: worldInfo.Name,
    author: worldInfo.Author,
    main: worldInfo.Main,
    icon: worldInfo.Icon,
    version: worldInfo.Version,
    description: worldInfo.Description,
    is_encrypt: worldInfo.IsEncrypt
};

World.delay = n => new Promise(resolve => setTimeout(resolve, n));
World.toast = text => world.ShowToast(text);

World.setText = (type, text) => world.SetText(type, text);
World.setTitle = text => world.SetTitle(text);
World.setLeftText = text => world.SetLeftText(text);
World.setCenterText = text => world.SetCenterText(text);
World.setSetRightText = text => world.SetRightText(text);

World.addText = (type, text) => world.AddText(type, text);
World.addTitle = text => world.AddTitle(text);
World.addLeftText = text => world.AddLeftText(text);
World.addCenterText = text => world.AddCenterText(text);
World.addRightText = text => world.AddRightText(text);

World.setCommandPlaceholderText = text => world.SetCommandPlaceholderText(text);

World.getParagraphCount = type => world.GetParagraphCount(type);
World.removeParagraph = (type, index) => world.RemoveParagraph(type, index);

World.setStretchRatio = (type, ratio) => world.SetStretchRatio(type, ratio);
World.setLeftStretchRatio = ratio => world.SetLeftStretchRatio(ratio);
World.setCenterStretchRatio = ratio => world.SetCenterStretchRatio(ratio);
World.setRightStretchRatio = ratio => world.SetRightStretchRatio(ratio);

World.setTextBackgroundColor = (type, colorHex) => world.SetTextBackgroundColor(type, colorHex);
World.setTextFontColor = (type, colorHex) => world.SetTextFontColor(type, colorHex);

World.setLeftButtons = buttons => {
    var buttons = world.SetLeftButtons(JSON.stringify(buttons));
    var array = [];
    for (let i = 0; i < buttons.Length; i++) {
        array.push(buttons.GetValue(i) ?? "");
    }
    return array;
}
World.setRightButtons = buttons => {
    var buttons = world.SetRightButtons(JSON.stringify(buttons));
    var array = [];
    for (let i = 0; i < buttons.Length; i++) {
        array.push(buttons.GetValue(i) ?? "");
    }
    return array;
}
World.addLeftButton = text => world.AddLeftButton(text);
World.addRightButton = text => world.AddRightButton(text);
World.removeLeftButtonByIndex = index => world.RemoveLeftButtonByIndex(index);
World.removeRightButtonByIndex = index => world.RemoveRightButtonByIndex(index);
World.removeButtonById = id => Utils.RemoveButtonById(id);

World.setBackgroundColor = colorHex => world.SetBackgroundColor(colorHex);
World.setBackgroundTexture = (path, filter = World.FilterType.Linear) => {
    world.SetBackgroundTexture(World.toAbsolutePath(path), filter);
};

World.setSaveValue = (section, key, value) => {
    if (typeof value === "object") {
        value = {
            value: JSON.stringify(value)
        }
    }
    Utils.SetSaveValue(section, key, value);
}

World.getSaveValue = (section, key, defaultValue) => {
    let value = Utils.GetSaveValue(section, key);
    if (value && typeof value === "object") {
        if (value.constructor.name.includes("Godot.Collections.Dictionary") || value.constructor.name.includes("Godot.Collections.Array")) {
            value = JSON.parse(Utils.GetJsonString(value));
        }
    }
    return value ?? defaultValue;
}

World.setGlobalSaveValue = (section, key, value) => {
    if (typeof value === "object") {
        value = {
            value: JSON.stringify(value)
        }
    }
    Utils.SetGlobalSaveValue(section, key, value);
}

World.getGlobalSaveValue = (section, key, defaultValue) => {
    let value = Utils.GetGlobalSaveValue(section, key);
    if (value && typeof value === "object") {
        if (value.constructor.name.includes("Godot.Collections.Dictionary") || value.constructor.name.includes("Godot.Collections.Array")) {
            value = JSON.parse(Utils.GetJsonString(value));
        }
    }
    return value ?? defaultValue;
}

World.readAsText = path => {
    return Utils.ReadAsText(World.toAbsolutePath(path));
}

World.readAsArrayBuffer = path => {
    return Utils.ReadAsArrayBuffer(World.toAbsolutePath(path));
}

World.versionCompare = (version1, version2) => Utils.VersionCompare(version1, version2);
World.exit = (exitCode = 1) => world.Exit(exitCode);

World.toAbsolutePath = (path) => {
    return typeof path === "string" && isRelative(path) ? normalize(dirname(World.callSites()[2].getFileName()) + "/" + path) : path;
}

World.callSites = () => {
    const _prepareStackTrace = Error.prepareStackTrace;
    try {
        let result = [];
        Error.prepareStackTrace = (_, callSites) => {
            const callSitesWithoutCurrent = callSites.slice(1);
            result = callSitesWithoutCurrent;
            return callSitesWithoutCurrent;
        };

        new Error().stack;
        Error.prepareStackTrace = _prepareStackTrace;
        return result;
    } finally {
        Error.prepareStackTrace = _prepareStackTrace;
    }
}

world.JsEvent = {
    emit(event, args) {
        if (args != null) {
            let array = [];
            for (let i = 0; i < args.Length; i++) {
                array.push(args.GetValue(i));
            }
            World.event.emit(event, ...array);
        } else {
            World.event.emit(event);
        }
    }
}