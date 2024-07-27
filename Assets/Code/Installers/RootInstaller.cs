
using Code.Infrastructure.AssetsProvider;
using MyProject.Config;
using MyProject.ReactionBurst.AudiolizationService;
using MyProject.ReactionBurst.SaveLoad;
using MyProject.ReactionBurst.UI;
using UnityEngine;
using Zenject;

namespace MyProject.ReactionBurst.Installers
{
    [CreateAssetMenu(menuName = "Installers/" + nameof(RootInstaller), fileName = nameof(RootInstaller))]
    public sealed class RootInstaller : ScriptableObjectInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<AddressablesAssetsProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<ConfigsProvider>().AsSingle();
            Container.BindInterfacesAndSelfTo<UIBuilder>().AsSingle();
            Container.BindInterfacesAndSelfTo<UI.UIService>().AsSingle();
            Container.BindInterfacesAndSelfTo<SaveLoadService>().AsSingle();
        }
    }
}
