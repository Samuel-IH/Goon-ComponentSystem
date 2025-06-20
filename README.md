# Goon Component System

## Overview

The Goon Component System is a lightweight component-based architecture built on top of Anvil. It provides a modular way to manage and extend game entities like players, creatures, areas, placeables, and generic objects in Neverwinter Nights: Enhanced Edition servers.

## Features

- **Component System**: A robust and extensible system for attaching components to entities.
- **Entity-Specific Components**: Specialized component implementations for areas, creatures, players, placeables, and game objects.
- **Task Management**: Built-in support for cancellable tasks tied to the lifecycle of entities.
- **Integration with Anvil API**: Seamlessly integrates with Anvil's services and events.
- **Custom Destruction Events**: Hooks into native object and area destructors for cleanup.

## Installation

Add the dependency to your `paket.dependencies` file:

```plaintext
nuget Goon.ComponentSystem
```

Include the dependency in your server project.

That's it! The component system will be available for use in your Anvil-based server.

## Usage

### Adding a Component

You can create custom components by extending the base component classes, such as `CreatureComponent`, `AreaComponent`, etc. Here's an example of adding a creature component:

```csharp
public class MyCreatureComponent : CreatureComponent
{
    public void DoSomething()
    {
        // Custom functionality
    }
}

// Example usage:
var creature = ... // Obtain an NwCreature instance
var component = creature.AddComponent<MyCreatureComponent>();
component.DoSomething();
```

### Removing a Component

To destroy a specific component attached to an entity:

```csharp
creature.DestroyComponent(component);
```

### Custom Events

The system hooks into object and area destructors, ensuring proper cleanup when entities are destroyed.

## Code Structure

- **`Component.cs`**: Abstract base class for all components, providing task management and lifecycle features.
- **`CreatureComponent.cs`**: Specialized component logic for creatures.
- **`AreaComponent.cs`**: Component management for areas.
- **`PlayerComponent.cs`**: Extension methods and structure for player-specific components.
- **`PlaceableComponent.cs`**: Support for placeable objects.
- **`GameObjectComponent.cs`**: Generalized components for game objects.
- **`ComponentSystem.cs`**: Core system managing component factories and event subscriptions.
- **`OnCreatureDestructor.cs`, `OnObjectDestructor.cs`, `OnAreaDestructor.cs`**: Event hooks for handling destruction of entities.

## Dependencies

- **Anvil**

## Contributing

Contributions are welcome! Feel free to fork the repository and submit a pull request.

## License

This project is licensed under the MIT License.