using App.Scripts.UI.Toolkit.Base;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Factory
{
    public interface IUIViewFactory
    {
        TView Create<TView>(VisualElement root) where TView : UIView;
    }
}