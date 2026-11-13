using System;
using System.Collections.Generic;
using Zenject;

public class WorkerViewSpawner : IInitializable,IDisposable
{
    private readonly WorkerFleet _workerFleet;
    private readonly WorkerView.Factory _viewFactory;
    private readonly WorkerPresenter.Factory _presenterFactory;
    private readonly List<WorkerPresenter> _presenters =  new List<WorkerPresenter>();

    public WorkerViewSpawner(WorkerFleet workerFleet, WorkerView.Factory viewFactory,
        WorkerPresenter.Factory presenterFactory)
    {
        _workerFleet = workerFleet;
        _viewFactory = viewFactory;
        _presenterFactory = presenterFactory;
    }

    public void Initialize()
    {
        foreach (var worker in _workerFleet.Workers)
            CreateWorker(worker);
    }

    public void Dispose()
    {
        foreach (var presenter in _presenters)
            presenter.Dispose();
    }

    private void CreateWorker(WorkerModel worker)
    {
        WorkerView view = _viewFactory.Create();
        view.PlaceAt(worker.HomePosition);
        
        WorkerPresenter presenter = _presenterFactory.Create(worker,view);
        presenter.Initialize();
        _presenters.Add(presenter);
    }
}