using System;
using System.Linq;
using R3;
using Zenject;

public class BaseDispatcher : IInitializable, IDisposable
{
    private const int EmptyCount = 0;
    
    private readonly WorkerFleet _workerFleet;
    private readonly ResourceRegistry _resourceRegistry;
    private readonly ResourceField _resourceField;
    private readonly ResourceStorage _resourceStorage;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public BaseDispatcher(WorkerFleet workerFleet, ResourceRegistry resourceRegistry, ResourceField resourceField,
        ResourceStorage resourceStorage)
    {
        _workerFleet = workerFleet;
        _resourceRegistry = resourceRegistry;
        _resourceField = resourceField;
        _resourceStorage = resourceStorage;
    }

    public void Initialize()
    {
        Observable.CombineLatest(ObserveIdleWorkerCount(),_resourceRegistry.FreeCount,CanDispatch)
            .DistinctUntilChanged().Where(canDispatch => canDispatch).Subscribe(_ => DispatchAll())
            .AddTo(_subscriptions);
        
        Observable.Merge(_workerFleet.Workers.Select(worker => worker.Delivered)).Subscribe(AcceptDelivery)
            .AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }

    private Observable<int> ObserveIdleWorkerCount()
    {
        return Observable.CombineLatest(_workerFleet.Workers.Select(worker => worker.State.AsObservable()))
            .Select(states => states.Count(state => state == WorkerState.Idle));
    }

    private bool CanDispatch(int idleWorkerCount, int freeResourceCount)
    {
        return idleWorkerCount > EmptyCount && freeResourceCount > EmptyCount;
    }

    private void DispatchAll()
    {
        WorkerModel idleWorker = FindIdleWorker();

        while (idleWorker != null && _resourceRegistry.TryTakeFree(out var resource))
        {
            idleWorker.Assign(resource);
            idleWorker = FindIdleWorker();
        }
    }

    private WorkerModel FindIdleWorker()
    {
        return _workerFleet.Workers.FirstOrDefault(worker => worker.IsIdle);
    }

    private void AcceptDelivery(ResourceModel resource)
    {
        _resourceRegistry.Remove(resource);
        _resourceField.Remove(resource);
        _resourceStorage.AddResource();
    }
}