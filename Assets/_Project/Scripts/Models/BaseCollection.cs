using System;
using System.Collections.Generic;
using R3;
using UnityEngine;
using Zenject;

public class BaseCollection : IInitializable,IDisposable
{
    private readonly GameConfig _config;
    private readonly BaseModel.Factory _baseFactory;
    private readonly List<BaseModel> _bases = new List<BaseModel>();
    private readonly Subject<BaseModel> _added = new Subject<BaseModel>();
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public BaseCollection(GameConfig config, BaseModel.Factory baseFactory)
    {
        _config = config;
        _baseFactory = baseFactory;
    }
    
    public IReadOnlyList<BaseModel> Bases => _bases;
    public Observable<BaseModel> Added => _added;

    public void Initialize()
    {
        BaseModel startBase = CreateBase(_config.StartBasePosition);
        
        while(startBase.WorkerCount.CurrentValue < _config.InitialWorkerCount)
            startBase.CreateWorker();
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
        
        foreach (var baseModel in _bases)
            baseModel.Dispose();
        
        _added.Dispose();
    }
    
    private BaseModel CreateBase(Vector3 position)
    {
        BaseModel baseModel = _baseFactory.Create(position);
        
        baseModel.Initialize();
        _bases.Add(baseModel);
        
        baseModel.ExpansionRequest.Subscribe(BuildExpansionBase).AddTo(_subscriptions);
        
        _added.OnNext(baseModel);
        
        return baseModel;
    }

    private void BuildExpansionBase(BaseExpansionRequest request)
    {
        BaseModel newBase = CreateBase(request.Position);
        newBase.AddWorker(request.Builder);
    }
}