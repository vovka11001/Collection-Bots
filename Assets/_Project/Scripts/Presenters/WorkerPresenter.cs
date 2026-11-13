using System;
using R3;
using Zenject;

public class WorkerPresenter : IDisposable
{
    private readonly WorkerModel _model;
    private readonly WorkerView _view;
    private readonly IResourceViewProvider _resourceViewProvider;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public WorkerPresenter(WorkerModel model, WorkerView view, IResourceViewProvider resourceViewProvider)
    {
        _model = model;
        _view = view;
        _resourceViewProvider = resourceViewProvider;
    }
    
    public void Initialize()
    {
        _model.MoveRequested.Subscribe(_view.MoveTo).AddTo(_subscriptions);
        _view.Arrived.Subscribe(_ => _model.ReportArrived()).AddTo(_subscriptions);
        _model.CarriedResource.Where(resource => resource != null).Subscribe(AttachResourceView).AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }

    private void AttachResourceView(ResourceModel resource)
    {
        if (_resourceViewProvider.TryGetView(resource, out var view))
            _view.Hold(view);
    }
    
    public class Factory : PlaceholderFactory<WorkerModel, WorkerView, WorkerPresenter>{}
}