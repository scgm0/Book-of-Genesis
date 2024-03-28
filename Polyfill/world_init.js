var global = global || globalThis || (function () { return this; }());

let World = global.World = global.World || {};

const loader = puer.loader;
const world = loader.World;
const worldInfo = loader.WorldInfo;

World.getSourceMapStack = (stack) => {
    return loader.GetSourceMapStack(stack);
}

const Log = puer.loadType('创世记.Log');

World.Log = Log;

(function (FilterType) {
    FilterType[FilterType["Linear"] = 0] = "Linear";
    FilterType[FilterType["Nearest"] = 1] = "Nearest";
})(World.FilterType = {});

(function (EventType) {
    EventType[EventType["Ready"] = 0] = "Ready";
    EventType[EventType["Exit"] = 1] = "Exit";
    EventType[EventType["Command"] = 2] = "Command";
    EventType[EventType["LeftButtonClick"] = 3] = "LeftButtonClick";
    EventType[EventType["RightButtonClick"] = 4] = "RightButtonClick";
    EventType[EventType["TextUrlClick"] = 5] = "TextUrlClick";
})(World.EventType = {});

(function (TextType) {
    TextType[TextType["All"] = 0] = "All";
    TextType[TextType["Title"] = 1] = "Title";
    TextType[TextType["LeftText"] = 2] = "LeftText";
    TextType[TextType["CenterText"] = 3] = "CenterText";
    TextType[TextType["RightText"] = 4] = "RightText";
})(World.TextType = {});

World.info = {
    get name() {
        return worldInfo.Name;
    },

    get version() {
        return worldInfo.Version;
    }
    
}

World.delay = (n) => new Promise(resolve => setTimeout(resolve, n));
World.toast = world.Toast;
World.setTitle = (...args) => world.SetTitle(...args);
World.setCenterText = (...args) => world.SetCenterText(...args);
World.setText = (...args) => world.SetText(...args);
World.addText = (...args) => world.AddText(...args);
World.setCommandPlaceholderText = (...args) => world.SetCommandPlaceholderText(...args);
World.removeParagraph = (...args) => world.RemoveParagraph(...args);

World.setTextBackgroundColor = (...args) => world.SetTextBackgroundColor(...args);

world.JsEventEmit = (event, args) => {
    if (args !=null) {
        let a = [];
        for(let i = 0; i < args.Length; i++) {
            a.push(args[i])
        }
        console.log(a);
        World.event.emit(event, ...a);
    } else {
        World.event.emit(event);
    }
}