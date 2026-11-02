using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Scripts.AppFlow
{
    public interface IAppFlowService
    {
        UniTask BootAsync(CancellationToken ct = default);
        UniTask ToGameAsync(CancellationToken ct = default);
    }
}
