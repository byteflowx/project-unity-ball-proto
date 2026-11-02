using System;
using App.Scripts.Engine.Audio;
using App.Scripts.UI.Toolkit.Base;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Factory
{
    public sealed class UIViewFactory : IUIViewFactory
    {
        private readonly Func<Type, UIView> _create;
        private readonly IAudioService _audio;

        public UIViewFactory(Func<Type, UIView> create, IAudioService audio)
        {
            _create = create;
            _audio = audio;
        }

        public TView Create<TView>(VisualElement root) where TView : UIView
        {
            TView view = (TView)_create(typeof(TView));
            view.Initialize(root, _audio);
            return view;
        }
    }
}
