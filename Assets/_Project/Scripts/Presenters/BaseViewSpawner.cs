using System;
using System.Collections.Generic;
using R3;
using Zenject;

public class BaseViewSpawner : IInitializable, IDisposable
{
    private readonly BaseCollection _baseCollection;
    private readonly BasePresenter.Factory _presenterFactory;
    private readonly BaseView.Factory _viewFactory;
    private readonly List<BasePresenter> _presenters = new List<BasePresenter>();
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public BaseViewSpawner(BaseCollection baseCollection, BaseView.Factory viewFactory,
        BasePresenter.Factory basePresenterFactory)
    {
        _baseCollection = baseCollection;
        _viewFactory = viewFactory;
        _presenterFactory = basePresenterFactory;
    }

    public void Initialize()
    {
        _baseCollection.Added.Subscribe(CreateBaseView).AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
        
        foreach (var presenter in _presenters)
            presenter.Dispose();
    }

    private void CreateBaseView(BaseModel baseModel)
    {
        BaseView view = _viewFactory.Create();
        BasePresenter presenter = _presenterFactory.Create(baseModel,view);
        
        presenter.Initialize();
        _presenters.Add(presenter);
    }
}