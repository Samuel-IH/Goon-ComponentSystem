using System.Runtime.InteropServices;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

internal class OnObjectDestroyed : IEvent
{
    NwObject IEvent.Context => null!;

    /// <summary>
    /// Nullable only because I don't know the inner workings of the game engine.
    /// This is a safer assumption.
    /// </summary>
    internal CNWSObject? Object { get; private init; }
    
    internal sealed unsafe class Factory : HookEventFactory
    {
        private delegate void OnObjectDestructor(void* pObject);

        private static FunctionHook<OnObjectDestructor> ObjectDestructorHook { get; set; } = null!;

        protected override IDisposable[] RequestHooks()
        {
            delegate* unmanaged<void*, void> pHook = &ObjectDestructor;
            ObjectDestructorHook = HookService.RequestHook<OnObjectDestructor>(pHook, FunctionsLinux._ZN10CNWSObjectD1Ev, HookOrder.SharedHook);
            
            return new IDisposable[] { ObjectDestructorHook };
        }
        
        [UnmanagedCallersOnly]
        private static void ObjectDestructor(void* pObject)
        {
            var obj = CNWSObject.FromPointer(pObject);

            ProcessEvent(new OnObjectDestroyed()
            {
                Object = obj,
            });

            ObjectDestructorHook.CallOriginal(pObject);
        }
    }
}