namespace App.Scripts.UI.Toolkit.Base
{
    public abstract class UIScreen : UIView
    {
        private const string VISIBLE_CLASS = "screen--visible";
        private const string HIDDEN_CLASS = "screen--hidden";

        protected override string VisibleClass => VISIBLE_CLASS;
        protected override string HiddenClass => HIDDEN_CLASS;
    }
}
