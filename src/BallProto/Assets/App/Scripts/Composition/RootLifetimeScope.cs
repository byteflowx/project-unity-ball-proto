using System;
using App.Scripts.AppFlow;
using App.Scripts.Composition.Bootstrap;
using App.Scripts.Composition.Flow;
using App.Scripts.Composition.StateMachine;
using App.Scripts.Composition.StateMachine.States;
using App.Scripts.Engine.Addressable;
using App.Scripts.Engine.Audio;
using App.Scripts.Engine.SceneLoader;
using App.Scripts.Engine.Time;
using App.Scripts.Game;
using App.Scripts.UI.Factory;
using App.Scripts.UI.Services.Loader;
using App.Scripts.UI.Services.UIService;
using App.Scripts.UI.Toolkit.Base;
using App.Scripts.UI.Toolkit.Screens;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace App.Scripts.Composition
{
    public sealed class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private PanelRenderer _panelRenderer;
        [SerializeField] private AudioComponent _audio;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(_panelRenderer);
            builder.RegisterComponent(_audio);

            RegisterEngine(builder);
            RegisterUi(builder);
            RegisterGame(builder);
            RegisterAppFlow(builder);

            builder.RegisterEntryPoint<AppBootstrap>();
        }

        private static void RegisterEngine(IContainerBuilder builder)
        {
            builder.Register<AddressableService>(Lifetime.Singleton).As<IAddressableService>();
            builder.Register<UnityTimeService>(Lifetime.Singleton).As<IUnityTimeService>();
            builder.Register<AudioService>(Lifetime.Singleton).As<IAudioService>();
            builder.Register<SceneLoaderService>(Lifetime.Singleton).As<ISceneLoaderService>();
        }

        private static void RegisterUi(IContainerBuilder builder)
        {
            builder.RegisterFactory<Type, UIView>(
                container => type => (UIView)container.Resolve(type),
                Lifetime.Singleton);

            builder.Register<UIViewFactory>(Lifetime.Singleton).As<IUIViewFactory>();
            builder.Register<LoaderService>(Lifetime.Singleton).As<ILoaderService>();
            builder.Register<UIService>(Lifetime.Singleton).As<IUIService>();
            builder.Register<GameScreen>(Lifetime.Transient);
        }

        private static void RegisterGame(IContainerBuilder builder)
        {
            builder.Register<LevelService>(Lifetime.Singleton).As<ILevelService>();
        }

        private static void RegisterAppFlow(IContainerBuilder builder)
        {
            builder.Register<AppStateMachine>(Lifetime.Singleton).AsSelf();
            builder.Register<AppFlowService>(Lifetime.Singleton).As<IAppFlowService>();

            builder.Register<LoadAudioState>(Lifetime.Singleton).As<IState>();
            builder.Register<CreateUiState>(Lifetime.Singleton).As<IState>();
            builder.Register<GameState>(Lifetime.Singleton).As<IState>();
        }
    }
}
