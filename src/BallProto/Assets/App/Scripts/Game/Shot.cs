using UnityEngine;

namespace App.Scripts.Game
{
    public sealed class Shot : MonoBehaviour
    {
        private const float SPEED = 24f;
        private const float SINK_RATIO = 0.3f;
        private const float TRAIL_WIDTH = 1.7f;

        [SerializeField] private TrailRenderer _trail;

        public float Radius { get; private set; }
        public Vector2 Flat => new(transform.position.x, transform.position.z);

        public void Grow(Vector3 position, float radius)
        {
            Radius = radius;
            transform.position = position;
            transform.localScale = Vector3.one * radius;
        }

        public void Launch()
        {
            _trail.widthMultiplier = Radius * TRAIL_WIDTH;
            _trail.emitting = true;
        }

        public void Fly(Vector2 aim, float deltaTime)
        {
            float step = SPEED * deltaTime;
            Vector2 flat = Vector2.MoveTowards(Flat, aim, step);
            transform.position = new Vector3(flat.x, Mathf.MoveTowards(transform.position.y, Radius, step * SINK_RATIO), flat.y);
        }
    }
}
