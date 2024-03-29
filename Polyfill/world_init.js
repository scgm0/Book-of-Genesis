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

World.delay = n => new Promise(resolve => setTimeout(resolve, n));
World.toast = text => world.ShowToast(text);
World.setTitle = text => world.SetTitle(text);
World.setCenterText = text => world.SetCenterText(text);
World.setText = (type, text, exclude = null) => world.SetText(type, text, exclude);
World.addText = (type, text, exclude = null) => world.AddText(type, text, exclude);
World.setCommandPlaceholderText = text => world.SetCommandPlaceholderText(text);
World.removeParagraph = (type, index) => world.RemoveParagraph(type, index);

World.setTextBackgroundColor = (type, colorHex, exclude = null) => world.SetTextBackgroundColor(type, colorHex, exclude);

world.JsEvent = {
    emit(event, args){
        if (args !=null) {
            let array = [];
            for(let i = 0; i < args.Length; i++) {
                array.push(args.GetValue(i));
            }
            World.event.emit(event, ...array);
        } else {
            World.event.emit(event);
        }
    }
}
// world.SetEventEmit((event, args) => {
//     if (args !=null) {
//         let array = [];
//         for(let i = 0; i < args.Length; i++) {
//             array.push(args.GetValue(i));
//         }
//         World.event.emit(event, ...array);
//     } else {
//         World.event.emit(event);
//     }
// });