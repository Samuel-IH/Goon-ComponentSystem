using System.Runtime.InteropServices;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

internal class OnPlaceableDestroyed : IEvent
{
    NwObject IEvent.Context => null!;
    
    internal CNWSPlaceable? Placeable { get; private init; }
    
    internal sealed unsafe class Factory : HookEventFactory
    {
        [NativeFunction("_ZN13CNWSPlaceableD1Ev", "")]
        private delegate void OnDestructor(void* pObject);

        private static FunctionHook<OnDestructor> PlaceableDestructorHook { get; set; } = null!;

        protected override IDisposable[] RequestHooks()
        {
            delegate* unmanaged<void*, void> pHook = &PlaceableDestructor;
            PlaceableDestructorHook = HookService.RequestHook<OnDestructor>(pHook, HookOrder.Earliest);
            
            return new IDisposable[] { PlaceableDestructorHook };
        }
        
        [UnmanagedCallersOnly]
        private static void PlaceableDestructor(void* pObject)
        {
            var obj = CNWSPlaceable.FromPointer(pObject);

            ProcessEvent(EventCallbackType.Before, new OnPlaceableDestroyed()
            {
                Placeable = obj,
            });

            PlaceableDestructorHook.CallOriginal(pObject);
        }
    }
}
