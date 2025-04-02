using System.Runtime.InteropServices;
using Anvil.API;

namespace Goon.ComponentSystem;

public static class ComponentSystemPluginFinder
{
    private static ComponentSystem? _componentSystem = null;
    
    public static ComponentSystem ComponentSystem
    {
        get
        {
            if (_componentSystem != null) return _componentSystem;
            
            var ptr = NwModule.Instance.GetObjectVariable<LocalVariableString>("__plugin_ComponentSystem").Value;
            if (string.IsNullOrEmpty(ptr)) throw new Exception("ComponentSystem not registered");
            
            var handle = GCHandle.FromIntPtr(IntPtr.Parse(ptr));
            if (handle.Target is not ComponentSystem componentSystem) throw new Exception("ComponentSystem not registered");
            
            _componentSystem = componentSystem;
            return _componentSystem;
        }
    }
    
    internal static void Register(ComponentSystem instance)
    {
        var handle = GCHandle.Alloc(instance);
        var str = GCHandle.ToIntPtr(handle).ToString();
        NwModule.Instance.GetObjectVariable<LocalVariableString>("__plugin_ComponentSystem").Value = str;
    }
}
