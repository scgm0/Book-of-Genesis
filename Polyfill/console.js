var global = global || globalThis || (function () { return this; }());

const Log = World.Log;

if (Log) {
    delete World.Log;
    var console = {};
    var getSourceMapStack = World.getSourceMapStack;

    function toString(args) {
        return Array.prototype.map.call(args, x => {
            try {
                return x instanceof Error ? `${x}\n${getSourceMapStack(getStack(x))}` : x.toString();
            } catch (err) {
                return err;
            }
        }).join(' ');
    }

    function getStack(error) {
        let stack = error.stack;
        stack = stack.replace(error.message, "");
        stack = stack.substring(stack.indexOf("\n")+1);
        return stack;
    }

    console.debug = function() {
        Log.Debug(toString(arguments));
    }

    console.log = function() {
        Log.Info(toString(arguments));
    }

    console.info = function() {
        Log.Info(toString(arguments));
    }

    console.warn = function() {
        Log.Warn(toString(arguments));
    }

    console.error = function() {
        Log.Error(toString(arguments));
    }

    console.trace = function() {
        Log.Info(toString(arguments) + "\n" + getStack(new Error()) + "\n");
    }

    console.assert = function(condition) {
        if (condition)
            return
        if (arguments.length > 1)
            Log.Assert(false, "Assertion failed: " + toString(Array.prototype.slice.call(arguments, 1)) + "\n" + getStack(new Error()) + "\n");
        else
            Log.Assert(false, "Assertion failed: console.assert\n" + getStack(new Error()) + "\n");
    }

    const timeRecorder = new Map();

    console.time = function(name){
        timeRecorder.set(name,+new Date);
    }

    console.timeEnd = function(name){
        const startTime = timeRecorder.get(name);
        if(startTime){
            console.log(String(name)+": "+(+new Date - startTime)+" ms");
            timeRecorder.delete(name);
        }else{
            console.warn("Timer '" + String(name)+ "' does not exist");
        };
    }

    global.console = console;
}
