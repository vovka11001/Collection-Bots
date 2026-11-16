using System;
using R3;
using Zenject;

public class ResourceCounterPresenter : IInitializable, IDisposable
{
    private readonly IReadOnlyResourceStorage _resourceStorage;
    private readonly ResourceCounterView _resourceCounterView;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public ResourceCounterPresenter(IReadOnlyResourceStorage resourceStorage, ResourceCounterView resourceCounterView)
    {
        _resourceStorage = resourceStorage;
        _resourceCounterView = resourceCounterView;
    }

    public void Initialize()
    {
        _resourceStorage.Count.Subscribe(_resourceCounterView.SetCount).AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }
}