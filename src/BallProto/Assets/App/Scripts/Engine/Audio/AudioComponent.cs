using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Engine.Audio
{
    public sealed class AudioComponent : MonoBehaviour
    {
        [SerializeField] private AudioSource _soundTemplate;
        [SerializeField] private List<AudioSource> _soundSources;
        [SerializeField] private AudioSource _musicSource;

        public IReadOnlyList<AudioSource> SoundSources => _soundSources;
        public AudioSource MusicSource => _musicSource;

        public AudioSource RentSoundSource()
        {
            foreach (AudioSource source in _soundSources)
            {
                if (!source.isPlaying)
                    return source;
            }

            AudioSource created = Instantiate(_soundTemplate, transform);
            _soundSources.Add(created);

            return created;
        }
    }
}
