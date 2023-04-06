using System.Runtime.InteropServices;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

public class OnRemovePcFromWorldEventData: IEvent
{
    NwObject IEvent.Context => null!;

    /// <summary>
    /// Nullable only because I don't know the inner workings of the game engine.
    /// This is a safer assumption.
    /// </summary>
    internal CNWSPlayer? Player { get; private init; }

    internal sealed unsafe class Factory : HookEventFactory
    {
        private delegate void HookDelegate(void* pObject);

        private static FunctionHook<HookDelegate> Hook { get; set; } = null!;

        protected override IDisposable[] RequestHooks()
        {
            delegate* unmanaged<void*, void> pHook = &Callback;
            Hook = HookService.RequestHook<HookDelegate>(pHook,
                FunctionsLinux._ZN21CServerExoAppInternal17RemovePCFromWorldEP10CNWSPlayer, HookOrder.SharedHook);

            return new IDisposable[] { Hook };
        }

        [UnmanagedCallersOnly]
        private static void Callback(void* pPlayer)
        {
            var obj = CNWSObject.FromPointer(pPlayer);

            ProcessEvent(new OnRemovePcFromWorldEventData()
            {
                Player = CNWSPlayer.FromPointer(pPlayer),
            });

            Hook.CallOriginal(pPlayer);
        }
    }
}