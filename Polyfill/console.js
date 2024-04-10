var global = global || globalThis || (function () { return this; }());

const Log = World.Log;

if (Log) {
    delete World.Log;
    const console = {};

    function getStack(error, rm = false) {
        let stack = error.stack;
        stack = stack.replace(`${error.toString()}\n`, "");
        if (rm) stack = stack.substring(stack.indexOf("\n")+1);
        return stack;
    }

    const getKeysOfEnumerableProperties = (object, compareKeys) => {
        const rawKeys = Object.keys(object);
        const keys = compareKeys !== null ? rawKeys.sort(compareKeys) : rawKeys;
        if (Object.getOwnPropertySymbols) {
            Object.getOwnPropertySymbols(object).forEach(symbol => {
                if (Object.getOwnPropertyDescriptor(object, symbol).enumerable) {
                    keys.push(symbol);
                }
            });
        }
        return keys;
    };

    function printIteratorEntries(
        iterator,
        config,
        indentation,
        depth,
        refs,
        printer,
        separator = ': '
    ) {
        let result = '';
        let width = 0;
        let current = iterator.next();
        if (!current.done) {
            result += config.spacingOuter;
            const indentationNext = indentation + config.indent;
            while (!current.done) {
                result += indentationNext;
                if (width++ === config.maxWidth) {
                    result += '…';
                    break;
                }
                const name = printer(
                    current.value[0],
                    config,
                    indentationNext,
                    depth,
                    refs
                );
                const value = printer(
                    current.value[1],
                    config,
                    indentationNext,
                    depth,
                    refs
                );
                result += name + separator + value;
                current = iterator.next();
                if (!current.done) {
                    result += `,${config.spacingInner}`;
                } else if (!config.min) {
                    result += ',';
                }
            }
            result += config.spacingOuter + indentation;
        }
        return result;
    }

    function printIteratorValues(
        iterator,
        config,
        indentation,
        depth,
        refs,
        printer
    ) {
        let result = '';
        let width = 0;
        let current = iterator.next();
        if (!current.done) {
            result += config.spacingOuter;
            const indentationNext = indentation + config.indent;
            while (!current.done) {
                result += indentationNext;
                if (width++ === config.maxWidth) {
                    result += '…';
                    break;
                }
                result += printer(current.value, config, indentationNext, depth, refs);
                current = iterator.next();
                if (!current.done) {
                    result += `,${config.spacingInner}`;
                } else if (!config.min) {
                    result += ',';
                }
            }
            result += config.spacingOuter + indentation;
        }
        return result;
    }

    function printListItems(list, config, indentation, depth, refs, printer) {
        let result = '';
        if (list.length) {
            result += config.spacingOuter;
            const indentationNext = indentation + config.indent;
            for (let i = 0; i < list.length; i++) {
                result += indentationNext;
                if (i === config.maxWidth) {
                    result += '…';
                    break;
                }
                if (i in list) {
                    result += printer(list[i], config, indentationNext, depth, refs);
                }
                if (i < list.length - 1) {
                    result += `,${config.spacingInner}`;
                } else if (!config.min) {
                    result += ',';
                }
            }
            result += config.spacingOuter + indentation;
        }
        return result;
    }

    function printObjectProperties(val, config, indentation, depth, refs, printer) {
        let result = '';
        const keys = getKeysOfEnumerableProperties(val, config.compareKeys);
        if (keys.length) {
            result += config.spacingOuter;
            const indentationNext = indentation + config.indent;
            for (let i = 0; i < keys.length; i++) {
                const key = keys[i];
                const name = printer(key, config, indentationNext, depth, refs);
                const value = printer(val[key], config, indentationNext, depth, refs);
                result += `${indentationNext + name}: ${value}`;
                if (i < keys.length - 1) {
                    result += `,${config.spacingInner}`;
                } else if (!config.min) {
                    result += ',';
                }
            }
            result += config.spacingOuter + indentation;
        }
        return result;
    }

    const toString = Object.prototype.toString;
    const toISOString = Date.prototype.toISOString;
    const errorToString = Error.prototype.toString;
    const regExpToString = RegExp.prototype.toString;

    const getConstructorName = val =>
        (typeof val.constructor === 'function' && val.constructor.name) || 'Object';

    const isWindow = val => typeof window !== 'undefined' && val === window;
    const SYMBOL_REGEXP = /^Symbol\((.*)\)(.*)$/;
    const NEWLINE_REGEXP = /\n/gi;

    class PrettyFormatPluginError extends Error {
        constructor(message, stack) {
            super(message);
            this.stack = stack;
            this.name = this.constructor.name;
        }
    }

    function isToStringedArrayType(toStringed) {
        return (
            toStringed === '[object Array]' ||
            toStringed === '[object ArrayBuffer]' ||
            toStringed === '[object DataView]' ||
            toStringed === '[object Float32Array]' ||
            toStringed === '[object Float64Array]' ||
            toStringed === '[object Int8Array]' ||
            toStringed === '[object Int16Array]' ||
            toStringed === '[object Int32Array]' ||
            toStringed === '[object Uint8Array]' ||
            toStringed === '[object Uint8ClampedArray]' ||
            toStringed === '[object Uint16Array]' ||
            toStringed === '[object Uint32Array]'
        );
    }

    function printNumber(val) {
        return Object.is(val, -0) ? '-0' : String(val);
    }

    function printBigInt(val) {
        return String(`${val}n`);
    }

    function printFunction(val, printFunctionName) {
        if (!printFunctionName) {
            return '[Function]';
        }
        return `[Function${val.name ? ': ' + val.name : ' (anonymous)'}]`;
    }

    function printClass(val, printClassName) {
        if (!printClassName) {
            return '[class]';
        }
        return `[class ${val.name == "" ? "(anonymous)" : val.name}]`;
    }

    function printSymbol(val) {
        return String(val).replace(SYMBOL_REGEXP, 'Symbol($1)');
    }

    function printError(val) {
        return `[${errorToString.call(val)}]`;
    }

    function printBasicValue(val, printFunctionName, escapeRegex, escapeString) {
        if (val === true || val === false) {
            return `${val}`;
        }
        if (val === undefined) {
            return 'undefined';
        }
        if (val === null) {
            return 'null';
        }
        const typeOf = typeof val;
        if (typeOf === 'number') {
            return printNumber(val);
        }
        if (typeOf === 'bigint') {
            return printBigInt(val);
        }
        if (typeOf === 'string') {
            if (escapeString) {
                return `"${val.replace(/"|\\/g, '\\$&')}"`;
            }
            return `"${val}"`;
        }
        if (typeOf === 'function') {
            if (val.toString().startsWith('class')) return printClass(val, printFunctionName);
            return printFunction(val, printFunctionName);
        }
        if (typeOf === 'symbol') {
            return printSymbol(val);
        }
        const toStringed = toString.call(val);
        if (toStringed === '[object WeakMap]') {
            return 'WeakMap {}';
        }
        if (toStringed === '[object WeakSet]') {
            return 'WeakSet {}';
        }
        if (
            toStringed === '[object Function]' ||
            toStringed === '[object GeneratorFunction]'
        ) {
            return printFunction(val, printFunctionName);
        }
        if (toStringed === '[object Symbol]') {
            return printSymbol(val);
        }
        if (toStringed === '[object Date]') {
            return isNaN(+val) ? 'Date { NaN }' : toISOString.call(val);
        }
        if (toStringed === '[object Error]') {
            return printError(val);
        }
        if (toStringed === '[object RegExp]') {
            if (escapeRegex) {
                // https://github.com/benjamingr/RegExp.escape/blob/main/polyfill.js
                return regExpToString.call(val).replace(/[\\^$*+?.()|[\]{}]/g, '\\$&');
            }
            return regExpToString.call(val);
        }
        if (val instanceof Error) {
            return printError(val);
        }
        return null;
    }

    function printComplexValue(
        val,
        config,
        indentation,
        depth,
        refs,
        hasCalledToJSON
    ) {
        if (refs.indexOf(val) !== -1) {
            return '[Circular]';
        }
        refs = refs.slice();
        refs.push(val);
        const hitMaxDepth = ++depth > config.maxDepth;
        const min = config.min;
        if (
            config.callToJSON &&
            !hitMaxDepth &&
            val.toJSON &&
            typeof val.toJSON === 'function' &&
            !hasCalledToJSON
        ) {
            return printer(val.toJSON(), config, indentation, depth, refs, true);
        }
        const toStringed = toString.call(val);
        if (toStringed === '[object Arguments]') {
            return hitMaxDepth
                ? '[Arguments]'
                : `${min ? '' : 'Arguments '}[${(0, printListItems)(
                    val,
                    config,
                    indentation,
                    depth,
                    refs,
                    printer
                )}]`;
        }
        if (isToStringedArrayType(toStringed)) {
            return hitMaxDepth
                ? `[${val.constructor.name}]`
                : `${min
                    ? ''
                    : !config.printBasicPrototype && val.constructor.name === 'Array'
                        ? ''
                        : `${val.constructor.name} `
                }[${(0, printListItems)(
                    val,
                    config,
                    indentation,
                    depth,
                    refs,
                    printer
                )}]`;
        }
        if (toStringed === '[object Map]') {
            return hitMaxDepth
                ? '[Map]'
                : `Map {${(0, printIteratorEntries)(
                    val.entries(),
                    config,
                    indentation,
                    depth,
                    refs,
                    printer,
                    ' => '
                )}}`;
        }
        if (toStringed === '[object Set]') {
            return hitMaxDepth
                ? '[Set]'
                : `Set {${(0, printIteratorValues)(
                    val.values(),
                    config,
                    indentation,
                    depth,
                    refs,
                    printer
                )}}`;
        }

        return hitMaxDepth || isWindow(val)
            ? `[${getConstructorName(val)}]`
            : `${min
                ? ''
                : !config.printBasicPrototype && getConstructorName(val) === 'Object'
                    ? ''
                    : `${getConstructorName(val)} `
            }{${(0, printObjectProperties)(
                val,
                config,
                indentation,
                depth,
                refs,
                printer
            )}}`;
    }

    function isNewPlugin(plugin) {
        return plugin.serialize != null;
    }

    function printPlugin(plugin, val, config, indentation, depth, refs) {
        let printed;
        try {
            printed = isNewPlugin(plugin)
                ? plugin.serialize(val, config, indentation, depth, refs, printer)
                : plugin.print(
                    val,
                    valChild => printer(valChild, config, indentation, depth, refs),
                    str => {
                        const indentationNext = indentation + config.indent;
                        return (
                            indentationNext +
                            str.replace(NEWLINE_REGEXP, `\n${indentationNext}`)
                        );
                    },
                    {
                        edgeSpacing: config.spacingOuter,
                        min: config.min,
                        spacing: config.spacingInner
                    },
                    config.colors
                );
        } catch (error) {
            throw new PrettyFormatPluginError(error.message, error.stack);
        }
        if (typeof printed !== 'string') {
            throw new Error(
                `pretty-format: Plugin must return type "string" but instead returned "${typeof printed}".`
            );
        }
        return printed;
    }

    function findPlugin(plugins, val) {
        for (let p = 0; p < plugins.length; p++) {
            try {
                if (plugins[p].test(val)) {
                    return plugins[p];
                }
            } catch (error) {
                throw new PrettyFormatPluginError(error.message, error.stack);
            }
        }
        return null;
    }

    function printer(val, config, indentation, depth, refs, hasCalledToJSON) {
        const plugin = findPlugin(config.plugins, val);
        if (plugin !== null) {
            return printPlugin(plugin, val, config, indentation, depth, refs);
        }
        const basicResult = printBasicValue(
            val,
            config.printFunctionName,
            config.escapeRegex,
            config.escapeString
        );
        if (basicResult !== null) {
            return basicResult;
        }
        return printComplexValue(
            val,
            config,
            indentation,
            depth,
            refs,
            hasCalledToJSON
        );
    }

    const DEFAULT_THEME = {
        comment: 'gray',
        content: 'reset',
        prop: 'yellow',
        tag: 'cyan',
        value: 'green'
    };
    const DEFAULT_THEME_KEYS = Object.keys(DEFAULT_THEME);

    const toOptionsSubtype = options => options;
    const DEFAULT_OPTIONS = toOptionsSubtype({
        callToJSON: true,
        compareKeys: undefined,
        escapeRegex: false,
        escapeString: true,
        highlight: false,
        indent: 2,
        maxDepth: Infinity,
        maxWidth: Infinity,
        min: false,
        plugins: [],
        printBasicPrototype: true,
        printFunctionName: true,
        theme: DEFAULT_THEME
    });

    function validateOptions(options) {
        Object.keys(options).forEach(key => {
            if (!Object.prototype.hasOwnProperty.call(DEFAULT_OPTIONS, key)) {
                throw new Error(`pretty-format: Unknown option "${key}".`);
            }
        });
        if (options.min && options.indent !== undefined && options.indent !== 0) {
            throw new Error(
                'pretty-format: Options "min" and "indent" cannot be used together.'
            );
        }
        if (options.theme !== undefined) {
            if (options.theme === null) {
                throw new Error('pretty-format: Option "theme" must not be null.');
            }
            if (typeof options.theme !== 'object') {
                throw new Error(
                    `pretty-format: Option "theme" must be of type "object" but instead received "${typeof options.theme}".`
                );
            }
        }
    }

    const getColorsHighlight = options =>
        DEFAULT_THEME_KEYS.reduce((colors, key) => {
            const value =
                options.theme && options.theme[key] !== undefined
                    ? options.theme[key]
                    : DEFAULT_THEME[key];
            const color = value && _ansiStyles.default[value];
            if (
                color &&
                typeof color.close === 'string' &&
                typeof color.open === 'string'
            ) {
                colors[key] = color;
            } else {
                throw new Error(
                    `pretty-format: Option "theme" has a key "${key}" whose value "${value}" is undefined in ansi-styles.`
                );
            }
            return colors;
        }, Object.create(null));
    const getColorsEmpty = () =>
        DEFAULT_THEME_KEYS.reduce((colors, key) => {
            colors[key] = {
                close: '',
                open: ''
            };
            return colors;
        }, Object.create(null));
    const getPrintFunctionName = options =>
        options?.printFunctionName ?? DEFAULT_OPTIONS.printFunctionName;
    const getEscapeRegex = options =>
        options?.escapeRegex ?? DEFAULT_OPTIONS.escapeRegex;
    const getEscapeString = options =>
        options?.escapeString ?? DEFAULT_OPTIONS.escapeString;
    const getConfig = options => ({
        callToJSON: options?.callToJSON ?? DEFAULT_OPTIONS.callToJSON,
        colors: options?.highlight ? getColorsHighlight(options) : getColorsEmpty(),
        compareKeys:
            typeof options?.compareKeys === 'function' || options?.compareKeys === null
                ? options.compareKeys
                : DEFAULT_OPTIONS.compareKeys,
        escapeRegex: getEscapeRegex(options),
        escapeString: getEscapeString(options),
        indent: options?.min
            ? ''
            : createIndent(options?.indent ?? DEFAULT_OPTIONS.indent),
        maxDepth: options?.maxDepth ?? DEFAULT_OPTIONS.maxDepth,
        maxWidth: options?.maxWidth ?? DEFAULT_OPTIONS.maxWidth,
        min: options?.min ?? DEFAULT_OPTIONS.min,
        plugins: options?.plugins ?? DEFAULT_OPTIONS.plugins,
        printBasicPrototype: options?.printBasicPrototype ?? true,
        printFunctionName: getPrintFunctionName(options),
        spacingInner: options?.min ? ' ' : '\n',
        spacingOuter: options?.min ? '' : '\n'
    });

    function createIndent(indent) {
        return new Array(indent + 1).join(' ');
    }

    function format(val, options) {
        if (options) {
            validateOptions(options);
        }
        const basicResult = printBasicValue(
            val,
            getPrintFunctionName(options),
            getEscapeRegex(options),
            getEscapeString(options)
        );
        if (basicResult !== null) {
            return basicResult;
        }
        return printComplexValue(val, getConfig(options), '', 0, []);
    }

    function argsToString(args) {
        return Array.prototype.map.call(args, x => {
            try {
                return typeof x ==="string" ? x : x instanceof Error ? `${x}\n${getStack(x)}` : format(x);
            } catch (err) {
                return err;
            }
        }).join(' ');
    }

    console.format = (val, options) => format(val, options);

    console.debug = function() {
        Log.Debug(argsToString(arguments));
    }

    console.log = function() {
        Log.Info(argsToString(arguments));
    }

    console.info = function() {
        Log.Info(argsToString(arguments));
    }

    console.warn = function() {
        Log.Warn(argsToString(arguments));
    }

    console.error = function() {
        Log.Error(argsToString(arguments));
    }

    console.trace = function() {
        Log.Trace(argsToString(arguments) + "\n" + getStack(new Error("Trace"), true));
    }

    console.assert = function(condition) {
        if (condition)
            return
        if (arguments.length > 1)
            Log.Assert(false, "Assertion failed: " + argsToString(Array.prototype.slice.call(arguments, 1)) + "\n" + getStack(new Error()));
        else
            Log.Assert(false, "Assertion failed: console.assert\n" + getStack(new Error()));
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
