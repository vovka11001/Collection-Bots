using System.Linq;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    private const string ResourcesGroupNmae = "Resources";
    private const string WorkerGroupName = "Workers";
    
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private ResourceView _resourceViewPrefab;
    [SerializeField] private WorkerView _workerViewPrefab;
    [SerializeField] private Transform _dropPoint;
    [SerializeField] private Transform[] _workerSpawnPoints;
    
    public override void InstallBindings()
    {
        BindResources();
        BindConfigs();
        BindBase();
        BindWorkers();
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
        Vector3[] workerSpawnPositions = _workerSpawnPoints.Select(spawnPoint => spawnPoint.position).ToArray();
        
        Container.BindInterfacesAndSelfTo<ResourceRegistry>().AsSingle();
        Container.BindInterfacesTo<BaseScanner>().AsSingle();
        Container.BindInstance(new BaseLocation(_dropPoint.position, workerSpawnPositions));
    }

    private void BindWorkers()
    {
        Container.BindInterfacesAndSelfTo<WorkerFleet>().AsSingle();
        Container.BindFactory<WorkerView, WorkerView.Factory>().FromComponentInNewPrefab(_workerViewPrefab)
            .UnderTransformGroup(WorkerGroupName);
        Container.BindFactory<WorkerModel, WorkerView, WorkerPresenter, WorkerPresenter.Factory>();
        Container.BindInterfacesTo<WorkerViewSpawner>().AsSingle();
        Container.BindInterfacesTo<WorkerDispatchTest>().AsSingle();
    }
}