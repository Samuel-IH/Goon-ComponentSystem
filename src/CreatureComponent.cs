namespace Goon.ComponentSystem;

using ComponentType = Anvil.API.NwCreature;

public abstract class CreatureComponent : Component<ComponentType> { }

public static class CreatureComponentExtensions
{
    public static void DestroyComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        ComponentSystem.Instance.creatureFactory.DestroyComponent(entity, component);
    }

    public static T? GetComponent<T>(this ComponentType entity) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.creatureFactory.GetComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.creatureFactory.AddComponent<T>(entity);
    }

    public static T AddComponent<T>(this ComponentType entity, T component) where T : Component<ComponentType>
    {
        return ComponentSystem.Instance.creatureFactory.AddComponent<T>(entity, component);
    }

    public static T GetOrAddComponent<T>(this ComponentType entity) where T : Component<ComponentType>, new()
    {
        return ComponentSystem.Instance.creatureFactory.GetOrAddComponent<T>(entity);
    }
}