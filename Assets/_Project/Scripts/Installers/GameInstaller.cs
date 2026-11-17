using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    private const string ResourcesGroupName = "Resources";
    private const string WorkerGroupName = "Workers";
    private const string BasesGroupName = "Bases";
    private const int LateInitializationOrder = 100;
    
    [SerializeField] private GameConfig _gameConfig;
    [SerializeField] private ResourceView _resourceViewPrefab;
    [SerializeField] private WorkerView _workerViewPrefab;
    [SerializeField] private BaseView _baseViewPrefab;
    
    public override void InstallBindings()
    {
        BindResources();
        BindConfigs();
        BindBases();
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
            .UnderTransformGroup(ResourcesGroupName);
        
        Container.BindInterfacesTo<ResourceViewSpawner>().AsSingle();
        Container.BindInterfacesAndSelfTo<ResourceRegistry>().AsSingle();
        Container.BindInterfacesTo<BaseScanner>().AsSingle();
    }

    private void BindBases()
    {
        Container.BindFactory<Vector3, BaseModel, BaseModel.Factory>();
        Container.BindInterfacesAndSelfTo<BaseCollection>().AsSingle();
        Container.BindExecutionOrder<BaseCollection>(LateInitializationOrder);

        Container.BindFactory<BaseView, BaseView.Factory>().FromComponentInNewPrefab(_baseViewPrefab)
            .UnderTransformGroup(BasesGroupName);
        
        Container.BindFactory<IBasePresentationModel,BaseView,BasePresenter,BasePresenter.Factory>();
        
        Container.BindInterfacesTo<BaseViewSpawner>().AsSingle();
    }

    private void BindWorkers()
    {
        Container.BindInterfacesAndSelfTo<WorkerCollection>().AsSingle();
        Container.BindFactory<WorkerView, WorkerView.Factory>().FromComponentInNewPrefab(_workerViewPrefab)
            .UnderTransformGroup(WorkerGroupName);
        
        Container.BindFactory<IWorkerPresentationModel, WorkerView, WorkerPresenter, WorkerPresenter.Factory>();
        Container.BindInterfacesTo<WorkerViewSpawner>().AsSingle();
    }
}