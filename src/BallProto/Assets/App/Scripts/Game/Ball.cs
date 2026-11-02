using System;
using UnityEngine;

namespace App.Scripts.Game
{
    public sealed class Ball : MonoBehaviour
    {
        private const float HOP_SECONDS = 0.3f;
        private const float TAKE_OFF_KICK = -3.2f;
        private const float LANDING_KICK = 5.5f;
        private const float STIFFNESS = 260f;
        private const float DAMPING = 15f;
        private const float MIN_SQUASH = -0.32f;
        private const float MAX_SQUASH = 0.38f;
        private const float SIDE_BULGE = 0.6f;

        [SerializeField] private Transform _body;

        private Vector3 _from;
        private Vector3 _to;
        private float _hop = 1f;
        private float _hopHeight;
        private float _squash;
        private float _squashSpeed;

        public float Radius { get; set; } = 1f;
        public bool IsHopping => _hop < 1f;
        public Vector3 Centre => _body.position;

        public event Action Landed;

        public void Kick(float speed) => _squashSpeed += speed;

        public void Hop(Vector3 to, float height)
        {
            _from = transform.position;
            _to = to;
            _hop = 0f;
            _hopHeight = height;
            Kick(TAKE_OFF_KICK);
        }

        public void Tick(float deltaTime)
        {
            float lift = 0f;

            if (IsHopping)
            {
                _hop = Mathf.Min(_hop + deltaTime / HOP_SECONDS, 1f);
                transform.position = Vector3.Lerp(_from, _to, _hop);
                lift = _hopHeight * 4f * _hop * (1f - _hop);

                if (_hop >= 1f)
                {
                    Kick(LANDING_KICK);
                    Landed?.Invoke();
                }
            }

            _squashSpeed += (-_squash * STIFFNESS - _squashSpeed * DAMPING) * deltaTime;
            _squash = Mathf.Clamp(_squash + _squashSpeed * deltaTime, MIN_SQUASH, MAX_SQUASH);

            float height = Radius * (1f - _squash);
            float width = Radius * (1f + _squash * SIDE_BULGE);
            _body.localScale = new Vector3(width, height, width);
            _body.localPosition = new Vector3(0f, height + lift, 0f);
        }
    }
}
