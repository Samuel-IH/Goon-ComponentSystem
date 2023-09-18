using Anvil.API;
using Anvil.Services;

namespace Goon.ComponentSystem;

    internal class GenericComponentFactory<TEntity> where TEntity : notnull
    {
        private readonly Dictionary<uint, List<Component<TEntity>>> _entityMap = new();
        private readonly InjectionService _injectionService;
        private readonly Func<TEntity, uint> _getNativeId;

        public GenericComponentFactory(InjectionService injectionService, Func<TEntity, uint> getNativeId)
        {
            _injectionService = injectionService;
            _getNativeId = getNativeId;
        }

        internal void Cleanup(uint nativeId)
        {
            if (!_entityMap.TryGetValue(nativeId, out List<Component<TEntity>>? destroyableComponents)) return;

            foreach (var component in destroyableComponents)
            {
                NwTask.Run(() =>
                {
                    component._OnDestroy();
                    component.cancellationTokenSource.Cancel();
                    return Task.CompletedTask;
                });
            }

            _entityMap.Remove(nativeId);
        }

        internal T AddComponent<T>(TEntity entity) where T : Component<TEntity>, new()
        {
            return AddComponent(entity, new T());
        }

        internal T AddComponent<T>(TEntity entity, T component) where T : Component<TEntity>
        {
            var id = _getNativeId(entity);
            if (!_entityMap.TryGetValue(id, out List<Component<TEntity>>? components))
            {
                components = new List<Component<TEntity>>();
                _entityMap[id] = components;
            }

            _injectionService.Inject(component);
            component.Entity = entity;
            components.Add(component);
            component._OnAwake();

            return component;
        }

        internal T? GetComponent<T>(TEntity entity) where T : Component<TEntity>
        {
            if (!_entityMap.TryGetValue(_getNativeId(entity), out List<Component<TEntity>>? components)) return null;

            foreach (var component in components)
            {
                if (component as T is T foundComponent)
                {
                    return foundComponent;
                }
            }

            return null;
        }

        internal T GetOrAddComponent<T>(TEntity entity) where T : Component<TEntity>, new()
        {
            if (GetComponent<T>(entity) is T component) return component;

            return AddComponent<T>(entity);
        }

        internal void DestroyComponent<T>(TEntity entity, T component) where T : Component<TEntity>
        {
            if (!_entityMap.TryGetValue(_getNativeId(entity), out var destroyableComponents)) return;

            if (destroyableComponents.Remove(component))
            {
                NwTask.Run(() =>
                {
                    component._OnDestroy();
                    component.cancellationTokenSource.Cancel();
                    return Task.CompletedTask;
                });
            }
        }
    }
    