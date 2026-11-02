namespace App.Scripts.Engine.Time
{
    public interface IUnityTimeService
    {
        float DeltaTime { get; }
        float InGameTime { get; }
        float RealtimeSinceStartup { get; }
    }
}