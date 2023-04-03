using System.Reflection;
using Anvil.Services;
using NWN.Native.API;

namespace Goon.ComponentSystem;

[ServiceBinding(typeof(ComponentSystem))]
public unsafe class ComponentSystem : IDisposable
{
    internal static ComponentSystem Instance { get; private set; } = null!;

    private delegate void ObjectDestructorHook(void* pObject);

    private readonly FunctionHook<ObjectDestructorHook> _objectDestructorHook;

    private delegate void RemovePcFromWorldHook(void* pServerAppInternal, void* pPlayer);

    private readonly FunctionHook<RemovePcFromWorldHook> _removePcFromWorldHook;

    private InjectionService InjectionService { get; init; }

    private readonly List<(Assembly, WeakReference<ComponentStorage>)> _componentStorages = new();
    private readonly ComponentStorage _defaultStorage;

    public ComponentSystem(HookService hookService, InjectionService injectionService)
    {
        #region Services

        InjectionService = injectionService;

        #endregion

        #region Hooks

        _objectDestructorHook = hookService.RequestHook<ObjectDestructorHook>(OnObjectDestructor,
            FunctionsLinux._ZN10CNWSObjectD1Ev, HookOrder.Early);
        _removePcFromWorldHook = hookService.RequestHook<RemovePcFromWorldHook>(OnRemovePCFromWorld,
            FunctionsLinux._ZN21CServerExoAppInternal17RemovePCFromWorldEP10CNWSPlayer, HookOrder.Early);

        #endregion

        #region Storage
        
        _defaultStorage = new ComponentStorage(injectionService);
        _componentStorages.Add((null!, new WeakReference<ComponentStorage>(_defaultStorage)));
        
        #endregion

        Instance = this;
    }

    private void OnRemovePCFromWorld(void* pServerAppInternal, void* pPlayer)
    {
        var player = CNWSPlayer.FromPointer(pPlayer);
        
        PerformCleanup(storage => storage.playerFactory.Cleanup(player.m_nPlayerID));

        _removePcFromWorldHook.CallOriginal(pServerAppInternal, pPlayer);
    }

    private void OnObjectDestructor(void* pObject)
    {
        var gameObject = CNWSObject.FromPointer(pObject);

        switch (gameObject.m_nObjectType)
        {
            case (byte)ObjectType.Placeable:
                PerformCleanup(storage => storage.placeableFactory.Cleanup(gameObject.m_idSelf));
                break;
        }

        _objectDestructorHook.CallOriginal(pObject);
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

    public void Dispose()
    {
        _objectDestructorHook.Dispose();
        _removePcFromWorldHook.Dispose();
    }
}