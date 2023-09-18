using System.Runtime.InteropServices;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

internal class OnAreaDestroyed : IEvent
{
    NwObject IEvent.Context => null!;
    
    internal CNWSArea? Area { get; private init; }
    
    internal sealed unsafe class Factory : HookEventFactory
    {
        [NativeFunction("_ZN8CNWSAreaD1Ev", "")]
        private delegate void OnDestructor(void* pObject);

        private static FunctionHook<OnDestructor> AreaDestructorHook { get; set; } = null!;

        protected override IDisposable[] RequestHooks()
        {
            delegate* unmanaged<void*, void> pHook = &AreaDestructor;
            AreaDestructorHook = HookService.RequestHook<OnDestructor>(pHook, HookOrder.Earliest);
            
            return new IDisposable[] { AreaDestructorHook };
        }
        
        [UnmanagedCallersOnly]
        private static void AreaDestructor(void* pObject)
        {
            var obj = CNWSArea.FromPointer(pObject);

            ProcessEvent(EventCallbackType.Before,new OnAreaDestroyed()
            {
                Area = obj,
            });

            AreaDestructorHook.CallOriginal(pObject);
        }
    }
}