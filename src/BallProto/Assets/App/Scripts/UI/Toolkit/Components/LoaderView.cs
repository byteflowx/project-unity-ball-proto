using System;
using App.Scripts.Engine.Time;
using App.Scripts.UI.Constants;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Toolkit.Components
{
    public sealed class LoaderView : IDisposable
    {
        private const float ROTATION_DEGREES_PER_SECOND = 360f;
        private const long UPDATE_INTERVAL_MS = 16;
        private const string HIDDEN_CLASS = "loader--hidden";
        private const string VISIBLE_CLASS = "loader--visible";

        private readonly VisualElement _root;
        private readonly VisualElement _spinner;
        private readonly IUnityTimeService _timeService;

        private IVisualElementScheduledItem _rotationSchedule;
        private float _currentRotation;
        private float _lastUpdateTime;

        private bool IsVisible => _root.ClassListContains(VISIBLE_CLASS);

        public LoaderView(VisualElement loaderContainer, IUnityTimeService timeService)
        {
            _root = loaderContainer;
            _timeService = timeService;
            _spinner = _root.Q<VisualElement>(UxmlLoaderNames.SPINNER);

            _root.AddToClassList(HIDDEN_CLASS);
        }

        public void Show()
        {
            if (IsVisible)
                return;

            _root.RemoveFromClassList(HIDDEN_CLASS);
            _root.AddToClassList(VISIBLE_CLASS);

            StartSpinnerAnimation();
        }

        public void Hide()
        {
            if (!IsVisible)
                return;

            StopSpinnerAnimation();

            _root.RemoveFromClassList(VISIBLE_CLASS);
            _root.AddToClassList(HIDDEN_CLASS);
        }

        public void Dispose() => StopSpinnerAnimation();

        private void StartSpinnerAnimation()
        {
            _currentRotation = 0f;
            _lastUpdateTime = _timeService.RealtimeSinceStartup;

            _rotationSchedule = _spinner.schedule
                .Execute(UpdateSpinnerRotation)
                .Every(UPDATE_INTERVAL_MS)
                .StartingIn(0);
        }

        private void StopSpinnerAnimation()
        {
            _rotationSchedule?.Pause();
            _rotationSchedule = null;

            _currentRotation = 0f;
            _spinner.style.rotate = new Rotate(0f);
        }

        private void UpdateSpinnerRotation()
        {
            float currentTime = _timeService.RealtimeSinceStartup;
            float deltaTime = currentTime - _lastUpdateTime;
            _lastUpdateTime = currentTime;

            _currentRotation += ROTATION_DEGREES_PER_SECOND * deltaTime;

            if (_currentRotation >= 360f)
                _currentRotation -= 360f;

            _spinner.style.rotate = new Rotate(_currentRotation);
        }
    }
}
