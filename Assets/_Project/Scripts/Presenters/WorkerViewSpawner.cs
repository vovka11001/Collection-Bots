using System;
using System.Collections.Generic;
using R3;
using Zenject;

public class WorkerViewSpawner : IInitializable,IDisposable
{
    private readonly WorkerCollection _workerCollection;
    private readonly WorkerView.Factory _viewFactory;
    private readonly WorkerPresenter.Factory _presenterFactory;
    private readonly List<WorkerPresenter> _presenters = new List<WorkerPresenter>();
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public WorkerViewSpawner(WorkerCollection workerCollection, WorkerView.Factory viewFactory,
        WorkerPresenter.Factory presenterFactory)
    {
        _workerCollection = workerCollection;
        _viewFactory = viewFactory;
        _presenterFactory = presenterFactory;
    }

    public void Initialize()
    {
        _workerCollection.Created.Subscribe(CreateWorkerView).AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();

        foreach (var presenter in _presenters)
        {
            presenter.Dispose();
        }
    }

    private void CreateWorkerView(WorkerModel worker)
    {
        WorkerView view = _viewFactory.Create();
        view.PlaceAt(worker.SpawnPosition);
        
        WorkerPresenter presenter = _presenterFactory.Create(worker, view);
        presenter.Initialize();
        _presenters.Add(presenter);
    }
}