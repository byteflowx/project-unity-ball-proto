using System.Collections.Generic;
using UnityEngine;

namespace App.Scripts.Engine.Audio
{
    [CreateAssetMenu(fileName = "AudioLibrary", menuName = "App/Audio/Audio Library")]
    public sealed class AudioLibrary : ScriptableObject
    {
        [SerializeField] private Dictionary<string, Sound> _sounds = new();

        public bool TryGetSound(string key, out Sound sound) => _sounds.TryGetValue(key, out sound);
    }
}
