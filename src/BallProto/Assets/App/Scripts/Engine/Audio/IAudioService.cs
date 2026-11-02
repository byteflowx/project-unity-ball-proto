using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Engine.Audio
{
    public interface IAudioService
    {
        UniTask InitializeAsync(CancellationToken ct = default);
        void PlaySound(string soundKey);
        void StopSound(string soundKey);
        UniTask PlayMusicAsync(string address, CancellationToken ct = default);
    }
}
