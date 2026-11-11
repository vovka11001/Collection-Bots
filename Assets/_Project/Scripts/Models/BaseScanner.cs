using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using Zenject;

public class BaseScanner : IInitializable, IDisposable
{
    private readonly GameConfig _config;
    private readonly ResourceField _resourceField;
    private readonly ResourceRegistry _resourceRegistry;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public BaseScanner(GameConfig config, ResourceRegistry resourceRegistry, ResourceField resourceField)
    {
        _config = config;
        _resourceField = resourceField;
        _resourceRegistry = resourceRegistry;
    }

    public void Initialize()
    {
        Observable.Interval(TimeSpan.FromSeconds(_config.ScanIntervalSeconds))
            .Select(_ => FindUnregisteredResources())
            .Where(foundResources => foundResources.Any())
            .Subscribe(RegisterAll)
            .AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }

    private List<ResourceModel> FindUnregisteredResources()
    {
        return _resourceField.Resources.Where(resource => _resourceRegistry.Contains(resource) == false).ToList();
    }

    private void RegisterAll(List<ResourceModel> foundResources)
    {
        foreach (var resourceModel in foundResources)
        {
            _resourceRegistry.Register(resourceModel);
        }
    }
}