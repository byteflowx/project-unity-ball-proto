using System;
using System.Threading;
using App.Scripts.Core.Common;
using App.Scripts.Core.Constants;
using App.Scripts.Engine.Addressable;
using App.Scripts.Engine.Time;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace App.Scripts.Engine.Audio
{
    public sealed class AudioService : IAudioService
    {
        private readonly IAddressableService _addressables;
        private readonly IUnityTimeService _time;
        private readonly AudioComponent _audio;

        private AudioLibrary _library;
        private string _currentMusicAddress;

        public AudioService(IAddressableService addressables, IUnityTimeService time, AudioComponent audio)
        {
            _addressables = addressables;
            _time = time;
            _audio = audio;
        }

        public async UniTask InitializeAsync(CancellationToken ct = default)
        {
            try
            {
                _library = await _addressables.Load<AudioLibrary>(AddressableConstants.AUDIO_LIBRARY, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Log.Error($"[AUDIO] library load failed at '{AddressableConstants.AUDIO_LIBRARY}', the session runs silent: {e}");
            }
        }

        public void PlaySound(string soundKey)
        {
            if (_library == null)
                return;

            if (!_library.TryGetSound(soundKey, out Sound sound) || sound == null || sound.Clip == null)
            {
                Log.Warning($"[AUDIO] no sound behind '{soundKey}' in the audio library");
                return;
            }

            AudioSource source = _audio.RentSoundSource();
            source.clip = sound.Clip;
            source.volume = Mathf.Clamp01(Vary(sound.Volume, sound.VolumeVariation));
            source.pitch = Vary(sound.Pitch, sound.PitchVariation);
            source.Play();
        }

        public void StopSound(string soundKey)
        {
            if (_library == null || !_library.TryGetSound(soundKey, out Sound sound))
                return;

            foreach (AudioSource source in _audio.SoundSources)
            {
                if (source.isPlaying && source.clip == sound.Clip)
                    source.Stop();
            }
        }

        public async UniTask PlayMusicAsync(string address, CancellationToken ct = default)
        {
            if (string.Equals(address, _currentMusicAddress, StringComparison.Ordinal))
                return;

            try
            {
                MusicTrack track = await _addressables.Load<MusicTrack>(address, ct);

                if (track == null || track.Clip == null)
                {
                    Log.Warning($"[AUDIO] track not found at '{address}'");
                    return;
                }

                _currentMusicAddress = address;
                Play(track);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                Log.Exception(e);
            }
        }

        private void Play(MusicTrack track)
        {
            AudioSource source = _audio.MusicSource;
            source.clip = track.Clip;
            source.pitch = track.Pitch;
            source.loop = track.Loop;
            source.volume = 0f;
            source.Play();

            FadeInAsync(source, track.Volume, track.FadeSeconds, _audio.destroyCancellationToken).Forget(Log.ExceptionUnlessCanceled);
        }

        private async UniTask FadeInAsync(AudioSource source, float volume, float seconds, CancellationToken ct)
        {
            float start = _time.RealtimeSinceStartup;

            while (seconds > 0f && _time.RealtimeSinceStartup - start < seconds)
            {
                source.volume = volume * (_time.RealtimeSinceStartup - start) / seconds;
                await UniTask.Yield(PlayerLoopTiming.Update, ct);
            }

            source.volume = volume;
        }

        private static float Vary(float value, float variation)
        {
            return variation > 0f ? value + UnityEngine.Random.Range(-variation, variation) : value;
        }
    }
}
