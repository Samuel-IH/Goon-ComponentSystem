using System.Reflection;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

[ServiceBinding(typeof(ComponentSystem))]
public unsafe class ComponentSystem
{
    internal static ComponentSystem Instance { get; private set; } = null!;

    private InjectionService InjectionService { get; init; }

    private readonly List<(Assembly, WeakReference<ComponentStorage>)> _componentStorages = new();
    private readonly ComponentStorage _defaultStorage;

    public ComponentSystem(InjectionService injectionService, EventService eventService)
    {
        #region Services

        InjectionService = injectionService;

        #endregion

        #region Events
        
        eventService.SubscribeAll<OnObjectDestroyed, OnObjectDestroyed.Factory>(OnObjectDestructor);
        eventService.SubscribeAll<OnRemovePcFromWorldEventData, OnRemovePcFromWorldEventData.Factory>(OnRemovePCFromWorld);
        
        #endregion

        #region Storage
        
        _defaultStorage = new ComponentStorage(injectionService);
        _componentStorages.Add((null!, new WeakReference<ComponentStorage>(_defaultStorage)));
        
        #endregion

        Instance = this;
    }

    private void OnRemovePCFromWorld(OnRemovePcFromWorldEventData evtData)
    {
        PerformCleanup(storage => storage.playerFactory.Cleanup(evtData.Player.m_nPlayerID));
    }

    private void OnObjectDestructor(OnObjectDestroyed evtData)
    {
        var gameObject = evtData.Object;
        switch (gameObject.m_nObjectType)
        {
            case (byte)ObjectType.Placeable:
                PerformCleanup(storage => storage.placeableFactory.Cleanup(gameObject.m_idSelf));
                break;
        }
    }
    
    private void PerformCleanup(Action<ComponentStorage> action)
    {
        var needsPurge = false;
        
        foreach (var storage in _componentStorages)
        {
            if (storage.Item2.TryGetTarget(out var componentStorage))
            {
                action(componentStorage);
            }
            else
            {
                needsPurge = true;
            }
        }
        
        if (needsPurge)
        {
            _componentStorages.RemoveAll(storage => !storage.Item2.TryGetTarget(out _));
        }
    }
    
    internal TReturn PerformFactoryOperationForType<T, TReturn>(Func<ComponentStorage, TReturn> action)
    {
        var assembly = typeof(T).Assembly;
        var needsPurge = false;
        
        foreach (var storage in _componentStorages)
        {
            if (storage.Item1 != assembly) continue;
            if (storage.Item2.TryGetTarget(out var componentStorage))
            {
                return action(componentStorage);
            }
            else
            {
                needsPurge = true;
            }
        }
        
        if (needsPurge)
        {
            _componentStorages.RemoveAll(storage => !storage.Item2.TryGetTarget(out _));
        }
        
        return action(_defaultStorage);
    }
    
    internal void PerformFactoryOperationForType<T>(Action<ComponentStorage> action)
    {
        var assembly = typeof(T).Assembly;
        var needsPurge = false;
        
        foreach (var storage in _componentStorages)
        {
            if (storage.Item1 != assembly) continue;
            if (storage.Item2.TryGetTarget(out var componentStorage))
            {
                action(componentStorage);
            }
            else
            {
                needsPurge = true;
            }
        }
        
        if (needsPurge)
        {
            _componentStorages.RemoveAll(storage => !storage.Item2.TryGetTarget(out _));
        }
        
        action(_defaultStorage);
    }
    
    /// <summary>
    /// Registers and returns a new ComponentStorage instance. Make sure to keep a strong reference to this instance
    /// or your components will go poof! The storage returned will be used to contain any components that are of the
    /// same assembly as the storage.
    /// </summary>
    /// <returns></returns>
    public ComponentStorage RegisterStorageForAssembly(Assembly assembly)
    {
        var storage = new ComponentStorage(InjectionService);
        _componentStorages.Add((assembly, new WeakReference<ComponentStorage>(storage)));
        return storage;
    }
}