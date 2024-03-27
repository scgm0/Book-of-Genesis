
    declare namespace CS {
    //keep type incompatibility / 此属性保持类型不兼容
    const __keep_incompatibility: unique symbol;
    interface $Ref<T> {
        value: T
    }
    namespace System {
        interface Array$1<T> extends System.Array {
            get_Item(index: number):T;
            set_Item(index: number, value: T):void;
        }
    }
    type $Task<T> = System.Threading.Tasks.Task$1<T>
    namespace System {
        class Object
        {
            protected [__keep_incompatibility]: never;
        }
        interface IAsyncResult
        {
        }
        interface IDisposable
        {
        }
        class ValueType extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        class Void extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        class Int32 extends System.ValueType implements System.IComparable, System.IConvertible, System.ISpanFormattable, System.IFormattable, System.IComparable$1<number>, System.IEquatable$1<number>, System.Numerics.IBinaryInteger$1<number>, System.Numerics.IBinaryNumber$1<number>, System.Numerics.IBitwiseOperators$3<number, number, number>, System.Numerics.INumber$1<number>, System.Numerics.IComparisonOperators$3<number, number, boolean>, System.Numerics.IEqualityOperators$3<number, number, boolean>, System.Numerics.IModulusOperators$3<number, number, number>, System.Numerics.INumberBase$1<number>, System.Numerics.IAdditionOperators$3<number, number, number>, System.Numerics.IAdditiveIdentity$2<number, number>, System.Numerics.IDecrementOperators$1<number>, System.Numerics.IDivisionOperators$3<number, number, number>, System.Numerics.IIncrementOperators$1<number>, System.Numerics.IMultiplicativeIdentity$2<number, number>, System.Numerics.IMultiplyOperators$3<number, number, number>, System.ISpanParsable$1<number>, System.IParsable$1<number>, System.Numerics.ISubtractionOperators$3<number, number, number>, System.Numerics.IUnaryPlusOperators$2<number, number>, System.Numerics.IUnaryNegationOperators$2<number, number>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<number>, System.Numerics.IShiftOperators$3<number, number, number>, System.Numerics.IMinMaxValue$1<number>, System.Numerics.ISignedNumber$1<number>, System.IBinaryIntegerParseAndFormatInfo$1<number>
        {
            protected [__keep_incompatibility]: never;
        }
        interface IComparable
        {
        }
        interface IConvertible
        {
        }
        interface ISpanFormattable extends System.IFormattable
        {
        }
        interface IFormattable
        {
        }
        interface IComparable$1<T>
        {
        }
        interface IEquatable$1<T>
        {
        }
        class Boolean extends System.ValueType implements System.IComparable, System.IConvertible, System.IComparable$1<boolean>, System.IEquatable$1<boolean>, System.ISpanParsable$1<boolean>, System.IParsable$1<boolean>
        {
            protected [__keep_incompatibility]: never;
        }
        interface ISpanParsable$1<TSelf> extends System.IParsable$1<TSelf>
        {
        }
        interface IParsable$1<TSelf>
        {
        }
        interface IUtf8SpanFormattable
        {
        }
        interface IUtf8SpanParsable$1<TSelf>
        {
        }
        interface IBinaryIntegerParseAndFormatInfo$1<TSelf> extends System.Numerics.IBinaryInteger$1<TSelf>, System.Numerics.IBinaryNumber$1<TSelf>, System.Numerics.IBitwiseOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumber$1<TSelf>, System.IComparable, System.IComparable$1<TSelf>, System.Numerics.IComparisonOperators$3<TSelf, TSelf, boolean>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IModulusOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumberBase$1<TSelf>, System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>, System.Numerics.IShiftOperators$3<TSelf, number, TSelf>, System.Numerics.IMinMaxValue$1<TSelf>
        {
        }
        class Nullable$1<T> extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        class Exception extends System.Object implements System.Runtime.Serialization.ISerializable
        {
            protected [__keep_incompatibility]: never;
        }
        class AggregateException extends System.Exception implements System.Runtime.Serialization.ISerializable
        {
            protected [__keep_incompatibility]: never;
        }
        class Enum extends System.ValueType implements System.IComparable, System.ISpanFormattable, System.IFormattable, System.IConvertible
        {
            protected [__keep_incompatibility]: never;
        }
        class TimeSpan extends System.ValueType implements System.IComparable, System.IComparable$1<System.TimeSpan>, System.IEquatable$1<System.TimeSpan>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<System.TimeSpan>, System.IParsable$1<System.TimeSpan>, System.IUtf8SpanFormattable
        {
            protected [__keep_incompatibility]: never;
        }
        class TimeProvider extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        class Delegate extends System.Object implements System.ICloneable, System.Runtime.Serialization.ISerializable
        {
            protected [__keep_incompatibility]: never;
        }
        interface ICloneable
        {
        }
        interface MulticastDelegate
        { 
        (...args:any[]) : any; 
        Invoke?: (...args:any[]) => any;
        }
        var MulticastDelegate: { new (func: (...args:any[]) => any): MulticastDelegate; }
        interface Action$1<T>
        { 
        (obj: T) : void; 
        Invoke?: (obj: T) => void;
        }
        interface Action$2<T1, T2>
        { 
        (arg1: T1, arg2: T2) : void; 
        Invoke?: (arg1: T1, arg2: T2) => void;
        }
        interface Func$2<T, TResult>
        { 
        (arg: T) : TResult; 
        Invoke?: (arg: T) => TResult;
        }
        interface Func$3<T1, T2, TResult>
        { 
        (arg1: T1, arg2: T2) : TResult; 
        Invoke?: (arg1: T1, arg2: T2) => TResult;
        }
        class Array extends System.Object implements System.ICloneable, System.Collections.IList, System.Collections.ICollection, System.Collections.IEnumerable, System.Collections.IStructuralComparable, System.Collections.IStructuralEquatable
        {
            protected [__keep_incompatibility]: never;
        }
        interface Action
        { 
        () : void; 
        Invoke?: () => void;
        }
        var Action: { new (func: () => void): Action; }
        interface Func$1<TResult>
        { 
        () : TResult; 
        Invoke?: () => TResult;
        }
        class String extends System.Object implements System.IComparable, System.Collections.IEnumerable, System.IConvertible, System.Collections.Generic.IEnumerable$1<number>, System.IComparable$1<string>, System.IEquatable$1<string>, System.ICloneable, System.ISpanParsable$1<string>, System.IParsable$1<string>
        {
            protected [__keep_incompatibility]: never;
        }
        class Char extends System.ValueType implements System.IComparable, System.IComparable$1<number>, System.IEquatable$1<number>, System.IConvertible, System.ISpanFormattable, System.IFormattable, System.Numerics.IBinaryInteger$1<number>, System.Numerics.IBinaryNumber$1<number>, System.Numerics.IBitwiseOperators$3<number, number, number>, System.Numerics.INumber$1<number>, System.Numerics.IComparisonOperators$3<number, number, boolean>, System.Numerics.IEqualityOperators$3<number, number, boolean>, System.Numerics.IModulusOperators$3<number, number, number>, System.Numerics.INumberBase$1<number>, System.Numerics.IAdditionOperators$3<number, number, number>, System.Numerics.IAdditiveIdentity$2<number, number>, System.Numerics.IDecrementOperators$1<number>, System.Numerics.IDivisionOperators$3<number, number, number>, System.Numerics.IIncrementOperators$1<number>, System.Numerics.IMultiplicativeIdentity$2<number, number>, System.Numerics.IMultiplyOperators$3<number, number, number>, System.ISpanParsable$1<number>, System.IParsable$1<number>, System.Numerics.ISubtractionOperators$3<number, number, number>, System.Numerics.IUnaryPlusOperators$2<number, number>, System.Numerics.IUnaryNegationOperators$2<number, number>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<number>, System.Numerics.IShiftOperators$3<number, number, number>, System.Numerics.IMinMaxValue$1<number>, System.Numerics.IUnsignedNumber$1<number>, System.IUtfChar$1<number>, System.IBinaryIntegerParseAndFormatInfo$1<number>
        {
            protected [__keep_incompatibility]: never;
        }
        interface IUtfChar$1<TSelf> extends System.Numerics.IBinaryInteger$1<TSelf>, System.Numerics.IBinaryNumber$1<TSelf>, System.Numerics.IBitwiseOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumber$1<TSelf>, System.IComparable, System.IComparable$1<TSelf>, System.Numerics.IComparisonOperators$3<TSelf, TSelf, boolean>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IModulusOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumberBase$1<TSelf>, System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>, System.Numerics.IShiftOperators$3<TSelf, number, TSelf>
        {
        }
        class Type extends System.Reflection.MemberInfo implements System.Reflection.ICustomAttributeProvider, System.Reflection.IReflect
        {
            protected [__keep_incompatibility]: never;
            public static Delimiter : number
            public static EmptyTypes : System.Array$1<System.Type>
            public static Missing : any
            public static FilterAttribute : System.Reflection.MemberFilter
            public static FilterName : System.Reflection.MemberFilter
            public static FilterNameIgnoreCase : System.Reflection.MemberFilter
            public get IsInterface(): boolean;
            public get MemberType(): System.Reflection.MemberTypes;
            public get Namespace(): string;
            public get AssemblyQualifiedName(): string;
            public get FullName(): string;
            public get Assembly(): System.Reflection.Assembly;
            public get Module(): System.Reflection.Module;
            public get IsNested(): boolean;
            public get DeclaringType(): System.Type;
            public get DeclaringMethod(): System.Reflection.MethodBase;
            public get ReflectedType(): System.Type;
            public get UnderlyingSystemType(): System.Type;
            public get IsTypeDefinition(): boolean;
            public get IsArray(): boolean;
            public get IsByRef(): boolean;
            public get IsPointer(): boolean;
            public get IsConstructedGenericType(): boolean;
            public get IsGenericParameter(): boolean;
            public get IsGenericTypeParameter(): boolean;
            public get IsGenericMethodParameter(): boolean;
            public get IsGenericType(): boolean;
            public get IsGenericTypeDefinition(): boolean;
            public get IsSZArray(): boolean;
            public get IsVariableBoundArray(): boolean;
            public get IsByRefLike(): boolean;
            public get IsFunctionPointer(): boolean;
            public get IsUnmanagedFunctionPointer(): boolean;
            public get HasElementType(): boolean;
            public get GenericTypeArguments(): System.Array$1<System.Type>;
            public get GenericParameterPosition(): number;
            public get GenericParameterAttributes(): System.Reflection.GenericParameterAttributes;
            public get Attributes(): System.Reflection.TypeAttributes;
            public get IsAbstract(): boolean;
            public get IsImport(): boolean;
            public get IsSealed(): boolean;
            public get IsSpecialName(): boolean;
            public get IsClass(): boolean;
            public get IsNestedAssembly(): boolean;
            public get IsNestedFamANDAssem(): boolean;
            public get IsNestedFamily(): boolean;
            public get IsNestedFamORAssem(): boolean;
            public get IsNestedPrivate(): boolean;
            public get IsNestedPublic(): boolean;
            public get IsNotPublic(): boolean;
            public get IsPublic(): boolean;
            public get IsAutoLayout(): boolean;
            public get IsExplicitLayout(): boolean;
            public get IsLayoutSequential(): boolean;
            public get IsAnsiClass(): boolean;
            public get IsAutoClass(): boolean;
            public get IsUnicodeClass(): boolean;
            public get IsCOMObject(): boolean;
            public get IsContextful(): boolean;
            public get IsEnum(): boolean;
            public get IsMarshalByRef(): boolean;
            public get IsPrimitive(): boolean;
            public get IsValueType(): boolean;
            public get IsSignatureType(): boolean;
            public get IsSecurityCritical(): boolean;
            public get IsSecuritySafeCritical(): boolean;
            public get IsSecurityTransparent(): boolean;
            public get StructLayoutAttribute(): System.Runtime.InteropServices.StructLayoutAttribute;
            public get TypeInitializer(): System.Reflection.ConstructorInfo;
            public get TypeHandle(): System.RuntimeTypeHandle;
            public get GUID(): System.Guid;
            public get BaseType(): System.Type;
            public static get DefaultBinder(): System.Reflection.Binder;
            public get ContainsGenericParameters(): boolean;
            public get IsVisible(): boolean;
            public static GetType ($typeName: string, $throwOnError: boolean, $ignoreCase: boolean) : System.Type
            public static GetType ($typeName: string, $throwOnError: boolean) : System.Type
            public static GetType ($typeName: string) : System.Type
            public static GetType ($typeName: string, $assemblyResolver: System.Func$2<System.Reflection.AssemblyName, System.Reflection.Assembly>, $typeResolver: System.Func$4<System.Reflection.Assembly, string, boolean, System.Type>) : System.Type
            public static GetType ($typeName: string, $assemblyResolver: System.Func$2<System.Reflection.AssemblyName, System.Reflection.Assembly>, $typeResolver: System.Func$4<System.Reflection.Assembly, string, boolean, System.Type>, $throwOnError: boolean) : System.Type
            public static GetType ($typeName: string, $assemblyResolver: System.Func$2<System.Reflection.AssemblyName, System.Reflection.Assembly>, $typeResolver: System.Func$4<System.Reflection.Assembly, string, boolean, System.Type>, $throwOnError: boolean, $ignoreCase: boolean) : System.Type
            public static GetTypeFromHandle ($handle: System.RuntimeTypeHandle) : System.Type
            public GetType () : System.Type
            public GetElementType () : System.Type
            public GetArrayRank () : number
            public GetGenericTypeDefinition () : System.Type
            public GetGenericArguments () : System.Array$1<System.Type>
            public GetOptionalCustomModifiers () : System.Array$1<System.Type>
            public GetRequiredCustomModifiers () : System.Array$1<System.Type>
            public GetGenericParameterConstraints () : System.Array$1<System.Type>
            public IsAssignableTo ($targetType: System.Type) : boolean
            public GetConstructor ($types: System.Array$1<System.Type>) : System.Reflection.ConstructorInfo
            public GetConstructor ($bindingAttr: System.Reflection.BindingFlags, $types: System.Array$1<System.Type>) : System.Reflection.ConstructorInfo
            public GetConstructor ($bindingAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.ConstructorInfo
            public GetConstructor ($bindingAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $callConvention: System.Reflection.CallingConventions, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.ConstructorInfo
            public GetConstructors () : System.Array$1<System.Reflection.ConstructorInfo>
            public GetConstructors ($bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.ConstructorInfo>
            public GetEvent ($name: string) : System.Reflection.EventInfo
            public GetEvent ($name: string, $bindingAttr: System.Reflection.BindingFlags) : System.Reflection.EventInfo
            public GetEvents () : System.Array$1<System.Reflection.EventInfo>
            public GetEvents ($bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.EventInfo>
            public GetField ($name: string) : System.Reflection.FieldInfo
            public GetField ($name: string, $bindingAttr: System.Reflection.BindingFlags) : System.Reflection.FieldInfo
            public GetFields () : System.Array$1<System.Reflection.FieldInfo>
            public GetFields ($bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.FieldInfo>
            public GetFunctionPointerCallingConventions () : System.Array$1<System.Type>
            public GetFunctionPointerReturnType () : System.Type
            public GetFunctionPointerParameterTypes () : System.Array$1<System.Type>
            public GetMember ($name: string) : System.Array$1<System.Reflection.MemberInfo>
            public GetMember ($name: string, $bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.MemberInfo>
            public GetMember ($name: string, $type: System.Reflection.MemberTypes, $bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.MemberInfo>
            public GetMembers () : System.Array$1<System.Reflection.MemberInfo>
            public GetMemberWithSameMetadataDefinitionAs ($member: System.Reflection.MemberInfo) : System.Reflection.MemberInfo
            public GetMembers ($bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.MemberInfo>
            public GetMethod ($name: string) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $bindingAttr: System.Reflection.BindingFlags) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $bindingAttr: System.Reflection.BindingFlags, $types: System.Array$1<System.Type>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $types: System.Array$1<System.Type>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $bindingAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $bindingAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $callConvention: System.Reflection.CallingConventions, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $genericParameterCount: number, $types: System.Array$1<System.Type>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $genericParameterCount: number, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $genericParameterCount: number, $bindingAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.MethodInfo
            public GetMethod ($name: string, $genericParameterCount: number, $bindingAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $callConvention: System.Reflection.CallingConventions, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.MethodInfo
            public GetMethods () : System.Array$1<System.Reflection.MethodInfo>
            public GetMethods ($bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.MethodInfo>
            public GetNestedType ($name: string) : System.Type
            public GetNestedType ($name: string, $bindingAttr: System.Reflection.BindingFlags) : System.Type
            public GetNestedTypes () : System.Array$1<System.Type>
            public GetNestedTypes ($bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Type>
            public GetProperty ($name: string) : System.Reflection.PropertyInfo
            public GetProperty ($name: string, $bindingAttr: System.Reflection.BindingFlags) : System.Reflection.PropertyInfo
            public GetProperty ($name: string, $returnType: System.Type) : System.Reflection.PropertyInfo
            public GetProperty ($name: string, $types: System.Array$1<System.Type>) : System.Reflection.PropertyInfo
            public GetProperty ($name: string, $returnType: System.Type, $types: System.Array$1<System.Type>) : System.Reflection.PropertyInfo
            public GetProperty ($name: string, $returnType: System.Type, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.PropertyInfo
            public GetProperty ($name: string, $bindingAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $returnType: System.Type, $types: System.Array$1<System.Type>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>) : System.Reflection.PropertyInfo
            public GetProperties () : System.Array$1<System.Reflection.PropertyInfo>
            public GetProperties ($bindingAttr: System.Reflection.BindingFlags) : System.Array$1<System.Reflection.PropertyInfo>
            public GetDefaultMembers () : System.Array$1<System.Reflection.MemberInfo>
            public static GetTypeHandle ($o: any) : System.RuntimeTypeHandle
            public static GetTypeArray ($args: System.Array$1<any>) : System.Array$1<System.Type>
            public static GetTypeCode ($type: System.Type) : System.TypeCode
            public static GetTypeFromCLSID ($clsid: System.Guid) : System.Type
            public static GetTypeFromCLSID ($clsid: System.Guid, $throwOnError: boolean) : System.Type
            public static GetTypeFromCLSID ($clsid: System.Guid, $server: string) : System.Type
            public static GetTypeFromCLSID ($clsid: System.Guid, $server: string, $throwOnError: boolean) : System.Type
            public static GetTypeFromProgID ($progID: string) : System.Type
            public static GetTypeFromProgID ($progID: string, $throwOnError: boolean) : System.Type
            public static GetTypeFromProgID ($progID: string, $server: string) : System.Type
            public static GetTypeFromProgID ($progID: string, $server: string, $throwOnError: boolean) : System.Type
            public InvokeMember ($name: string, $invokeAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $target: any, $args: System.Array$1<any>) : any
            public InvokeMember ($name: string, $invokeAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $target: any, $args: System.Array$1<any>, $culture: System.Globalization.CultureInfo) : any
            public InvokeMember ($name: string, $invokeAttr: System.Reflection.BindingFlags, $binder: System.Reflection.Binder, $target: any, $args: System.Array$1<any>, $modifiers: System.Array$1<System.Reflection.ParameterModifier>, $culture: System.Globalization.CultureInfo, $namedParameters: System.Array$1<string>) : any
            public GetInterface ($name: string) : System.Type
            public GetInterface ($name: string, $ignoreCase: boolean) : System.Type
            public GetInterfaces () : System.Array$1<System.Type>
            public GetInterfaceMap ($interfaceType: System.Type) : System.Reflection.InterfaceMapping
            public IsInstanceOfType ($o: any) : boolean
            public IsEquivalentTo ($other: System.Type) : boolean
            public GetEnumUnderlyingType () : System.Type
            public GetEnumValues () : System.Array
            public GetEnumValuesAsUnderlyingType () : System.Array
            public MakeArrayType () : System.Type
            public MakeArrayType ($rank: number) : System.Type
            public MakeByRefType () : System.Type
            public MakeGenericType (...typeArguments: System.Type[]) : System.Type
            public MakePointerType () : System.Type
            public static MakeGenericSignatureType ($genericTypeDefinition: System.Type, ...typeArguments: System.Type[]) : System.Type
            public static MakeGenericMethodParameter ($position: number) : System.Type
            public Equals ($o: any) : boolean
            public Equals ($o: System.Type) : boolean
            public static op_Equality ($left: System.Type, $right: System.Type) : boolean
            public static op_Inequality ($left: System.Type, $right: System.Type) : boolean
            public IsEnumDefined ($value: any) : boolean
            public GetEnumName ($value: any) : string
            public GetEnumNames () : System.Array$1<string>
            public FindInterfaces ($filter: System.Reflection.TypeFilter, $filterCriteria: any) : System.Array$1<System.Type>
            public FindMembers ($memberType: System.Reflection.MemberTypes, $bindingAttr: System.Reflection.BindingFlags, $filter: System.Reflection.MemberFilter, $filterCriteria: any) : System.Array$1<System.Reflection.MemberInfo>
            public IsSubclassOf ($c: System.Type) : boolean
            public IsAssignableFrom ($c: System.Type) : boolean
        }
        interface Func$4<T1, T2, T3, TResult>
        { 
        (arg1: T1, arg2: T2, arg3: T3) : TResult; 
        Invoke?: (arg1: T1, arg2: T2, arg3: T3) => TResult;
        }
        class RuntimeTypeHandle extends System.ValueType implements System.IEquatable$1<System.RuntimeTypeHandle>, System.Runtime.Serialization.ISerializable
        {
            protected [__keep_incompatibility]: never;
        }
        class Attribute extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        enum TypeCode
        { Empty = 0, Object = 1, DBNull = 2, Boolean = 3, Char = 4, SByte = 5, Byte = 6, Int16 = 7, UInt16 = 8, Int32 = 9, UInt32 = 10, Int64 = 11, UInt64 = 12, Single = 13, Double = 14, Decimal = 15, DateTime = 16, String = 18 }
        class Guid extends System.ValueType implements System.ISpanFormattable, System.IFormattable, System.IComparable, System.IComparable$1<System.Guid>, System.IEquatable$1<System.Guid>, System.ISpanParsable$1<System.Guid>, System.IParsable$1<System.Guid>, System.IUtf8SpanFormattable
        {
            protected [__keep_incompatibility]: never;
        }
        interface IFormatProvider
        {
        }
    }
    namespace System.Threading.Tasks {
        class Task extends System.Object implements System.IAsyncResult, System.IDisposable
        {
            protected [__keep_incompatibility]: never;
            public get Id(): number;
            public static get CurrentId(): number | null;
            public get Exception(): System.AggregateException;
            public get Status(): System.Threading.Tasks.TaskStatus;
            public get IsCanceled(): boolean;
            public get IsCompleted(): boolean;
            public get IsCompletedSuccessfully(): boolean;
            public get CreationOptions(): System.Threading.Tasks.TaskCreationOptions;
            public get AsyncState(): any;
            public static get Factory(): System.Threading.Tasks.TaskFactory;
            public static get CompletedTask(): $Task<any>;
            public get IsFaulted(): boolean;
            public Start () : void
            public Start ($scheduler: System.Threading.Tasks.TaskScheduler) : void
            public RunSynchronously () : void
            public RunSynchronously ($scheduler: System.Threading.Tasks.TaskScheduler) : void
            public Dispose () : void
            public GetAwaiter () : System.Runtime.CompilerServices.TaskAwaiter
            public ConfigureAwait ($continueOnCapturedContext: boolean) : System.Runtime.CompilerServices.ConfiguredTaskAwaitable
            public ConfigureAwait ($options: System.Threading.Tasks.ConfigureAwaitOptions) : System.Runtime.CompilerServices.ConfiguredTaskAwaitable
            public static Yield () : System.Runtime.CompilerServices.YieldAwaitable
            public Wait () : void
            public Wait ($timeout: System.TimeSpan) : boolean
            public Wait ($timeout: System.TimeSpan, $cancellationToken: System.Threading.CancellationToken) : boolean
            public Wait ($cancellationToken: System.Threading.CancellationToken) : void
            public Wait ($millisecondsTimeout: number) : boolean
            public Wait ($millisecondsTimeout: number, $cancellationToken: System.Threading.CancellationToken) : boolean
            public WaitAsync ($cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public WaitAsync ($timeout: System.TimeSpan) : $Task<any>
            public WaitAsync ($timeout: System.TimeSpan, $timeProvider: System.TimeProvider) : $Task<any>
            public WaitAsync ($timeout: System.TimeSpan, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public WaitAsync ($timeout: System.TimeSpan, $timeProvider: System.TimeProvider, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$1<$Task<any>>) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$1<$Task<any>>, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$1<$Task<any>>, $scheduler: System.Threading.Tasks.TaskScheduler) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$1<$Task<any>>, $continuationOptions: System.Threading.Tasks.TaskContinuationOptions) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$1<$Task<any>>, $cancellationToken: System.Threading.CancellationToken, $continuationOptions: System.Threading.Tasks.TaskContinuationOptions, $scheduler: System.Threading.Tasks.TaskScheduler) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$2<$Task<any>, any>, $state: any) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$2<$Task<any>, any>, $state: any, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$2<$Task<any>, any>, $state: any, $scheduler: System.Threading.Tasks.TaskScheduler) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$2<$Task<any>, any>, $state: any, $continuationOptions: System.Threading.Tasks.TaskContinuationOptions) : $Task<any>
            public ContinueWith ($continuationAction: System.Action$2<$Task<any>, any>, $state: any, $cancellationToken: System.Threading.CancellationToken, $continuationOptions: System.Threading.Tasks.TaskContinuationOptions, $scheduler: System.Threading.Tasks.TaskScheduler) : $Task<any>
            public static WaitAll (...tasks: $Task<any>[]) : void
            public static WaitAll ($tasks: System.Array$1<$Task<any>>, $timeout: System.TimeSpan) : boolean
            public static WaitAll ($tasks: System.Array$1<$Task<any>>, $millisecondsTimeout: number) : boolean
            public static WaitAll ($tasks: System.Array$1<$Task<any>>, $cancellationToken: System.Threading.CancellationToken) : void
            public static WaitAll ($tasks: System.Array$1<$Task<any>>, $millisecondsTimeout: number, $cancellationToken: System.Threading.CancellationToken) : boolean
            public static WaitAny (...tasks: $Task<any>[]) : number
            public static WaitAny ($tasks: System.Array$1<$Task<any>>, $timeout: System.TimeSpan) : number
            public static WaitAny ($tasks: System.Array$1<$Task<any>>, $cancellationToken: System.Threading.CancellationToken) : number
            public static WaitAny ($tasks: System.Array$1<$Task<any>>, $millisecondsTimeout: number) : number
            public static WaitAny ($tasks: System.Array$1<$Task<any>>, $millisecondsTimeout: number, $cancellationToken: System.Threading.CancellationToken) : number
            public static FromException ($exception: System.Exception) : $Task<any>
            public static FromCanceled ($cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public static Run ($action: System.Action) : $Task<any>
            public static Run ($action: System.Action, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public static Run ($function: System.Func$1<$Task<any>>) : $Task<any>
            public static Run ($function: System.Func$1<$Task<any>>, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public static Delay ($delay: System.TimeSpan) : $Task<any>
            public static Delay ($delay: System.TimeSpan, $timeProvider: System.TimeProvider) : $Task<any>
            public static Delay ($delay: System.TimeSpan, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public static Delay ($delay: System.TimeSpan, $timeProvider: System.TimeProvider, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public static Delay ($millisecondsDelay: number) : $Task<any>
            public static Delay ($millisecondsDelay: number, $cancellationToken: System.Threading.CancellationToken) : $Task<any>
            public static WhenAll ($tasks: System.Collections.Generic.IEnumerable$1<$Task<any>>) : $Task<any>
            public static WhenAll (...tasks: $Task<any>[]) : $Task<any>
            public static WhenAny (...tasks: $Task<any>[]) : System.Threading.Tasks.Task$1<$Task<any>>
            public static WhenAny ($task1: $Task<any>, $task2: $Task<any>) : System.Threading.Tasks.Task$1<$Task<any>>
            public static WhenAny ($tasks: System.Collections.Generic.IEnumerable$1<$Task<any>>) : System.Threading.Tasks.Task$1<$Task<any>>
            public constructor ($action: System.Action)
            public constructor ($action: System.Action, $cancellationToken: System.Threading.CancellationToken)
            public constructor ($action: System.Action, $creationOptions: System.Threading.Tasks.TaskCreationOptions)
            public constructor ($action: System.Action, $cancellationToken: System.Threading.CancellationToken, $creationOptions: System.Threading.Tasks.TaskCreationOptions)
            public constructor ($action: System.Action$1<any>, $state: any)
            public constructor ($action: System.Action$1<any>, $state: any, $cancellationToken: System.Threading.CancellationToken)
            public constructor ($action: System.Action$1<any>, $state: any, $creationOptions: System.Threading.Tasks.TaskCreationOptions)
            public constructor ($action: System.Action$1<any>, $state: any, $cancellationToken: System.Threading.CancellationToken, $creationOptions: System.Threading.Tasks.TaskCreationOptions)
        }
        class TaskScheduler extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        enum TaskStatus
        { Created = 0, WaitingForActivation = 1, WaitingToRun = 2, Running = 3, WaitingForChildrenToComplete = 4, RanToCompletion = 5, Canceled = 6, Faulted = 7 }
        enum TaskCreationOptions
        { None = 0, PreferFairness = 1, LongRunning = 2, AttachedToParent = 4, DenyChildAttach = 8, HideScheduler = 16, RunContinuationsAsynchronously = 64 }
        class TaskFactory extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        enum ConfigureAwaitOptions
        { None = 0, ContinueOnCapturedContext = 1, SuppressThrowing = 2, ForceYielding = 4 }
        enum TaskContinuationOptions
        { None = 0, PreferFairness = 1, LongRunning = 2, AttachedToParent = 4, DenyChildAttach = 8, HideScheduler = 16, LazyCancellation = 32, RunContinuationsAsynchronously = 64, NotOnRanToCompletion = 65536, NotOnFaulted = 131072, NotOnCanceled = 262144, OnlyOnRanToCompletion = 393216, OnlyOnFaulted = 327680, OnlyOnCanceled = 196608, ExecuteSynchronously = 524288 }
        class Task$1<TResult> extends System.Threading.Tasks.Task implements System.IAsyncResult, System.IDisposable
        {
            protected [__keep_incompatibility]: never;
        }
    }
    namespace System.Numerics {
        interface IBinaryInteger$1<TSelf> extends System.Numerics.IBinaryNumber$1<TSelf>, System.Numerics.IBitwiseOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumber$1<TSelf>, System.IComparable, System.IComparable$1<TSelf>, System.Numerics.IComparisonOperators$3<TSelf, TSelf, boolean>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IModulusOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumberBase$1<TSelf>, System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>, System.Numerics.IShiftOperators$3<TSelf, number, TSelf>
        {
        }
        interface IBinaryNumber$1<TSelf> extends System.Numerics.IBitwiseOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumber$1<TSelf>, System.IComparable, System.IComparable$1<TSelf>, System.Numerics.IComparisonOperators$3<TSelf, TSelf, boolean>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IModulusOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumberBase$1<TSelf>, System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>
        {
        }
        interface IBitwiseOperators$3<TSelf, TOther, TResult>
        {
        }
        interface INumber$1<TSelf> extends System.IComparable, System.IComparable$1<TSelf>, System.Numerics.IComparisonOperators$3<TSelf, TSelf, boolean>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IModulusOperators$3<TSelf, TSelf, TSelf>, System.Numerics.INumberBase$1<TSelf>, System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>
        {
        }
        interface IComparisonOperators$3<TSelf, TOther, TResult> extends System.Numerics.IEqualityOperators$3<TSelf, TOther, TResult>
        {
        }
        interface IEqualityOperators$3<TSelf, TOther, TResult>
        {
        }
        interface IModulusOperators$3<TSelf, TOther, TResult>
        {
        }
        interface INumberBase$1<TSelf> extends System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>
        {
        }
        interface IAdditionOperators$3<TSelf, TOther, TResult>
        {
        }
        interface IAdditiveIdentity$2<TSelf, TResult>
        {
        }
        interface IDecrementOperators$1<TSelf>
        {
        }
        interface IDivisionOperators$3<TSelf, TOther, TResult>
        {
        }
        interface IIncrementOperators$1<TSelf>
        {
        }
        interface IMultiplicativeIdentity$2<TSelf, TResult>
        {
        }
        interface IMultiplyOperators$3<TSelf, TOther, TResult>
        {
        }
        interface ISubtractionOperators$3<TSelf, TOther, TResult>
        {
        }
        interface IUnaryPlusOperators$2<TSelf, TResult>
        {
        }
        interface IUnaryNegationOperators$2<TSelf, TResult>
        {
        }
        interface IShiftOperators$3<TSelf, TOther, TResult>
        {
        }
        interface IMinMaxValue$1<TSelf>
        {
        }
        interface ISignedNumber$1<TSelf> extends System.Numerics.INumberBase$1<TSelf>, System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>
        {
        }
        interface IUnsignedNumber$1<TSelf> extends System.Numerics.INumberBase$1<TSelf>, System.Numerics.IAdditionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IAdditiveIdentity$2<TSelf, TSelf>, System.Numerics.IDecrementOperators$1<TSelf>, System.Numerics.IDivisionOperators$3<TSelf, TSelf, TSelf>, System.IEquatable$1<TSelf>, System.Numerics.IEqualityOperators$3<TSelf, TSelf, boolean>, System.Numerics.IIncrementOperators$1<TSelf>, System.Numerics.IMultiplicativeIdentity$2<TSelf, TSelf>, System.Numerics.IMultiplyOperators$3<TSelf, TSelf, TSelf>, System.ISpanFormattable, System.IFormattable, System.ISpanParsable$1<TSelf>, System.IParsable$1<TSelf>, System.Numerics.ISubtractionOperators$3<TSelf, TSelf, TSelf>, System.Numerics.IUnaryPlusOperators$2<TSelf, TSelf>, System.Numerics.IUnaryNegationOperators$2<TSelf, TSelf>, System.IUtf8SpanFormattable, System.IUtf8SpanParsable$1<TSelf>
        {
        }
    }
    namespace System.Runtime.Serialization {
        interface ISerializable
        {
        }
        interface IDeserializationCallback
        {
        }
    }
    namespace System.Runtime.CompilerServices {
        class TaskAwaiter extends System.ValueType implements System.Runtime.CompilerServices.ICriticalNotifyCompletion, System.Runtime.CompilerServices.INotifyCompletion, System.Runtime.CompilerServices.ITaskAwaiter
        {
            protected [__keep_incompatibility]: never;
        }
        interface ICriticalNotifyCompletion extends System.Runtime.CompilerServices.INotifyCompletion
        {
        }
        interface INotifyCompletion
        {
        }
        interface ITaskAwaiter
        {
        }
        class ConfiguredTaskAwaitable extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        class YieldAwaitable extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
    }
    namespace System.Threading {
        class CancellationToken extends System.ValueType implements System.IEquatable$1<System.Threading.CancellationToken>
        {
            protected [__keep_incompatibility]: never;
        }
    }
    namespace System.Collections {
        interface IList extends System.Collections.ICollection, System.Collections.IEnumerable
        {
        }
        interface ICollection extends System.Collections.IEnumerable
        {
        }
        interface IEnumerable
        {
        }
        interface IStructuralComparable
        {
        }
        interface IStructuralEquatable
        {
        }
    }
    namespace System.Collections.Generic {
        interface IEnumerable$1<T> extends System.Collections.IEnumerable
        {
        }
        interface IList$1<T> extends System.Collections.Generic.ICollection$1<T>, System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable
        {
        }
        interface ICollection$1<T> extends System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable
        {
        }
        interface IReadOnlyList$1<T> extends System.Collections.Generic.IReadOnlyCollection$1<T>, System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable
        {
        }
        interface IReadOnlyCollection$1<T> extends System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable
        {
        }
        class List$1<T> extends System.Object implements System.Collections.Generic.IList$1<T>, System.Collections.Generic.ICollection$1<T>, System.Collections.Generic.IEnumerable$1<T>, System.Collections.IEnumerable, System.Collections.IList, System.Collections.ICollection, System.Collections.Generic.IReadOnlyList$1<T>, System.Collections.Generic.IReadOnlyCollection$1<T>
        {
            protected [__keep_incompatibility]: never;
            public [Symbol.iterator]() : IterableIterator<T>
        }
    }
    namespace 创世记 {
        class Log extends System.Object
        {
            protected [__keep_incompatibility]: never;
            public static LogList : System.Collections.Generic.List$1<创世记.Log.LogData>
            public static LogSeverity : 创世记.Log.Severity
            public static Debug (...m: string[]) : void
            public static Info (...m: string[]) : void
            public static Warn (...m: string[]) : void
            public static Error (...m: string[]) : void
            public static Error ($e: System.Exception) : void
            public static Assert ($condition: boolean, ...m: string[]) : void
        }
    }
    namespace 创世记.Log {
        class LogData extends System.ValueType implements System.IEquatable$1<创世记.Log.LogData>
        {
            protected [__keep_incompatibility]: never;
        }
        enum Severity
        { Debug = -1, Info = 0, Warn = 1, Error = 2 }
    }
    namespace System.Reflection {
        class MemberInfo extends System.Object implements System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        interface ICustomAttributeProvider
        {
        }
        interface IReflect
        {
        }
        interface MemberFilter
        { 
        (m: System.Reflection.MemberInfo, filterCriteria: any) : boolean; 
        Invoke?: (m: System.Reflection.MemberInfo, filterCriteria: any) => boolean;
        }
        var MemberFilter: { new (func: (m: System.Reflection.MemberInfo, filterCriteria: any) => boolean): MemberFilter; }
        class AssemblyName extends System.Object implements System.ICloneable, System.Runtime.Serialization.IDeserializationCallback, System.Runtime.Serialization.ISerializable
        {
            protected [__keep_incompatibility]: never;
        }
        class Assembly extends System.Object implements System.Reflection.ICustomAttributeProvider, System.Runtime.Serialization.ISerializable
        {
            protected [__keep_incompatibility]: never;
        }
        enum MemberTypes
        { Constructor = 1, Event = 2, Field = 4, Method = 8, Property = 16, TypeInfo = 32, Custom = 64, NestedType = 128, All = 191 }
        class Module extends System.Object implements System.Reflection.ICustomAttributeProvider, System.Runtime.Serialization.ISerializable
        {
            protected [__keep_incompatibility]: never;
        }
        class MethodBase extends System.Reflection.MemberInfo implements System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        enum GenericParameterAttributes
        { None = 0, VarianceMask = 3, Covariant = 1, Contravariant = 2, SpecialConstraintMask = 28, ReferenceTypeConstraint = 4, NotNullableValueTypeConstraint = 8, DefaultConstructorConstraint = 16 }
        enum TypeAttributes
        { VisibilityMask = 7, NotPublic = 0, Public = 1, NestedPublic = 2, NestedPrivate = 3, NestedFamily = 4, NestedAssembly = 5, NestedFamANDAssem = 6, NestedFamORAssem = 7, LayoutMask = 24, AutoLayout = 0, SequentialLayout = 8, ExplicitLayout = 16, ClassSemanticsMask = 32, Class = 0, Interface = 32, Abstract = 128, Sealed = 256, SpecialName = 1024, Import = 4096, Serializable = 8192, WindowsRuntime = 16384, StringFormatMask = 196608, AnsiClass = 0, UnicodeClass = 65536, AutoClass = 131072, CustomFormatClass = 196608, CustomFormatMask = 12582912, BeforeFieldInit = 1048576, RTSpecialName = 2048, HasSecurity = 262144, ReservedMask = 264192 }
        class ConstructorInfo extends System.Reflection.MethodBase implements System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        enum BindingFlags
        { Default = 0, IgnoreCase = 1, DeclaredOnly = 2, Instance = 4, Static = 8, Public = 16, NonPublic = 32, FlattenHierarchy = 64, InvokeMethod = 256, CreateInstance = 512, GetField = 1024, SetField = 2048, GetProperty = 4096, SetProperty = 8192, PutDispProperty = 16384, PutRefDispProperty = 32768, ExactBinding = 65536, SuppressChangeType = 131072, OptionalParamBinding = 262144, IgnoreReturn = 16777216, DoNotWrapExceptions = 33554432 }
        class Binder extends System.Object
        {
            protected [__keep_incompatibility]: never;
        }
        class ParameterModifier extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        enum CallingConventions
        { Standard = 1, VarArgs = 2, Any = 3, HasThis = 32, ExplicitThis = 64 }
        class EventInfo extends System.Reflection.MemberInfo implements System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        class FieldInfo extends System.Reflection.MemberInfo implements System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        class MethodInfo extends System.Reflection.MethodBase implements System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        class PropertyInfo extends System.Reflection.MemberInfo implements System.Reflection.ICustomAttributeProvider
        {
            protected [__keep_incompatibility]: never;
        }
        class InterfaceMapping extends System.ValueType
        {
            protected [__keep_incompatibility]: never;
        }
        interface TypeFilter
        { 
        (m: System.Type, filterCriteria: any) : boolean; 
        Invoke?: (m: System.Type, filterCriteria: any) => boolean;
        }
        var TypeFilter: { new (func: (m: System.Type, filterCriteria: any) => boolean): TypeFilter; }
    }
    namespace System.Runtime.InteropServices {
        class StructLayoutAttribute extends System.Attribute
        {
            protected [__keep_incompatibility]: never;
        }
    }
    namespace System.Globalization {
        class CultureInfo extends System.Object implements System.IFormatProvider, System.ICloneable
        {
            protected [__keep_incompatibility]: never;
        }
    }
}
