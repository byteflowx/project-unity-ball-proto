using UnityEngine;

namespace App.Scripts.Engine.Audio
{
    [CreateAssetMenu(fileName = "Sound", menuName = "App/Audio/Sound")]
    public sealed class Sound : ScriptableObject
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] [Range(0f, 1f)] private float _volume = 1f;
        [SerializeField] [Range(-3f, 3f)] private float _pitch = 1f;
        [SerializeField] [Range(0f, 1f)] private float _volumeVariation;
        [SerializeField] [Range(0f, 1f)] private float _pitchVariation;

        public AudioClip Clip => _clip;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public float VolumeVariation => _volumeVariation;
        public float PitchVariation => _pitchVariation;
    }
}
