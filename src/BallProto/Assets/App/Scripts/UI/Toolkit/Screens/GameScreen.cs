using App.Scripts.AppFlow;
using App.Scripts.Core.Common;
using App.Scripts.UI.Constants;
using App.Scripts.UI.Toolkit.Base;
using App.Scripts.UI.Toolkit.Controls;
using Cysharp.Threading.Tasks;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Toolkit.Screens
{
    public sealed class GameScreen : UIScreen
    {
        private readonly IAppFlowService _appFlow;

        private Label _hint;
        private Label _title;
        private VisualElement _result;
        private RoundedButton _retryButton;

        public GameScreen(IAppFlowService appFlow)
        {
            _appFlow = appFlow;
        }

        public void HideHint() => _hint.style.display = DisplayStyle.None;

        public void ShowResult(string title)
        {
            _title.text = title;
            _result.style.display = DisplayStyle.Flex;
        }

        protected override void SetVisualElements()
        {
            _hint = Root.Q<Label>(UxmlGameNames.HINT);
            _title = Root.Q<Label>(UxmlGameNames.TITLE);
            _result = Root.Q<VisualElement>(UxmlGameNames.RESULT);
            _retryButton = Root.Q<RoundedButton>(UxmlGameNames.RETRY_BUTTON);
        }

        protected override void RegisterCallbacks()
        {
            Events.RegisterCallback<ClickEvent>(_retryButton, OnRetryClicked);
        }

        protected override void OnShow()
        {
            _hint.style.display = DisplayStyle.Flex;
            _result.style.display = DisplayStyle.None;
        }

        private void OnRetryClicked()
        {
            _appFlow.ToGameAsync(LifetimeToken).Forget(Log.ExceptionUnlessCanceled);
        }
    }
}
