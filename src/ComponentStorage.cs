using Anvil.API;
using Anvil.Services;

namespace Goon.ComponentSystem;

/// <summary>
/// Stores components. Use this class if you need to store your components within your own
/// assembly. A good use-case for this would be supporting hot-reload.
/// Call <see cref="ComponentSystem.RegisterStorageForAssembly"/> to use it.
/// </summary>
public sealed class ComponentStorage
{
    internal readonly GenericComponentFactory<NwPlayer> playerFactory;
    internal readonly GenericComponentFactory<NwPlaceable> placeableFactory;
    
    internal ComponentStorage(InjectionService injectionService)
    {
        playerFactory = new GenericComponentFactory<NwPlayer>(injectionService, player => player.PlayerId);
        placeableFactory = new GenericComponentFactory<NwPlaceable>(injectionService, placeable => placeable.ObjectId);
    }
}