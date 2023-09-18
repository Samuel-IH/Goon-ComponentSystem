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
    
    public async void StartTask<T1>(Func<T1, CancellableDelays, Task> task, T1 arg1)
    {
        try
        {
            await task(arg1, _cancellableDelays);
        }
        catch (TaskCanceledException) { }
    }
    
    public async void StartTask<T1, T2>(Func<T1, T2, CancellableDelays, Task> task, T1 arg1, T2 arg2)
    {
        try
        {
            await task(arg1, arg2, _cancellableDelays);
        }
        catch (TaskCanceledException) { }
    }
    
    public async void StartTask<T1, T2, T3>(Func<T1, T2, T3, CancellableDelays, Task> task, T1 arg1, T2 arg2, T3 arg3)
    {
        try
        {
            await task(arg1, arg2, arg3, _cancellableDelays);
        }
        catch (TaskCanceledException) { }
    }
    
    public async void StartTask<T1, T2, T3, T4>(Func<T1, T2, T3, T4, CancellableDelays, Task> task, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
    {
        try
        {
            await task(arg1, arg2, arg3, arg4, _cancellableDelays);
        }
        catch (TaskCanceledException) { }
    }
    
    public async void StartTask<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, CancellableDelays, Task> task, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
    {
        try
        {
            await task(arg1, arg2, arg3, arg4, arg5, _cancellableDelays);
        }
        catch (TaskCanceledException) { }
    }
    
    public async void StartTask<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, CancellableDelays, Task> task, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
    {
        try
        {
            await task(arg1, arg2, arg3, arg4, arg5, arg6, _cancellableDelays);
        }
        catch (TaskCanceledException) { }
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