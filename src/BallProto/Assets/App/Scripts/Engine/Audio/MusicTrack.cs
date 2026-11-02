using UnityEngine;

namespace App.Scripts.Engine.Audio
{
    [CreateAssetMenu(fileName = "MusicTrack", menuName = "App/Audio/Music Track")]
    public sealed class MusicTrack : ScriptableObject
    {
        [SerializeField] private AudioClip _clip;
        [SerializeField] [Range(0f, 1f)] private float _volume = 1f;
        [SerializeField] [Range(-3f, 3f)] private float _pitch = 1f;
        [SerializeField] private bool _loop = true;
        [SerializeField] [Min(0f)] private float _fadeSeconds = 1f;

        public AudioClip Clip => _clip;
        public float Volume => _volume;
        public float Pitch => _pitch;
        public bool Loop => _loop;
        public float FadeSeconds => _fadeSeconds;
    }
}
