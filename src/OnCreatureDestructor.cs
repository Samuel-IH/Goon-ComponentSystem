using System.Runtime.InteropServices;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

internal class OnCreatureDestroyed : IEvent
{
    NwObject IEvent.Context => null!;
    
    internal CNWSCreature? Creature { get; private init; }
    
    internal sealed unsafe class Factory : HookEventFactory
    {
        [NativeFunction("_ZN13CNWSCreatureD1Ev", "")]
        private delegate void OnDestructor(void* pObject);

        private static FunctionHook<OnDestructor> CreatureDestructorHook { get; set; } = null!;

        protected override IDisposable[] RequestHooks()
        {
            delegate* unmanaged<void*, void> pHook = &CreatureDestructor;
            CreatureDestructorHook = HookService.RequestHook<OnDestructor>(pHook, HookOrder.Earliest);
            
            return new IDisposable[] { CreatureDestructorHook };
        }
        
        [UnmanagedCallersOnly]
        private static void CreatureDestructor(void* pObject)
        {
            var obj = CNWSCreature.FromPointer(pObject);

            ProcessEvent(EventCallbackType.Before, new OnCreatureDestroyed()
            {
                Creature = obj,
            });

            CreatureDestructorHook.CallOriginal(pObject);
        }
    }
}