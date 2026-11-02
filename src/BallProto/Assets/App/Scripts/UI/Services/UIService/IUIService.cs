using System.Threading;
using App.Scripts.UI.Toolkit.Base;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Services.UIService
{
    public interface IUIService
    {
        VisualElement Root { get; }
        ReadOnlyReactiveProperty<UIScreen> CurrentScreen { get; }

        UniTask InitializeAsync(CancellationToken ct);
        UniTask ShowScreenAsync<TScreen>(CancellationToken ct = default) where TScreen : UIScreen;
        void CloseCurrentScreen();
    }
}
