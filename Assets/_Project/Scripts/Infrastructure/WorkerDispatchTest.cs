using System;
using System.Linq;
using R3;
using Zenject;

public class WorkerDispatchTest : IInitializable, IDisposable
{
    private const float DispatchIntervalSeconds = 1f;

    private readonly WorkerFleet _workerFleet;
    private readonly ResourceRegistry _resourceRegistry;
    private readonly ResourceField _resourceField;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public WorkerDispatchTest(WorkerFleet workerFleet, ResourceRegistry resourceRegistry, ResourceField resourceField)
    {
        _workerFleet = workerFleet;
        _resourceRegistry = resourceRegistry;
        _resourceField = resourceField;
    }

    public void Initialize()
    {
        Observable.Interval(TimeSpan.FromSeconds(DispatchIntervalSeconds)).Subscribe(_ => AssignIdleWorkers())
            .AddTo(_subscriptions);
        Observable.Merge(_workerFleet.Workers.Select(worker => worker.Delivered)).Subscribe(AcceptDelivery)
            .AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }

    private void AssignIdleWorkers()
    {
        foreach (var worker in _workerFleet.Workers.Where(worker => worker.IsIdle))
        {
            if(_resourceRegistry.TryTakeFree(out ResourceModel resource) == false)
                return;
            
            worker.Assign(resource);
        }
    }

    private void AcceptDelivery(ResourceModel resource)
    {
        _resourceRegistry.Remove(resource);
        _resourceField.Remove(resource);
    }
}