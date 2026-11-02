using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace App.Scripts.Engine.SceneLoader
{
    public sealed class SceneLoaderService : ISceneLoaderService
    {
        private bool _isProcessing;

        public async UniTask LoadAsync(string sceneName, CancellationToken ct = default)
        {
            while (_isProcessing)
                await UniTask.Yield(ct);

            _isProcessing = true;

            try
            {
                await SceneManager.LoadSceneAsync(sceneName).ToUniTask(cancellationToken: ct);
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
