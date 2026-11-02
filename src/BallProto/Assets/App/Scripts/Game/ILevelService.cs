using System.Threading;
using Cysharp.Threading.Tasks;

namespace App.Scripts.Game
{
    public interface ILevelService
    {
        UniTask LoadAsync(CancellationToken ct = default);
    }
}
