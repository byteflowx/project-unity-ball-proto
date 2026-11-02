using UnityEngine;

namespace App.Scripts.Game
{
    public sealed class Door : MonoBehaviour
    {
        private const float OPEN_SECONDS = 0.7f;
        private const float OPEN_ANGLE = 78f;
        private const float BOB_SPEED = 2.2f;
        private const float BOB_HEIGHT = 0.12f;
        private const float OPEN_LIFT = 2.2f;
        private const float FLAME_SPIN = 40f;

        [SerializeField] private Transform[] _petals;
        [SerializeField] private Transform _flame;

        private Quaternion[] _closed;
        private Vector3[] _axes;
        private Vector3 _flamePosition;
        private float _age;
        private float _open;
        private bool _isOpening;

        public void Open() => _isOpening = true;

        public void Tick(float deltaTime)
        {
            _age += deltaTime;

            if (_isOpening && _open < 1f)
            {
                _open = Mathf.Min(_open + deltaTime / OPEN_SECONDS, 1f);
                float eased = 1f + 2.2f * Mathf.Pow(_open - 1f, 3f) + 1.2f * Mathf.Pow(_open - 1f, 2f);

                for (int i = 0; i < _petals.Length; i++)
                    _petals[i].rotation = Quaternion.AngleAxis(OPEN_ANGLE * eased, _axes[i]) * _closed[i];
            }

            _flame.localPosition = _flamePosition + Vector3.up * (Mathf.Sin(_age * BOB_SPEED) * BOB_HEIGHT + _open * OPEN_LIFT);
            _flame.Rotate(0f, FLAME_SPIN * deltaTime, 0f, Space.World);
        }

        private void Awake()
        {
            _flamePosition = _flame.localPosition;
            _closed = new Quaternion[_petals.Length];
            _axes = new Vector3[_petals.Length];

            for (int i = 0; i < _petals.Length; i++)
            {
                _closed[i] = _petals[i].rotation;
                _axes[i] = Vector3.Cross(Vector3.up, _petals[i].position - transform.position).normalized;
            }
        }
    }
}
