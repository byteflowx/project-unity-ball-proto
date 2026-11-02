using UnityEngine.UIElements;

namespace App.Scripts.UI.Toolkit.Controls
{
    [UxmlElement]
    public partial class RoundedButton : VisualElement
    {
        private const string ROOT_CLASS = "rounded-btn";
        private const string LABEL_CLASS = "rounded-btn__label";
        private const string PRESSED_CLASS = "rounded-btn--pressed";

        private readonly Label _label;

        [UxmlAttribute]
        public string Text
        {
            get => _label?.text ?? string.Empty;
            set
            {
                if (_label != null)
                    _label.text = value;
            }
        }

        public RoundedButton()
        {
            AddToClassList(ROOT_CLASS);

            _label = new Label();
            _label.AddToClassList(LABEL_CLASS);
            _label.pickingMode = PickingMode.Ignore;
            Add(_label);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            RegisterCallback<PointerDownEvent>(OnPointerDown);
            RegisterCallback<PointerUpEvent>(OnPointerUp);
            RegisterCallback<PointerLeaveEvent>(OnPointerLeave);
        }

        private void OnAttachToPanel(AttachToPanelEvent evt)
        {
            focusable = true;
        }

        private void OnPointerDown(PointerDownEvent evt)
        {
            if (evt.button != 0 || !enabledSelf)
                return;

            AddToClassList(PRESSED_CLASS);
        }

        private void OnPointerUp(PointerUpEvent evt)
        {
            if (evt.button != 0)
                return;

            RemoveFromClassList(PRESSED_CLASS);
        }

        private void OnPointerLeave(PointerLeaveEvent evt)
        {
            RemoveFromClassList(PRESSED_CLASS);
        }
    }
}
