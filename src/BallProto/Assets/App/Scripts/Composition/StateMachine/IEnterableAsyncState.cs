using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Composition.StateMachine
{
    public interface IEnterableAsyncState
    {
        UniTask<Next> EnterAsync(CancellationToken ct = default);
    }
}
