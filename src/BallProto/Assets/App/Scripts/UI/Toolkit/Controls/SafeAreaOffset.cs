using UnityEngine;

namespace App.Scripts.UI.Toolkit.Controls
{
    internal readonly struct SafeAreaOffset
    {
        private const float TOLERANCE = 0.01f;

        public readonly float Left;
        public readonly float Right;
        public readonly float Top;
        public readonly float Bottom;

        public SafeAreaOffset(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }

        public bool Equals(SafeAreaOffset other)
        {
            return Mathf.Abs(Left - other.Left) < TOLERANCE &&
                   Mathf.Abs(Right - other.Right) < TOLERANCE &&
                   Mathf.Abs(Top - other.Top) < TOLERANCE &&
                   Mathf.Abs(Bottom - other.Bottom) < TOLERANCE;
        }
    }
}
