namespace App.Scripts.Engine.Time
{
    public sealed class UnityTimeService : IUnityTimeService
    {
        public float DeltaTime => UnityEngine.Time.deltaTime;
        public float InGameTime => UnityEngine.Time.time;
        public float RealtimeSinceStartup => UnityEngine.Time.realtimeSinceStartup;
    }
}