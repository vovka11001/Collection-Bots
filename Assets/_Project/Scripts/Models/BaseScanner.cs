using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;
using Zenject;

public class BaseScanner : IInitializable, IDisposable
{
    private readonly GameConfig _config;
    private readonly IReadOnlyResourceField _resourceField;
    private readonly BaseCollection _baseCollection;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public BaseScanner(GameConfig config, BaseCollection baseCollection, IReadOnlyResourceField resourceField)
    {
        _config = config;
        _resourceField = resourceField;
        _baseCollection = baseCollection;
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
        return _resourceField.Resources.Where(resource => IsRegisteredAnyWhere(resource) == false).ToList();
    }

    private bool IsRegisteredAnyWhere(ResourceModel resourceModel)
    {
        return _baseCollection.Bases.Any(baseModel => baseModel.KnowsResource(resourceModel));
    }

    private void RegisterAll(List<ResourceModel> foundResources)
    {
        foreach (var resourceModel in foundResources)
        {
            BaseModel nearestBase = FindNearestBase(resourceModel.Position);
            
            if (nearestBase != null)
                nearestBase.RegisterResource(resourceModel);
        }
    }

    private BaseModel FindNearestBase(Vector3 position)
    {
        return _baseCollection.Bases.OrderBy(basemodel => (basemodel.Position - position).sqrMagnitude).FirstOrDefault();
    }
}