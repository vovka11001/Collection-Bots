using System;
using R3;
using Zenject;

public class BasePresenter : IDisposable
{
    private readonly IBasePresentationModel _model;
    private readonly BaseView _view;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    
    public BasePresenter(IBasePresentationModel model, BaseView view)
    {
        _model = model;
        _view = view;
    }

    public void Initialize()
    {
        _view.SetPosition(_model.Position);
        
        _model.Storage.Count.Subscribe(_view.SetResourceCount).AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }
    
    public class Factory : PlaceholderFactory<IBasePresentationModel, BaseView, BasePresenter> {}
}