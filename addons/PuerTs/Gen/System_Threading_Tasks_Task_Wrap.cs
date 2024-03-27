#nullable disable
        #if !(EXPERIMENTAL_IL2CPP_PUERTS && ENABLE_IL2CPP)
using System;
using Puerts;

namespace PuertsStaticWrap
{
#pragma warning disable 0219
    public static class System_Threading_Tasks_Task_Wrap 
    {
    
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8ConstructorCallback))]
        public static IntPtr Constructor(IntPtr isolate, IntPtr info, int paramLen, long data)
        {
            try
            {

                if (paramLen == 1)
                {
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action), false, false, v8Value0, ref argobj0, ref argType0))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action arg0 = (System.Action)argobj0;
                        var result = new System.Threading.Tasks.Task(arg0);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                }
                if (paramLen == 2)
                {
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action arg0 = (System.Action)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;
                        var result = new System.Threading.Tasks.Task(arg0, arg1);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskCreationOptions), false, false, v8Value1, ref argobj1, ref argType1))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action arg0 = (System.Action)argobj0;
                        System.Threading.Tasks.TaskCreationOptions arg1 = (System.Threading.Tasks.TaskCreationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value1, false);
                        var result = new System.Threading.Tasks.Task(arg0, arg1);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Object> arg0 = (System.Action<System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        var result = new System.Threading.Tasks.Task(arg0, arg1);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                }
                if (paramLen == 3)
                {
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskCreationOptions), false, false, v8Value2, ref argobj2, ref argType2))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action arg0 = (System.Action)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;
                        System.Threading.Tasks.TaskCreationOptions arg2 = (System.Threading.Tasks.TaskCreationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value2, false);
                        var result = new System.Threading.Tasks.Task(arg0, arg1, arg2);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Object> arg0 = (System.Action<System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;
                        var result = new System.Threading.Tasks.Task(arg0, arg1, arg2);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskCreationOptions), false, false, v8Value2, ref argobj2, ref argType2))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Object> arg0 = (System.Action<System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        System.Threading.Tasks.TaskCreationOptions arg2 = (System.Threading.Tasks.TaskCreationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value2, false);
                        var result = new System.Threading.Tasks.Task(arg0, arg1, arg2);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                }
                if (paramLen == 4)
                {
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    IntPtr v8Value3 = PuertsDLL.GetArgumentValue(isolate, info, 3);
                    object argobj3 = null;
                    JsValueType argType3 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskCreationOptions), false, false, v8Value3, ref argobj3, ref argType3))

                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Object> arg0 = (System.Action<System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;
                        System.Threading.Tasks.TaskCreationOptions arg3 = (System.Threading.Tasks.TaskCreationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value3, false);
                        var result = new System.Threading.Tasks.Task(arg0, arg1, arg2, arg3);


                        return Puerts.Utils.GetObjectPtr((int)data, typeof(System.Threading.Tasks.Task), result);
                    }
                }

                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to " + typeof(System.Threading.Tasks.Task).GetFriendlyName() + " constructor");
            } catch (Exception e) {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
            return IntPtr.Zero;
        }
    // ==================== constructor end ====================

    // ==================== methods start ====================
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_Start(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                if (paramLen == 0)
                {
            
                    {

                        obj.Start ();

                        return;
                    }
                }
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.TaskScheduler), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.TaskScheduler>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.TaskScheduler arg0 = (System.Threading.Tasks.TaskScheduler)argobj0;

                        obj.Start (arg0);

                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to Start");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_RunSynchronously(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                if (paramLen == 0)
                {
            
                    {

                        obj.RunSynchronously ();

                        return;
                    }
                }
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.TaskScheduler), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.TaskScheduler>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.TaskScheduler arg0 = (System.Threading.Tasks.TaskScheduler)argobj0;

                        obj.RunSynchronously (arg0);

                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to RunSynchronously");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_Dispose(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                {
            
                    {

                        obj.Dispose ();

                    }
                }
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_GetAwaiter(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                {
            
                    {

                        var result = obj.GetAwaiter ();

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                    }
                }
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_ConfigureAwait(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Boolean, typeof(bool), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        bool arg0 = (bool)PuertsDLL.GetBooleanFromValue(isolate, v8Value0, false);

                        var result = obj.ConfigureAwait (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.ConfigureAwaitOptions), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        System.Threading.Tasks.ConfigureAwaitOptions arg0 = (System.Threading.Tasks.ConfigureAwaitOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value0, false);

                        var result = obj.ConfigureAwait (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to ConfigureAwait");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_Yield(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                {
            
                    {

                        var result = System.Threading.Tasks.Task.Yield ();

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                    }
                }
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_Wait(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                if (paramLen == 0)
                {
            
                    {

                        obj.Wait ();

                        return;
                    }
                }
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;

                        var result = obj.Wait (arg0);

                        Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.CancellationToken arg0 = (System.Threading.CancellationToken)argobj0;

                        obj.Wait (arg0);

                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        int arg0 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value0, false);

                        var result = obj.Wait (arg0);

                        Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = obj.Wait (arg0, arg1);

                        Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        int arg0 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value0, false);
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = obj.Wait (arg0, arg1);

                        Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to Wait");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_WaitAsync(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.CancellationToken arg0 = (System.Threading.CancellationToken)argobj0;

                        var result = obj.WaitAsync (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;

                        var result = obj.WaitAsync (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.TimeProvider), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.TimeProvider>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.TimeProvider arg1 = (System.TimeProvider)argobj1;

                        var result = obj.WaitAsync (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = obj.WaitAsync (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 3)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.TimeProvider), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.TimeProvider>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.TimeProvider arg1 = (System.TimeProvider)argobj1;
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;

                        var result = obj.WaitAsync (arg0, arg1, arg2);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to WaitAsync");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void M_ContinueWith(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
        
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task> arg0 = (System.Action<System.Threading.Tasks.Task>)argobj0;

                        var result = obj.ContinueWith (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task> arg0 = (System.Action<System.Threading.Tasks.Task>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = obj.ContinueWith (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.TaskScheduler), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task> arg0 = (System.Action<System.Threading.Tasks.Task>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.Tasks.TaskScheduler>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.Tasks.TaskScheduler arg1 = (System.Threading.Tasks.TaskScheduler)argobj1;

                        var result = obj.ContinueWith (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskContinuationOptions), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task> arg0 = (System.Action<System.Threading.Tasks.Task>)argobj0;
                        System.Threading.Tasks.TaskContinuationOptions arg1 = (System.Threading.Tasks.TaskContinuationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value1, false);

                        var result = obj.ContinueWith (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task, System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task, System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task, System.Object> arg0 = (System.Action<System.Threading.Tasks.Task, System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;

                        var result = obj.ContinueWith (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 4)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    IntPtr v8Value3 = PuertsDLL.GetArgumentValue(isolate, info, 3);
                    object argobj3 = null;
                    JsValueType argType3 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskContinuationOptions), false, false, v8Value2, ref argobj2, ref argType2) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.TaskScheduler), false, false, v8Value3, ref argobj3, ref argType3))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task> arg0 = (System.Action<System.Threading.Tasks.Task>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;
                        System.Threading.Tasks.TaskContinuationOptions arg2 = (System.Threading.Tasks.TaskContinuationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value2, false);
                        argobj3 = argobj3 != null ? argobj3 : StaticTranslate<System.Threading.Tasks.TaskScheduler>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value3, false); System.Threading.Tasks.TaskScheduler arg3 = (System.Threading.Tasks.TaskScheduler)argobj3;

                        var result = obj.ContinueWith (arg0, arg1, arg2, arg3);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 3)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task, System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task, System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task, System.Object> arg0 = (System.Action<System.Threading.Tasks.Task, System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;

                        var result = obj.ContinueWith (arg0, arg1, arg2);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task, System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.TaskScheduler), false, false, v8Value2, ref argobj2, ref argType2))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task, System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task, System.Object> arg0 = (System.Action<System.Threading.Tasks.Task, System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.Tasks.TaskScheduler>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.Tasks.TaskScheduler arg2 = (System.Threading.Tasks.TaskScheduler)argobj2;

                        var result = obj.ContinueWith (arg0, arg1, arg2);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task, System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskContinuationOptions), false, false, v8Value2, ref argobj2, ref argType2))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task, System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task, System.Object> arg0 = (System.Action<System.Threading.Tasks.Task, System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        System.Threading.Tasks.TaskContinuationOptions arg2 = (System.Threading.Tasks.TaskContinuationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value2, false);

                        var result = obj.ContinueWith (arg0, arg1, arg2);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 5)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    IntPtr v8Value3 = PuertsDLL.GetArgumentValue(isolate, info, 3);
                    object argobj3 = null;
                    JsValueType argType3 = JsValueType.Invalid;
                    IntPtr v8Value4 = PuertsDLL.GetArgumentValue(isolate, info, 4);
                    object argobj4 = null;
                    JsValueType argType4 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action<System.Threading.Tasks.Task, System.Object>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Any, typeof(System.Object), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(System.Threading.Tasks.TaskContinuationOptions), false, false, v8Value3, ref argobj3, ref argType3) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.TaskScheduler), false, false, v8Value4, ref argobj4, ref argType4))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action<System.Threading.Tasks.Task, System.Object>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action<System.Threading.Tasks.Task, System.Object> arg0 = (System.Action<System.Threading.Tasks.Task, System.Object>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Object>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Object arg1 = (System.Object)argobj1;
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;
                        System.Threading.Tasks.TaskContinuationOptions arg3 = (System.Threading.Tasks.TaskContinuationOptions)StaticTranslate<int>.Get((int)data, isolate, Puerts.NativeValueApi.GetValueFromArgument, v8Value3, false);
                        argobj4 = argobj4 != null ? argobj4 : StaticTranslate<System.Threading.Tasks.TaskScheduler>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value4, false); System.Threading.Tasks.TaskScheduler arg4 = (System.Threading.Tasks.TaskScheduler)argobj4;

                        var result = obj.ContinueWith (arg0, arg1, arg2, arg3, arg4);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to ContinueWith");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_WaitAll(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                if (paramLen >= 0)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatchParams((int)data, isolate, info, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task), 0, paramLen, v8Value0, ref argobj0, ref argType0))
                    {
                        System.Threading.Tasks.Task[] arg0 = ArgHelper.GetParams<System.Threading.Tasks.Task>((int)data, isolate, info, 0, paramLen, v8Value0);

                        System.Threading.Tasks.Task.WaitAll (arg0);

                        return;
                    }
                }
                if (paramLen == 0)
                {
            
                    {

                        System.Threading.Tasks.Task.WaitAll (System.Array.Empty<System.Threading.Tasks.Task>());

                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.TimeSpan arg1 = (System.TimeSpan)argobj1;

                        var result = System.Threading.Tasks.Task.WaitAll (arg0, arg1);

                        Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        int arg1 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value1, false);

                        var result = System.Threading.Tasks.Task.WaitAll (arg0, arg1);

                        Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        System.Threading.Tasks.Task.WaitAll (arg0, arg1);

                        return;
                    }
                }
                if (paramLen == 3)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        int arg1 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value1, false);
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;

                        var result = System.Threading.Tasks.Task.WaitAll (arg0, arg1, arg2);

                        Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to WaitAll");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_WaitAny(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                if (paramLen >= 0)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatchParams((int)data, isolate, info, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task), 0, paramLen, v8Value0, ref argobj0, ref argType0))
                    {
                        System.Threading.Tasks.Task[] arg0 = ArgHelper.GetParams<System.Threading.Tasks.Task>((int)data, isolate, info, 0, paramLen, v8Value0);

                        var result = System.Threading.Tasks.Task.WaitAny (arg0);

                        Puerts.PuertsDLL.ReturnNumber(isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 0)
                {
            
                    {

                        var result = System.Threading.Tasks.Task.WaitAny (System.Array.Empty<System.Threading.Tasks.Task>());

                        Puerts.PuertsDLL.ReturnNumber(isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.TimeSpan arg1 = (System.TimeSpan)argobj1;

                        var result = System.Threading.Tasks.Task.WaitAny (arg0, arg1);

                        Puerts.PuertsDLL.ReturnNumber(isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = System.Threading.Tasks.Task.WaitAny (arg0, arg1);

                        Puerts.PuertsDLL.ReturnNumber(isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        int arg1 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value1, false);

                        var result = System.Threading.Tasks.Task.WaitAny (arg0, arg1);

                        Puerts.PuertsDLL.ReturnNumber(isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 3)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task[]), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task[]>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task[] arg0 = (System.Threading.Tasks.Task[])argobj0;
                        int arg1 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value1, false);
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;

                        var result = System.Threading.Tasks.Task.WaitAny (arg0, arg1, arg2);

                        Puerts.PuertsDLL.ReturnNumber(isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to WaitAny");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_FromException(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    ;
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Exception>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Exception arg0 = (System.Exception)argobj0;

                        var result = System.Threading.Tasks.Task.FromException (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                    }
                }
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_FromCanceled(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    ;
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.CancellationToken arg0 = (System.Threading.CancellationToken)argobj0;

                        var result = System.Threading.Tasks.Task.FromCanceled (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                    }
                }
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_Run(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action arg0 = (System.Action)argobj0;

                        var result = System.Threading.Tasks.Task.Run (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Func<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Func<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Func<System.Threading.Tasks.Task> arg0 = (System.Func<System.Threading.Tasks.Task>)argobj0;

                        var result = System.Threading.Tasks.Task.Run (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Action), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Action>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Action arg0 = (System.Action)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = System.Threading.Tasks.Task.Run (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject | Puerts.JsValueType.Function, typeof(System.Func<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Func<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Func<System.Threading.Tasks.Task> arg0 = (System.Func<System.Threading.Tasks.Task>)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = System.Threading.Tasks.Task.Run (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to Run");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_Delay(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;

                        var result = System.Threading.Tasks.Task.Delay (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        int arg0 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value0, false);

                        var result = System.Threading.Tasks.Task.Delay (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.TimeProvider), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.TimeProvider>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.TimeProvider arg1 = (System.TimeProvider)argobj1;

                        var result = System.Threading.Tasks.Task.Delay (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = System.Threading.Tasks.Task.Delay (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.Number, typeof(int), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        int arg0 = (int)PuertsDLL.GetNumberFromValue(isolate, v8Value0, false);
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.CancellationToken arg1 = (System.Threading.CancellationToken)argobj1;

                        var result = System.Threading.Tasks.Task.Delay (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 3)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    IntPtr v8Value2 = PuertsDLL.GetArgumentValue(isolate, info, 2);
                    object argobj2 = null;
                    JsValueType argType2 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.TimeSpan), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.TimeProvider), false, false, v8Value1, ref argobj1, ref argType1) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NativeObject, typeof(System.Threading.CancellationToken), false, false, v8Value2, ref argobj2, ref argType2))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.TimeSpan>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.TimeSpan arg0 = (System.TimeSpan)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.TimeProvider>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.TimeProvider arg1 = (System.TimeProvider)argobj1;
                        argobj2 = argobj2 != null ? argobj2 : StaticTranslate<System.Threading.CancellationToken>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value2, false); System.Threading.CancellationToken arg2 = (System.Threading.CancellationToken)argobj2;

                        var result = System.Threading.Tasks.Task.Delay (arg0, arg1, arg2);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to Delay");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_WhenAll(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task> arg0 = (System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>)argobj0;

                        var result = System.Threading.Tasks.Task.WhenAll (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen >= 0)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatchParams((int)data, isolate, info, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task), 0, paramLen, v8Value0, ref argobj0, ref argType0))
                    {
                        System.Threading.Tasks.Task[] arg0 = ArgHelper.GetParams<System.Threading.Tasks.Task>((int)data, isolate, info, 0, paramLen, v8Value0);

                        var result = System.Threading.Tasks.Task.WhenAll (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 0)
                {
            
                    {

                        var result = System.Threading.Tasks.Task.WhenAll (System.Array.Empty<System.Threading.Tasks.Task>());

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to WhenAll");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void F_WhenAny(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
        
                if (paramLen >= 0)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatchParams((int)data, isolate, info, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task), 0, paramLen, v8Value0, ref argobj0, ref argType0))
                    {
                        System.Threading.Tasks.Task[] arg0 = ArgHelper.GetParams<System.Threading.Tasks.Task>((int)data, isolate, info, 0, paramLen, v8Value0);

                        var result = System.Threading.Tasks.Task.WhenAny (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 0)
                {
            
                    {

                        var result = System.Threading.Tasks.Task.WhenAny (System.Array.Empty<System.Threading.Tasks.Task>());

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 2)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    IntPtr v8Value1 = PuertsDLL.GetArgumentValue(isolate, info, 1);
                    object argobj1 = null;
                    JsValueType argType1 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task), false, false, v8Value0, ref argobj0, ref argType0) && ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Threading.Tasks.Task), false, false, v8Value1, ref argobj1, ref argType1))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Threading.Tasks.Task>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Threading.Tasks.Task arg0 = (System.Threading.Tasks.Task)argobj0;
                        argobj1 = argobj1 != null ? argobj1 : StaticTranslate<System.Threading.Tasks.Task>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value1, false); System.Threading.Tasks.Task arg1 = (System.Threading.Tasks.Task)argobj1;

                        var result = System.Threading.Tasks.Task.WhenAny (arg0, arg1);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                if (paramLen == 1)
                {
            
                    IntPtr v8Value0 = PuertsDLL.GetArgumentValue(isolate, info, 0);
                    object argobj0 = null;
                    JsValueType argType0 = JsValueType.Invalid;
                    if (ArgHelper.IsMatch((int)data, isolate, Puerts.JsValueType.NullOrUndefined | Puerts.JsValueType.NativeObject, typeof(System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>), false, false, v8Value0, ref argobj0, ref argType0))
                    {
                        argobj0 = argobj0 != null ? argobj0 : StaticTranslate<System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>>.Get((int)data, isolate, NativeValueApi.GetValueFromArgument, v8Value0, false); System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task> arg0 = (System.Collections.Generic.IEnumerable<System.Threading.Tasks.Task>)argobj0;

                        var result = System.Threading.Tasks.Task.WhenAny (arg0);

                        Puerts.ResultHelper.Set((int)data, isolate, info, result);
                        return;
                    }
                }
                Puerts.PuertsDLL.ThrowException(isolate, "invalid arguments to WhenAny");
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
    // ==================== methods end ====================

    // ==================== properties start ====================
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_Id(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.Id;
                Puerts.PuertsDLL.ReturnNumber(isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_CurrentId(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var result = System.Threading.Tasks.Task.CurrentId;
                Puerts.ResultHelper.Set((int)data, isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_Exception(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.Exception;
                Puerts.ResultHelper.Set((int)data, isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_Status(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.Status;
                Puerts.PuertsDLL.ReturnNumber(isolate, info, (int)result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_IsCanceled(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.IsCanceled;
                Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_IsCompleted(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.IsCompleted;
                Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_IsCompletedSuccessfully(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.IsCompletedSuccessfully;
                Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_CreationOptions(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.CreationOptions;
                Puerts.PuertsDLL.ReturnNumber(isolate, info, (int)result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_AsyncState(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.AsyncState;
                Puerts.ResultHelper.Set((int)data, isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_Factory(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var result = System.Threading.Tasks.Task.Factory;
                Puerts.ResultHelper.Set((int)data, isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_CompletedTask(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var result = System.Threading.Tasks.Task.CompletedTask;
                Puerts.ResultHelper.Set((int)data, isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
        [Puerts.MonoPInvokeCallback(typeof(Puerts.V8FunctionCallback))]
        public static void G_IsFaulted(IntPtr isolate, IntPtr info, IntPtr self, int paramLen, long data)
        {
            try
            {
                var obj = Puerts.Utils.GetSelf((int)data, self) as System.Threading.Tasks.Task;
                var result = obj.IsFaulted;
                Puerts.PuertsDLL.ReturnBoolean(isolate, info, result);
            }
            catch (Exception e)
            {
                Puerts.PuertsDLL.ThrowException(isolate, "c# exception:" + e.Message + ",stack:" + e.StackTrace);
            }
        }
    // ==================== properties end ====================
    // ==================== array item get/set start ====================
    
    
    // ==================== array item get/set end ====================
    // ==================== operator start ====================
    // ==================== operator end ====================
    // ==================== events start ====================
    // ==================== events end ====================

    
    }
#pragma warning disable 0219
}
#endif
