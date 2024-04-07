var global = global || globalThis || (function () { return this; }());

let World = global.World = global.World || {};

const loader = puer.loader;
const world = loader.World;
const worldInfo = loader.WorldInfo;
const getLastException = puerts.getLastException;
const GetSourceMapStack = loader.GetSourceMapStack.bind(loader);

World.getLastException = () => getLastException();

World.getSourceMapStack = stack => {
    return GetSourceMapStack(stack);
}

const Log = puer.loadType('创世记.Log');
const Utils = puer.loadType('创世记.Utils');
const Json = puer.loadType('Godot.Json');

World.Log = Log;

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
World.info = JSON.parse(worldInfo.JsonString);

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
    for(let i = 0; i < buttons.Length; i++) {
        array.push(buttons.GetValue(i) ?? "");
    }
    return array;
}
World.setRightButtons = buttons => {
    var buttons = world.SetRightButtons(JSON.stringify(buttons));
    var array = [];
    for(let i = 0; i < buttons.Length; i++) {
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
World.setBackgroundTexture = (path, filter = World.FilterType.Linear) => world.SetBackgroundTexture(path, filter);

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
    if(value && typeof value === "object") {
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
    if(value && typeof value === "object") {
        if (value.constructor.name.includes("Godot.Collections.Dictionary") || value.constructor.name.includes("Godot.Collections.Array")) {
            value = JSON.parse(Utils.GetJsonString(value));
        }
    }
    return value ?? defaultValue;
}

World.versionCompare = (version1, version2) => Utils.VersionCompare(version1, version2);
World.exit = (exitCode = 1) => world.Exit(exitCode);

world.JsEvent = {
    emit(event, args){
        if (args !=null) {
            let array = [];
            for(let i = 0; i < args.Length; i++) {
                array.push(args.GetValue(i) ?? "");
            }
            World.event.emit(event, ...array);
        } else {
            World.event.emit(event);
        }
    }
}