using System;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Toolkit.Base
{
    public sealed class EventRegistry : IDisposable
    {
        private Action _unregisterActions;
        private bool _isDisposed;

        public void RegisterCallback<TEvent>(
            VisualElement element,
            Action<TEvent> callback) where TEvent : EventBase<TEvent>, new()
        {
            ThrowIfDisposedOrInvalid(element, callback);

            EventCallback<TEvent> eventCallback = new EventCallback<TEvent>(callback);
            element.RegisterCallback(eventCallback);
            _unregisterActions += () => element.UnregisterCallback(eventCallback);
        }

        public void RegisterCallback<TEvent>(
            VisualElement element,
            Action callback) where TEvent : EventBase<TEvent>, new()
        {
            ThrowIfDisposedOrInvalid(element, callback);

            EventCallback<TEvent> eventCallback = new EventCallback<TEvent>(_ => callback());
            element.RegisterCallback(eventCallback);
            _unregisterActions += () => element.UnregisterCallback(eventCallback);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _unregisterActions?.Invoke();
            _unregisterActions = null;
            _isDisposed = true;
        }

        private void ThrowIfDisposedOrInvalid(VisualElement element, Delegate callback)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(EventRegistry));
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            if (callback == null)
                throw new ArgumentNullException(nameof(callback));
        }
    }
}
