using System;
using R3;
using UnityEngine;
using Zenject;

public class ResourceSpawner : IInitializable, IDisposable
{
    private readonly GameConfig _config;
    private readonly ResourceField _resourceField;
    private readonly IRandomPointProvider _randomPointProvider;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public ResourceSpawner(GameConfig config, ResourceField resourceField, IRandomPointProvider randomPointProvider)
    {
        _config = config;
        _resourceField = resourceField;
        _randomPointProvider = randomPointProvider;
    }

    public void Initialize()
    {
        _resourceField.Count
            .Select(HasFreeSpace)
            .DistinctUntilChanged()
            .Select(CreateSpawnTicks)
            .Switch()
            .Subscribe(_ => TrySpawn())
            .AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }

    private bool HasFreeSpace(int resourceCount)
    {
        return resourceCount < _config.MaxResourcesOnField;
    }

    private Observable<Unit> CreateSpawnTicks(bool hasFreeSpace)
    {
        if (hasFreeSpace)
            return Observable.Interval(TimeSpan.FromSeconds(_config.ResourceSpawnIntervalSeconds));
        
        return Observable.Empty<Unit>();
    }

    private void TrySpawn()
    {
        if(_randomPointProvider.TryGetRandomPoint(out Vector3 spawnPoint))
            _resourceField.Add(new ResourceModel(spawnPoint));
    }
}