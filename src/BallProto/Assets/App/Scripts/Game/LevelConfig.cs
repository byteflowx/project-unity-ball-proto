using UnityEngine;

namespace App.Scripts.Game
{
    [CreateAssetMenu(fileName = "LevelConfig", menuName = "App/Game/Level Config")]
    public sealed class LevelConfig : ScriptableObject
    {
        [field: Header("Ball: size is mass, radius = sqrt(mass)")]
        [field: SerializeField] public float StartRadius { get; private set; } = 1.2f;
        [field: SerializeField] public float CriticalRadius { get; private set; } = 0.5f;
        [field: SerializeField] public float ChargePerSecond { get; private set; } = 0.32f;
        [field: SerializeField] public float MinShotMass { get; private set; } = 0.06f;
        [field: SerializeField] public float PathMargin { get; private set; } = 0.1f;
        [field: SerializeField] public float Standoff { get; private set; } = 1.2f;

        [field: Header("Shot and infection")]
        [field: Tooltip("Power = Scale * shotMass ^ Exponent. Below 1 the exponent makes a very big shot wasteful; a tiny one only takes out its target.")]
        [field: SerializeField] public float PowerScale { get; private set; } = 11f;
        [field: SerializeField] public float PowerExponent { get; private set; } = 0.75f;
        [field: Tooltip("A hop of this length costs as many power units as metres; longer hops cost quadratically more.")]
        [field: SerializeField] public float HopReference { get; private set; } = 1.2f;
        [field: SerializeField] public float MaxHop { get; private set; } = 3f;

        [field: Header("Field: the ball starts at the world origin")]
        [field: SerializeField] public Vector2 DoorPosition { get; private set; } = new(8f, 24f);
        [field: SerializeField] public int Seed { get; private set; } = 7;
        [field: SerializeField] public float ObstacleRadius { get; private set; } = 0.4f;
        [field: SerializeField] public float Spacing { get; private set; } = 1.15f;
        [field: SerializeField] public float TightSpacing { get; private set; } = 0.95f;
        [field: SerializeField] public float HalfWidth { get; private set; } = 11f;
        [field: SerializeField] public float FreeStart { get; private set; } = 4f;
        [field: SerializeField] public float FreeDoor { get; private set; } = 4.5f;
        [field: SerializeField] public float PadBehindDoor { get; private set; } = 14f;
        [field: Tooltip("Stretches of the path (metres) packed tighter, where infection travels further.")]
        [field: SerializeField] public Vector2[] TightZones { get; private set; }
        [field: Tooltip("Stretches of the path (metres) emptied around the lane.")]
        [field: SerializeField] public Vector2[] Clearings { get; private set; }
        [field: SerializeField] public float ClearingHalfWidth { get; private set; } = 3f;
        [field: Tooltip("Lone obstacles: x is metres along the path, y is the side offset.")]
        [field: SerializeField] public Vector2[] Loners { get; private set; }

        [field: Header("Rules")]
        [field: SerializeField] public float DoorOpenDistance { get; private set; } = 5f;

        public float StartMass => StartRadius * StartRadius;
        public float CriticalMass => CriticalRadius * CriticalRadius;

        public float LaneHalfWidth(float ballRadius) => ballRadius + PathMargin;
        public float Power(float shotMass) => PowerScale * Mathf.Pow(shotMass, PowerExponent);
    }
}
