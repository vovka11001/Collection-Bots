using Zenject;
using UnityEngine;
using UnityEngine.AI;

public class GameInstaller : MonoInstaller
{
    private const string ResourcesGroupNmae = "Resources";
    
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private ResourceView _resourceViewPrefab;
    
    public override void InstallBindings()
    {
        BindResources();
        BindConfigs();
        BindBase();
    }

    private void BindConfigs()
    {
        Container.BindInstance(_gameConfig);
    }

    private void BindResources()
    {
        Container.BindInterfacesAndSelfTo<ResourceField>().AsSingle();
        Container.Bind<IRandomPointProvider>().To<NavMeshRandomPointProvider>().AsSingle();
        Container.BindInterfacesTo<ResourceSpawner>().AsSingle();

        Container.BindFactory<ResourceView, ResourceView.Factory>()
            .FromComponentInNewPrefab(_resourceViewPrefab)
            .UnderTransformGroup(ResourcesGroupNmae);
        
        Container.BindInterfacesTo<ResourceViewSpawner>().AsSingle();
    }

    private void BindBase()
    {
        Container.BindInterfacesAndSelfTo<ResourceRegistry>().AsSingle();
        Container.BindInterfacesTo<BaseScanner>().AsSingle();
        Container.BindInterfacesTo<ResourceRegistryDebugLogger>().AsSingle();
    }
}