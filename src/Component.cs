using Anvil.API;

namespace Goon.ComponentSystem;

/// <summary>
/// A component that can be attached to an entity.
/// </summary>
public abstract class Component<T>
{
    /// <summary>
    /// The Entity that this component is attached to.
    /// </summary>
    public T Entity { get; internal set; } = default(T)!;
    
    public Component()
    {
        InitTaskRunner();
    }

    #region TaskRunner
    
    internal readonly CancellationTokenSource cancellationTokenSource = new ();
    
    public class CancellableDelays
    {
        private readonly CancellationToken _cancellationToken;

        internal CancellableDelays(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
        }

        public async Task Delay(TimeSpan delay)
        {
            await NwTask.Delay(delay);//, _cancellationToken);
            if (_cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
        }

        public async Task Delay(float seconds)
        {
            await NwTask.Delay(TimeSpan.FromSeconds(seconds));//), _cancellationToken);
            if (_cancellationToken.IsCancellationRequested) throw new TaskCanceledException();
        }
    }

    private CancellableDelays _cancellableDelays = null!;
    
    private void InitTaskRunner()
    {
        _cancellableDelays = new CancellableDelays(cancellationTokenSource.Token);
    }
    
    public async void StartTask(Func<CancellableDelays, Task> task)
    {
        try
        {
            await task(_cancellableDelays);
        }
        catch (TaskCanceledException)
        {
            // Do nothing. This is intended, it should stop the execution of the task.
            // This is a safety feature: component tasks do not continue to run after their entity has been destroyed.
        }
    }
    
    #endregion

    #region Lifecycle

    internal void _OnAwake() => OnAwake();
    internal void _OnDestroy() => OnDestroy();

    /// <summary>
    /// This is called immediately after the component is attached to an entity.
    /// </summary>
    protected virtual void OnAwake() { }
    
    /// <summary>
    /// This is called when the component is destroyed. Either by the entity being destroyed or by the
    /// component being removed from the entity.
    /// </summary>
    protected virtual void OnDestroy() { }
    #endregion
}