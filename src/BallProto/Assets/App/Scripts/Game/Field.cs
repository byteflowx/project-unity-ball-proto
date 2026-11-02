using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Game
{
    public sealed class Field : MonoBehaviour
    {
        private const float WAVE_SECONDS_PER_POWER = 0.05f;
        private const float INFECTED_SECONDS = 0.26f;
        private const float DEBRIS_HEIGHT = 0.8f;
        private const int DEBRIS_PER_OBSTACLE = 9;

        [SerializeField] private MeshRenderer _obstaclePrefab;
        [SerializeField] private Material _healthy;
        [SerializeField] private Material _sick;
        [SerializeField] private Material _infected;
        [SerializeField] private ParticleSystem _debris;

        private readonly List<Infection> _infections = new();
        private MeshRenderer[] _obstacles;
        private Level _level;

        public event Action Exploded;

        public void Build(Level level, float obstacleRadius)
        {
            _level = level;
            _obstacles = new MeshRenderer[level.Count];
            System.Random random = new(level.Count);

            for (int i = 0; i < level.Count; i++)
            {
                Vector2 point = level.Point(i);
                Quaternion rotation = Quaternion.Euler(0f, (float)random.NextDouble() * 40f - 20f, 0f);
                float width = obstacleRadius * 2f * (0.92f + (float)random.NextDouble() * 0.16f);

                _obstacles[i] = Instantiate(_obstaclePrefab, new Vector3(point.x, 0f, point.y), rotation, transform);
                _obstacles[i].transform.localScale = new Vector3(width, width * (1.6f + (float)random.NextDouble() * 0.4f), width);
            }
        }

        public void Preview(int first, float power)
        {
            _level.Spread(first, power);
            ApplyPreview();
        }

        public void ClearPreview()
        {
            _level.Reach.Clear();
            ApplyPreview();
        }

        public float Blast(int first, float power)
        {
            _level.Spread(first, power);
            _level.KillReached();

            float last = 0f;

            foreach ((int index, float left) in _level.Reach)
            {
                float delay = (power - left) * WAVE_SECONDS_PER_POWER;
                last = Mathf.Max(last, delay);
                _infections.Add(new Infection { Obstacle = _obstacles[index], Time = -delay, Scale = _obstacles[index].transform.localScale });
            }

            return last + INFECTED_SECONDS;
        }

        public void Tick(float deltaTime)
        {
            for (int i = _infections.Count - 1; i >= 0; i--)
            {
                Infection infection = _infections[i];
                infection.Time += deltaTime;

                if (infection.Time < 0f)
                    continue;

                infection.Obstacle.sharedMaterial = _infected;

                float t = infection.Time / INFECTED_SECONDS;
                float swell = 0.3f * t * t + 0.07f * Mathf.Sin(t * 26f) * (1f - t);
                infection.Obstacle.transform.localScale = Vector3.Scale(infection.Scale, new Vector3(1f + swell, 1f - swell * 0.5f, 1f + swell));

                if (t >= 1f)
                {
                    Explode(infection.Obstacle.gameObject);
                    _infections.RemoveAt(i);
                }
            }
        }

        private void ApplyPreview()
        {
            for (int i = 0; i < _obstacles.Length; i++)
            {
                if (_level.IsAlive(i))
                    _obstacles[i].sharedMaterial = _level.Reach.ContainsKey(i) ? _sick : _healthy;
            }
        }

        private void Explode(GameObject obstacle)
        {
            obstacle.SetActive(false);
            _debris.Emit(new ParticleSystem.EmitParams { position = obstacle.transform.position + Vector3.up * DEBRIS_HEIGHT, applyShapeToPosition = true }, DEBRIS_PER_OBSTACLE);
            Exploded?.Invoke();
        }
    }
}
