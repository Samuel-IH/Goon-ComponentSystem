namespace Goon.ComponentSystem;

using ComponentType = Anvil.API.NwPlaceable;

public abstract class PlaceableComponent : Component<ComponentType> { }

public static class PlaceableComponentExtensions
{
    public static void DestroyComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        ComponentSystem.Instance.placeableFactory.DestroyComponent(entity, component);
    }

    public static T? GetComponent<T>(this ComponentType entity) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.placeableFactory.GetComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.placeableFactory.AddComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.placeableFactory.AddComponent<T>(entity, component);
    }

    public static T GetOrAddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.placeableFactory.GetOrAddComponent<T>(entity);
    }
}