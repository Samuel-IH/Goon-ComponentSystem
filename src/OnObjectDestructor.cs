using System.Runtime.InteropServices;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

internal class OnObjectDestroyed : IEvent
{
    NwObject IEvent.Context => null!;
    
    internal CNWSObject? Obj { get; private init; }
    
    internal sealed unsafe class Factory : HookEventFactory
    {
        [NativeFunction("_ZN10CNWSObjectD2Ev", "")]
        private delegate void OnDestructor(void* pObject);

        private static FunctionHook<OnDestructor> ObjectDestructorHook { get; set; } = null!;

        protected override IDisposable[] RequestHooks()
        {
            delegate* unmanaged<void*, void> pHook = &ObjectDestructor;
            ObjectDestructorHook = HookService.RequestHook<OnDestructor>(pHook, HookOrder.Earliest);
            
            return new IDisposable[] { ObjectDestructorHook };
        }
        
        [UnmanagedCallersOnly]
        private static void ObjectDestructor(void* pObject)
        {
            var obj = CNWSObject.FromPointer(pObject);

            ProcessEvent(EventCallbackType.Before, new OnObjectDestroyed()
            {
                Obj = obj,
            });

            ObjectDestructorHook.CallOriginal(pObject);
        }
    }
}
