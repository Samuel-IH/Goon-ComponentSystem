using System.Reflection;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

[ServiceBinding(typeof(ComponentSystem))]
public unsafe class ComponentSystem
{
    internal static ComponentSystem Instance { get; private set; } = null!;

    private InjectionService InjectionService { get; init; }

    internal readonly GenericComponentFactory<NwPlayer> playerFactory;
    internal readonly GenericComponentFactory<NwPlaceable> placeableFactory;
    
    public ComponentSystem(InjectionService injectionService, EventService eventService)
    {
        #region Services

        InjectionService = injectionService;

        #endregion

        #region Events
        
        eventService.SubscribeAll<OnObjectDestroyed, OnObjectDestroyed.Factory>(OnObjectDestructor);
        NwModule.Instance.OnClientDisconnect += OnRemovePCFromWorld;
        
        #endregion

        #region Factories
        
        playerFactory = new GenericComponentFactory<NwPlayer>(injectionService, player => player.PlayerId);
        placeableFactory = new GenericComponentFactory<NwPlaceable>(injectionService, placeable => placeable.ObjectId);
        
        #endregion

        Instance = this;
    }

    private void OnRemovePCFromWorld(OnClientDisconnect evtData)
    {
        if (evtData.Player == null) return;
        playerFactory.Cleanup(evtData.Player.PlayerId);
    }

    private void OnObjectDestructor(OnObjectDestroyed evtData)
    {
        if (evtData.Object is not CNWSObject gameObject) return;
        
        switch (gameObject.m_nObjectType)
        {
            case (byte)ObjectType.Placeable:
                placeableFactory.Cleanup(gameObject.m_idSelf);
                break;
        }
    }
}