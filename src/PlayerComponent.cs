namespace Goon.ComponentSystem;

using ComponentType = Anvil.API.NwPlayer;

public abstract class PlayerComponent : Component<ComponentType> { }

public static class PlayerComponentExtensions
{
    public static void DestroyComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        ComponentSystem.Instance.playerFactory.DestroyComponent(entity, component);
    }

    public static T? GetComponent<T>(this ComponentType entity) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.playerFactory.GetComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.playerFactory.AddComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.playerFactory.AddComponent<T>(entity, component);
    }

    public static T GetOrAddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.playerFactory.GetOrAddComponent<T>(entity);
    }
}