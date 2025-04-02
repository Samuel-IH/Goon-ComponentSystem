using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace Goon.ComponentSystem;

[ServiceBinding(typeof(ComponentSystem))]
public class ComponentSystem
{
    internal static ComponentSystem Instance => ComponentSystemPluginFinder.ComponentSystem;

    private InjectionService InjectionService { get; init; }

    internal readonly GenericComponentFactory<NwArea> areaFactory;
    internal readonly GenericComponentFactory<NwPlayer> playerFactory;
    internal readonly GenericComponentFactory<NwPlaceable> placeableFactory;
    internal readonly GenericComponentFactory<NwCreature> creatureFactory;
    
    public ComponentSystem(InjectionService injectionService, EventService eventService)
    {
        #region Services

        InjectionService = injectionService;

        #endregion

        #region Events
        
        NwModule.Instance.OnClientDisconnect += OnRemovePCFromWorld;
        eventService.SubscribeAll<OnAreaDestroyed, OnAreaDestroyed.Factory>(OnAreaDestroyed);
        eventService.SubscribeAll<OnPlaceableDestroyed, OnPlaceableDestroyed.Factory>(OnPlaceableDestroyed);
        eventService.SubscribeAll<OnCreatureDestroyed, OnCreatureDestroyed.Factory>(OnCreatureDestroyed);

        #endregion

        #region Factories
        
        areaFactory = new GenericComponentFactory<NwArea>(injectionService, area => area.ObjectId);
        playerFactory = new GenericComponentFactory<NwPlayer>(injectionService, player => player.PlayerId);
        placeableFactory = new GenericComponentFactory<NwPlaceable>(injectionService, placeable => placeable.ObjectId);
        creatureFactory = new GenericComponentFactory<NwCreature>(injectionService, creature => creature.ObjectId);
        
        #endregion

        ComponentSystemPluginFinder.Register(this);
    }

    private void OnRemovePCFromWorld(OnClientDisconnect evtData)
    {
        if (evtData.Player == null) return;
        playerFactory.Cleanup(evtData.Player.PlayerId);
    }

    private void OnAreaDestroyed(OnAreaDestroyed evtData)
    {
        if (evtData.Area == null) return;
        areaFactory.Cleanup(evtData.Area.m_idSelf);
    }
    
    private void OnPlaceableDestroyed(OnPlaceableDestroyed evtData)
    {
        if (evtData.Placeable == null) return;
        placeableFactory.Cleanup(evtData.Placeable.m_idSelf);
    }
    
    private void OnCreatureDestroyed(OnCreatureDestroyed evtData)
    {
        if (evtData.Creature == null) return;
        creatureFactory.Cleanup(evtData.Creature.m_idSelf);
    }
}