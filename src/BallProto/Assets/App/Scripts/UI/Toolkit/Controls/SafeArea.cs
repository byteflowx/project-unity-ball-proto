using UnityEngine;
using UnityEngine.UIElements;

namespace App.Scripts.UI.Toolkit.Controls
{
    [UxmlElement]
    public partial class SafeArea : VisualElement
    {
        private readonly VisualElement _contentContainer;
        private SafeAreaOffset _lastAppliedOffset;

        public override VisualElement contentContainer => _contentContainer;

        public SafeArea()
        {
            style.position = Position.Absolute;
            style.top = 0;
            style.bottom = 0;
            style.left = 0;
            style.right = 0;
            pickingMode = PickingMode.Ignore;

            _contentContainer = new VisualElement
            {
                name = "safeAreaContent",
                pickingMode = PickingMode.Ignore,
                style =
                {
                    flexGrow = 1,
                    flexShrink = 0
                }
            };
            hierarchy.Add(_contentContainer);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            ApplySafeArea();
        }

        private void OnGeometryChanged(GeometryChangedEvent evt)
        {
            ApplySafeArea();
        }

        private void ApplySafeArea()
        {
            if (panel == null || panel.contextType != ContextType.Player)
                return;

            SafeAreaOffset safeAreaOffset = CalculateSafeAreaOffset();

            if (safeAreaOffset.Equals(_lastAppliedOffset))
                return;

            _lastAppliedOffset = safeAreaOffset;

            SafeAreaOffset marginOffset = GetMarginOffset();

            _contentContainer.style.marginLeft = Mathf.Max(marginOffset.Left, safeAreaOffset.Left) - marginOffset.Left;
            _contentContainer.style.marginRight = Mathf.Max(marginOffset.Right, safeAreaOffset.Right) - marginOffset.Right;
            _contentContainer.style.marginTop = Mathf.Max(marginOffset.Top, safeAreaOffset.Top) - marginOffset.Top;
            _contentContainer.style.marginBottom = Mathf.Max(marginOffset.Bottom, safeAreaOffset.Bottom) - marginOffset.Bottom;
        }

        private SafeAreaOffset CalculateSafeAreaOffset()
        {
            Rect safeArea = Screen.safeArea;
            Vector2 leftTop = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
            Vector2 rightBottom = RuntimePanelUtils.ScreenToPanel(panel, new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

            return new SafeAreaOffset(
                leftTop.x,
                rightBottom.x,
                leftTop.y,
                rightBottom.y
            );
        }

        private SafeAreaOffset GetMarginOffset()
        {
            return new SafeAreaOffset(
                resolvedStyle.marginLeft,
                resolvedStyle.marginRight,
                resolvedStyle.marginTop,
                resolvedStyle.marginBottom
            );
        }
    }
}
