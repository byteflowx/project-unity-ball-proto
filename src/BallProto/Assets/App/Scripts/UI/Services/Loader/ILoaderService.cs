using UnityEngine.UIElements;

namespace App.Scripts.UI.Services.Loader
{
    public interface ILoaderService
    {
        void Initialize(VisualElement loaderContainer);
        void Show();
        void Hide();
    }
}
