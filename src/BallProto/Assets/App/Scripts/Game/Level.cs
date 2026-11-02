using System;
using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Game
{
    public sealed class Level
    {
        private readonly LevelConfig _config;
        private readonly List<Vector2> _points = new();
        private readonly List<float> _along = new();
        private readonly List<float> _side = new();
        private readonly List<int[]> _neighbours = new();
        private readonly Queue<int> _wave = new();
        private bool[] _alive;

        public Vector2 Direction { get; }
        public float Length { get; }
        public int Count => _points.Count;
        public Dictionary<int, float> Reach { get; } = new();

        public Level(LevelConfig config)
        {
            _config = config;
            Direction = config.DoorPosition.normalized;
            Length = config.DoorPosition.magnitude;

            Generate();
        }

        public Vector2 Point(int index) => _points[index];
        public bool IsAlive(int index) => _alive[index];
        public Vector2 At(float along) => Direction * along;
        public bool Blocks(int index, float along, float ballRadius) =>
            _alive[index] && _along[index] > along && _along[index] < Length && Mathf.Abs(_side[index]) < _config.LaneHalfWidth(ballRadius) + _config.ObstacleRadius;

        public int FirstBlocker(float along, float ballRadius)
        {
            int first = -1;

            for (int i = 0; i < Count; i++)
            {
                if (Blocks(i, along, ballRadius) && (first < 0 || _along[i] < _along[first]))
                    first = i;
            }

            return first;
        }

        public float StopDistance(float along, float ballRadius)
        {
            int blocker = FirstBlocker(along, ballRadius);

            return blocker < 0
                ? Length
                : Mathf.Max(along, _along[blocker] - _config.ObstacleRadius - ballRadius - _config.Standoff);
        }

        public int Touching(Vector2 position, float radius)
        {
            float reach = radius + _config.ObstacleRadius;

            for (int i = 0; i < Count; i++)
            {
                if (_alive[i] && (_points[i] - position).sqrMagnitude < reach * reach)
                    return i;
            }

            return -1;
        }

        public int FirstHit(Vector2 from, Vector2 to, float radius)
        {
            float distance = (to - from).magnitude;

            for (float travelled = 0f; travelled < distance; travelled += 0.2f)
            {
                int hit = Touching(Vector2.MoveTowards(from, to, travelled), radius);

                if (hit >= 0)
                    return hit;
            }

            return Touching(to, radius);
        }

        public void Spread(int first, float power)
        {
            Reach.Clear();
            Reach[first] = power;
            _wave.Enqueue(first);

            while (_wave.TryDequeue(out int current))
            {
                float left = Reach[current];

                foreach (int next in _neighbours[current])
                {
                    float hop = (_points[next] - _points[current]).magnitude;
                    float nextLeft = left - hop * hop / _config.HopReference;

                    if (!_alive[next] || nextLeft <= 0f || (Reach.TryGetValue(next, out float known) && known >= nextLeft))
                        continue;

                    Reach[next] = nextLeft;
                    _wave.Enqueue(next);
                }
            }
        }

        public void KillReached()
        {
            foreach (int index in Reach.Keys)
                _alive[index] = false;
        }

        private void Generate()
        {
            System.Random random = new(_config.Seed);
            float back = -_config.HalfWidth;
            float front = Length + _config.PadBehindDoor;
            int tries = Mathf.RoundToInt((front - back) * _config.HalfWidth * 12f);

            foreach (Vector2 loner in _config.Loners)
                Add(loner.x, loner.y);

            for (int i = 0; i < tries; i++)
            {
                float along = Mathf.Lerp(back, front, (float)random.NextDouble());
                float side = Mathf.Lerp(-_config.HalfWidth, _config.HalfWidth, (float)random.NextDouble());

                if (IsFree(along, side))
                    Add(along, side);
            }

            _alive = new bool[Count];
            Array.Fill(_alive, true);

            for (int i = 0; i < Count; i++)
                _neighbours.Add(FindNeighbours(i));
        }

        private bool IsFree(float along, float side)
        {
            Vector2 point = ToWorld(along, side);

            if (point.magnitude < _config.FreeStart || (point - At(Length)).magnitude < _config.FreeDoor)
                return false;

            if (Mathf.Abs(side) < _config.ClearingHalfWidth && IsIn(_config.Clearings, along))
                return false;

            float spacing = IsIn(_config.TightZones, along) ? _config.TightSpacing : _config.Spacing;

            foreach (Vector2 other in _points)
            {
                if ((other - point).sqrMagnitude < spacing * spacing)
                    return false;
            }

            return true;
        }

        private static bool IsIn(Vector2[] zones, float along)
        {
            foreach (Vector2 zone in zones)
            {
                if (along >= zone.x && along <= zone.y)
                    return true;
            }

            return false;
        }

        private void Add(float along, float side)
        {
            _points.Add(ToWorld(along, side));
            _along.Add(along);
            _side.Add(side);
        }

        private Vector2 ToWorld(float along, float side) => At(along) + new Vector2(Direction.y, -Direction.x) * side;

        private int[] FindNeighbours(int index)
        {
            List<int> found = new();

            for (int i = 0; i < Count; i++)
            {
                if (i != index && (_points[i] - _points[index]).magnitude < _config.MaxHop)
                    found.Add(i);
            }

            return found.ToArray();
        }
    }
}
