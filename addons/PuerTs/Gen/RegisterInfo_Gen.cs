using Puerts.TypeMapping;
using Puerts;

namespace PuertsStaticWrap
{
#if ENABLE_IL2CPP
    [UnityEngine.Scripting.Preserve]
#endif
    public static class PuerRegisterInfo_Gen
    {
        
        public static RegisterInfo GetRegisterInfo_System_Threading_Tasks_Task_Wrap() 
        {
            return new RegisterInfo 
            {
#if !EXPERIMENTAL_IL2CPP_PUERTS
                BlittableCopy = false,
#endif

                Members = new System.Collections.Generic.Dictionary<string, MemberRegisterInfo>
                {
                    
                    {".ctor", new MemberRegisterInfo { Name = ".ctor", IsStatic = false, MemberType = MemberType.Constructor, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Constructor = System_Threading_Tasks_Task_Wrap.Constructor
#endif
                    }},
                    {"Start", new MemberRegisterInfo { Name = "Start", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_Start
#endif
                    }},
                    {"RunSynchronously", new MemberRegisterInfo { Name = "RunSynchronously", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_RunSynchronously
#endif
                    }},
                    {"Dispose", new MemberRegisterInfo { Name = "Dispose", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_Dispose
#endif
                    }},
                    {"GetAwaiter", new MemberRegisterInfo { Name = "GetAwaiter", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_GetAwaiter
#endif
                    }},
                    {"ConfigureAwait", new MemberRegisterInfo { Name = "ConfigureAwait", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_ConfigureAwait
#endif
                    }},
                    {"Wait", new MemberRegisterInfo { Name = "Wait", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_Wait
#endif
                    }},
                    {"WaitAsync", new MemberRegisterInfo { Name = "WaitAsync", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_WaitAsync
#endif
                    }},
                    {"ContinueWith", new MemberRegisterInfo { Name = "ContinueWith", IsStatic = false, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.M_ContinueWith
#endif
                    }},
                    {"Id", new MemberRegisterInfo { Name = "Id", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_Id
#endif
                    }},
                    {"Exception", new MemberRegisterInfo { Name = "Exception", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_Exception
#endif
                    }},
                    {"Status", new MemberRegisterInfo { Name = "Status", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_Status
#endif
                    }},
                    {"IsCanceled", new MemberRegisterInfo { Name = "IsCanceled", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_IsCanceled
#endif
                    }},
                    {"IsCompleted", new MemberRegisterInfo { Name = "IsCompleted", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_IsCompleted
#endif
                    }},
                    {"IsCompletedSuccessfully", new MemberRegisterInfo { Name = "IsCompletedSuccessfully", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_IsCompletedSuccessfully
#endif
                    }},
                    {"CreationOptions", new MemberRegisterInfo { Name = "CreationOptions", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_CreationOptions
#endif
                    }},
                    {"AsyncState", new MemberRegisterInfo { Name = "AsyncState", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_AsyncState
#endif
                    }},
                    {"IsFaulted", new MemberRegisterInfo { Name = "IsFaulted", IsStatic = false, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_IsFaulted
#endif
                    }},
                    {"Yield_static", new MemberRegisterInfo { Name = "Yield", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_Yield
#endif
                    }},
                    {"WaitAll_static", new MemberRegisterInfo { Name = "WaitAll", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_WaitAll
#endif
                    }},
                    {"WaitAny_static", new MemberRegisterInfo { Name = "WaitAny", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_WaitAny
#endif
                    }},
                    {"FromException_static", new MemberRegisterInfo { Name = "FromException", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_FromException
#endif
                    }},
                    {"FromCanceled_static", new MemberRegisterInfo { Name = "FromCanceled", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_FromCanceled
#endif
                    }},
                    {"Run_static", new MemberRegisterInfo { Name = "Run", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_Run
#endif
                    }},
                    {"Delay_static", new MemberRegisterInfo { Name = "Delay", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_Delay
#endif
                    }},
                    {"WhenAll_static", new MemberRegisterInfo { Name = "WhenAll", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_WhenAll
#endif
                    }},
                    {"WhenAny_static", new MemberRegisterInfo { Name = "WhenAny", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = System_Threading_Tasks_Task_Wrap.F_WhenAny
#endif
                    }},
                    {"CurrentId_static", new MemberRegisterInfo { Name = "CurrentId", IsStatic = true, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_CurrentId
#endif
                    }},
                    {"Factory_static", new MemberRegisterInfo { Name = "Factory", IsStatic = true, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_Factory
#endif
                    }},
                    {"CompletedTask_static", new MemberRegisterInfo { Name = "CompletedTask", IsStatic = true, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = System_Threading_Tasks_Task_Wrap.G_CompletedTask
#endif
                    }},
                }
            };
        }
        public static RegisterInfo GetRegisterInfo_创世记_Log_Wrap() 
        {
            return new RegisterInfo 
            {
#if !EXPERIMENTAL_IL2CPP_PUERTS
                BlittableCopy = false,
#endif

                Members = new System.Collections.Generic.Dictionary<string, MemberRegisterInfo>
                {
                    
                    {"Debug_static", new MemberRegisterInfo { Name = "Debug", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = 创世记_Log_Wrap.F_Debug
#endif
                    }},
                    {"Info_static", new MemberRegisterInfo { Name = "Info", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = 创世记_Log_Wrap.F_Info
#endif
                    }},
                    {"Warn_static", new MemberRegisterInfo { Name = "Warn", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = 创世记_Log_Wrap.F_Warn
#endif
                    }},
                    {"Error_static", new MemberRegisterInfo { Name = "Error", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = 创世记_Log_Wrap.F_Error
#endif
                    }},
                    {"Assert_static", new MemberRegisterInfo { Name = "Assert", IsStatic = true, MemberType = MemberType.Method, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , Method = 创世记_Log_Wrap.F_Assert
#endif
                    }},
                    {"LogList_static", new MemberRegisterInfo { Name = "LogList", IsStatic = true, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = 创世记_Log_Wrap.G_LogList
#endif
                    }},
                    {"LogSeverity_static", new MemberRegisterInfo { Name = "LogSeverity", IsStatic = true, MemberType = MemberType.Property, UseBindingMode = BindingMode.FastBinding
#if !EXPERIMENTAL_IL2CPP_PUERTS
                    , PropertyGetter = 创世记_Log_Wrap.G_LogSeverity
#endif
                    }},
                }
            };
        }

        public static void AddRegisterInfoGetterIntoJsEnv(this JsEnv jsEnv)
        {
            
                jsEnv.AddRegisterInfoGetter(typeof(System.Threading.Tasks.Task), GetRegisterInfo_System_Threading_Tasks_Task_Wrap);
                jsEnv.AddRegisterInfoGetter(typeof(创世记.Log), GetRegisterInfo_创世记_Log_Wrap);
        }
    }
}