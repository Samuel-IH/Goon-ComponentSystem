namespace Goon.ComponentSystem;

using ComponentType = Anvil.API.NwArea;

public abstract class AreaComponent : Component<ComponentType> { }

public static class AreaComponentExtensions
{
    public static void DestroyComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        ComponentSystem.Instance.areaFactory.DestroyComponent(entity, component);
    }

    public static T? GetComponent<T>(this ComponentType entity) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.areaFactory.GetComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.areaFactory.AddComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.areaFactory.AddComponent<T>(entity, component);
    }

    public static T GetOrAddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.areaFactory.GetOrAddComponent<T>(entity);
    }
}