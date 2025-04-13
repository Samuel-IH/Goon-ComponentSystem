namespace Goon.ComponentSystem;

using ComponentType = Anvil.API.NwGameObject;

public abstract class GameObjectComponent : Component<ComponentType> { }

public static class GameObjectComponentExtensions
{
    public static void DestroyComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        ComponentSystem.Instance.gameObjectFactory.DestroyComponent(entity, component);
    }

    public static T? GetComponent<T>(this ComponentType entity) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.gameObjectFactory.GetComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.gameObjectFactory.AddComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.gameObjectFactory.AddComponent<T>(entity, component);
    }

    public static T GetOrAddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.gameObjectFactory.GetOrAddComponent<T>(entity);
    }
}