using System;
using System.Threading;
using App.Scripts.Core.Constants;
using App.Scripts.Engine.Audio;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Toolkit.Base
{
    public abstract class UIView : IDisposable
    {
        private readonly CancellationTokenSource _lifetime = new();

        protected VisualElement Root { get; private set; }
        protected EventRegistry Events { get; private set; }
        protected IAudioService Audio { get; private set; }

        protected abstract string VisibleClass { get; }
        protected abstract string HiddenClass { get; }

        protected CancellationToken LifetimeToken => _lifetime.Token;

        public void Initialize(VisualElement root, IAudioService audio)
        {
            Root = root;
            Audio = audio;
            Events = new EventRegistry();

            SetVisualElements();
            RegisterCallbacks();

            Events.RegisterCallback<ClickEvent>(Root, OnRootClicked);

            HideImmediately();
        }

        public virtual void Show()
        {
            Root.style.display = DisplayStyle.Flex;
            Root.RemoveFromClassList(HiddenClass);
            Root.AddToClassList(VisibleClass);

            OnShow();
        }

        public virtual void HideImmediately()
        {
            Root.style.display = DisplayStyle.None;
            Root.RemoveFromClassList(VisibleClass);
            Root.RemoveFromClassList(HiddenClass);
        }

        public virtual void Dispose()
        {
            _lifetime.Cancel();
            _lifetime.Dispose();
            Events?.Dispose();
        }

        protected abstract void SetVisualElements();
        protected abstract void RegisterCallbacks();
        protected virtual void OnShow() { }

        private void OnRootClicked(ClickEvent evt)
        {
            if (IsInteractive(evt.target as VisualElement))
                Audio.PlaySound(SoundKeys.BUTTON_CLICK);
        }

        private bool IsInteractive(VisualElement element)
        {
            while (element != null && element != Root)
            {
                if (element.focusable)
                    return true;

                element = element.parent;
            }

            return false;
        }
    }
}
