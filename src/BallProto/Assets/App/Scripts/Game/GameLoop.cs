using System;
using System.Collections.Generic;
using App.Scripts.Core.Common;
using App.Scripts.Core.Constants;
using App.Scripts.Engine.Audio;
using App.Scripts.Engine.Time;
using App.Scripts.UI.Services.UIService;
using App.Scripts.UI.Toolkit.Screens;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace App.Scripts.Game
{
    public sealed class GameLoop : MonoBehaviour
    {
        private const float INTRO_HOLD_SECONDS = 0.5f;
        private const float INTRO_SECONDS = 1.7f;
        private const float POP_INTERVAL = 0.045f;
        private const float RADIUS_SHARPNESS = 22f;
        private const float MIN_STEP = 0.05f;
        private const float WIN_RESULT_DELAY = 1.6f;
        private const float LOSE_RESULT_DELAY = 1.1f;
        private const int DUST_PER_LANDING = 10;
        private const string WIN_TITLE = "Victory!";
        private const string OVERCHARGE_TITLE = "Overcharged: the ball burst";
        private const string OUT_OF_SIZE_TITLE = "Out of size: the path is still blocked";

        [SerializeField] private LevelConfig _config;
        [SerializeField] private Ball _ball;
        [SerializeField] private Shot _shotPrefab;
        [SerializeField] private Door _door;
        [SerializeField] private Field _field;
        [SerializeField] private Transform _path;
        [SerializeField] private ParticleSystem _dust;
        [SerializeField] private ParticleSystem _burst;
        [SerializeField] private ParticleSystem _confetti;
        [SerializeField] private CinemachineCamera _introCamera;
        [SerializeField] private CinemachineCamera _winCamera;
        [SerializeField] private CinemachineImpulseSource _impulse;

        private readonly List<Shot> _flying = new();
        private IAudioService _audio;
        private IUnityTimeService _time;
        private IUIService _ui;
        private Level _level;
        private Shot _charging;
        private float _mass;
        private float _shotMass;
        private float _along;
        private float _readyAt;
        private float _blastEndsAt;
        private float _nextPopAt;
        private bool _isDoorOpen;
        private bool _isOver;

        [Inject]
        public void Construct(IAudioService audio, IUnityTimeService time, IUIService ui)
        {
            _audio = audio;
            _time = time;
            _ui = ui;
        }

        private float BallRadius => Mathf.Sqrt(_mass);
        private bool CanAffordShot => _mass - _config.MinShotMass > _config.CriticalMass;
        private GameScreen Screen => _ui.CurrentScreen.CurrentValue as GameScreen;

        private void Start()
        {
            _level = new Level(_config);
            _field.Build(_level, _config.ObstacleRadius);
            _field.Exploded += OnObstacleExploded;
            _ball.Landed += OnBallLanded;
            PlaceDoorAndPath();

            _mass = _config.StartMass;
            _ball.Radius = BallRadius;
            _readyAt = _blastEndsAt = _time.InGameTime + INTRO_SECONDS;

            PlayIntroAsync().Forget(Log.ExceptionUnlessCanceled);
        }

        private void PlaceDoorAndPath()
        {
            Vector2 door = _config.DoorPosition;
            _door.transform.position = new Vector3(door.x, 0f, door.y);
            _path.SetPositionAndRotation(
                new Vector3(door.x * 0.5f, _path.position.y, door.y * 0.5f),
                Quaternion.Euler(90f, Mathf.Atan2(door.x, door.y) * Mathf.Rad2Deg, 0f));
            _path.localScale = new Vector3(1f, _level.Length, 1f);
        }

        private void Update()
        {
            float deltaTime = _time.DeltaTime;

            _ball.Radius = Mathf.Lerp(_ball.Radius, BallRadius, 1f - Mathf.Exp(-RADIUS_SHARPNESS * deltaTime));
            _path.localScale = new Vector3(_config.LaneHalfWidth(_ball.Radius) * 2f, _path.localScale.y, 1f);

            _ball.Tick(deltaTime);
            _door.Tick(deltaTime);
            _field.Tick(deltaTime);
            FlyShots(deltaTime);

            if (!_isDoorOpen && Vector3.Distance(_ball.transform.position, _door.transform.position) <= _config.DoorOpenDistance)
                OpenDoor();

            if (_isOver)
                return;

            bool isHeld = Pointer.current != null && Pointer.current.press.isPressed;

            if (_charging != null)
            {
                if (isHeld)
                    Charge(deltaTime);
                else
                    Release();
            }
            else if (isHeld && _time.InGameTime >= _readyAt && CanAffordShot && _level.FirstBlocker(_along, BallRadius) >= 0)
            {
                BeginCharge();
            }
            else if (!_ball.IsHopping && _flying.Count == 0 && _time.InGameTime >= _blastEndsAt)
            {
                Advance();
            }
        }

        private async UniTask PlayIntroAsync()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(INTRO_HOLD_SECONDS), cancellationToken: destroyCancellationToken);
            _introCamera.gameObject.SetActive(false);
        }

        private void BeginCharge()
        {
            Screen?.HideHint();

            _shotMass = _config.MinShotMass;
            _mass -= _shotMass;
            _charging = Instantiate(_shotPrefab);
            _ball.Kick(2f);
            _audio.PlaySound(SoundKeys.GAME_CHARGE);

            PlaceChargingShot();
        }

        private void Charge(float deltaTime)
        {
            float flow = _config.ChargePerSecond * deltaTime;
            _shotMass += flow;
            _mass -= flow;

            if (_mass <= _config.CriticalMass)
                Overcharge();
            else
                PlaceChargingShot();
        }

        private void PlaceChargingShot()
        {
            float radius = Mathf.Sqrt(_shotMass);
            Vector3 forward = new(_level.Direction.x, 0f, _level.Direction.y);
            _charging.Grow(_ball.Centre + forward * (_ball.Radius + radius), radius);

            int blocker = _level.FirstBlocker(_along, BallRadius);

            if (blocker >= 0)
                _field.Preview(_level.FirstHit(_charging.Flat, _level.Point(blocker), radius), _config.Power(_shotMass));
        }

        private void Release()
        {
            _charging.Launch();
            _flying.Add(_charging);
            _charging = null;

            _audio.StopSound(SoundKeys.GAME_CHARGE);
            _field.ClearPreview();
            _ball.Kick(-4.5f);
            _audio.PlaySound(SoundKeys.GAME_SHOOT);
            _impulse.GenerateImpulseWithForce(0.1f + Mathf.Sqrt(_shotMass) * 0.2f);
        }

        private void FlyShots(float deltaTime)
        {
            int blocker = _level.FirstBlocker(_along, BallRadius);

            for (int i = _flying.Count - 1; i >= 0; i--)
            {
                Shot shot = _flying[i];

                if (blocker >= 0)
                    shot.Fly(_level.Point(blocker), deltaTime);

                int hit = _level.Touching(shot.Flat, shot.Radius);

                if (hit < 0 && blocker >= 0)
                    continue;

                if (hit >= 0)
                {
                    _blastEndsAt = Mathf.Max(_blastEndsAt, _time.InGameTime + _field.Blast(hit, _config.Power(shot.Radius * shot.Radius)));
                    _impulse.GenerateImpulseWithForce(0.25f + shot.Radius * 0.75f);
                }

                _flying.RemoveAt(i);
                Destroy(shot.gameObject);
            }
        }

        private void Advance()
        {
            float stop = _level.StopDistance(_along, BallRadius);

            if (stop - _along > MIN_STEP)
            {
                float hopLength = 1.1f + BallRadius * 1.5f;
                _along = Mathf.Min(stop, _along + (stop - _along) / Mathf.Ceil((stop - _along) / hopLength));

                Vector2 next = _level.At(_along);
                _ball.Hop(new Vector3(next.x, 0f, next.y), 0.75f + BallRadius * 0.8f);
            }
            else if (_level.FirstBlocker(_along, BallRadius) < 0)
            {
                Win();
            }
            else if (!CanAffordShot)
            {
                Lose(OUT_OF_SIZE_TITLE);
            }
        }

        private void OpenDoor()
        {
            _isDoorOpen = true;
            _door.Open();
            _audio.PlaySound(SoundKeys.GAME_DOOR);
        }

        private void OnBallLanded()
        {
            _dust.Emit(new ParticleSystem.EmitParams { position = _ball.transform.position, applyShapeToPosition = true }, DUST_PER_LANDING);
            _audio.PlaySound(SoundKeys.GAME_HOP);
        }

        private void OnObstacleExploded()
        {
            if (_time.InGameTime < _nextPopAt)
                return;

            _nextPopAt = _time.InGameTime + POP_INTERVAL;
            _audio.PlaySound(SoundKeys.GAME_POP);
        }

        private void Overcharge()
        {
            _audio.StopSound(SoundKeys.GAME_CHARGE);
            _field.ClearPreview();
            Destroy(_charging.gameObject);
            _charging = null;

            _burst.transform.position = _ball.Centre;
            _burst.Play();
            _ball.gameObject.SetActive(false);
            _impulse.GenerateImpulseWithForce(1.6f);

            Lose(OVERCHARGE_TITLE);
        }

        private void Win()
        {
            _isOver = true;
            _mass = 0f;
            _winCamera.gameObject.SetActive(true);
            _confetti.Play();
            _audio.PlaySound(SoundKeys.GAME_WIN);

            ShowResultAsync(WIN_TITLE, WIN_RESULT_DELAY).Forget(Log.ExceptionUnlessCanceled);
        }

        private void Lose(string reason)
        {
            _isOver = true;
            _audio.PlaySound(SoundKeys.GAME_LOSE);

            ShowResultAsync(reason, LOSE_RESULT_DELAY).Forget(Log.ExceptionUnlessCanceled);
        }

        private async UniTask ShowResultAsync(string title, float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: destroyCancellationToken);
            Screen?.ShowResult(title);
        }
    }
}
