using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Engine.SceneLoader
{
    public interface ISceneLoaderService
    {
        UniTask LoadAsync(string sceneName, CancellationToken ct = default);
    }
}
