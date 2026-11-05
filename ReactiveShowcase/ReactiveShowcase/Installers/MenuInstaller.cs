using Zenject;

namespace ReactiveShowcase.Installers;

// ReSharper disable once ClassNeverInstantiated.Global
public class MenuInstaller : Installer {
    public override void InstallBindings() {
        Container.Bind<MenuButtonManager>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<ReactiveFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
        Container.Bind<HubViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<ReactiveViewController>().FromNewComponentAsViewController().AsSingle();
        Container.Bind<ExampleViewController>().FromNewComponentAsViewController().AsSingle();
    }
}
