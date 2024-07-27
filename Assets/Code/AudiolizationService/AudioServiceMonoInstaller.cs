using UnityEngine;
using Zenject;

namespace MyProject.ReactionBurst.AudiolizationService
{
    public class AudioServiceMonoInstaller : MonoInstaller
    {
        [SerializeField] private AudioService _audioService;
        [SerializeField] private AudioServiceConfig _audioServiceConfig;
        [SerializeField] private bool _useAddressables = true;
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<SoundPlayer>().AsSingle();
            Container.BindInterfacesAndSelfTo<MusicPlayer>().AsSingle();
            
            if(_useAddressables)
                Container.BindInterfacesAndSelfTo<AddressableAudioClipLoader>().AsSingle();
            else
                Container.BindInterfacesAndSelfTo<ResourcesAudioClipLoader>().AsSingle();
            
            Container.BindInterfacesAndSelfTo<AudioService>().FromInstance(_audioService).AsSingle();
            Container.Bind<AudioServiceConfig>().FromInstance(_audioServiceConfig).WhenInjectedInto<AudioService>();
        }
    }
}