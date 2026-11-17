using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

public class WorkerCollection : IDisposable
{
    private readonly List<WorkerModel> _workers = new List<WorkerModel>();
    private readonly Subject<WorkerModel> _created = new Subject<WorkerModel>();
    
    public IReadOnlyList<WorkerModel> Workers => _workers;
    public Observable<WorkerModel> Created => _created;

    public WorkerModel Create(Vector3 spawnPosition)
    {
        WorkerModel worker = new WorkerModel(spawnPosition);
        
        _workers.Add(worker);
        _created.OnNext(worker);
        
        return worker;
    }

    public void Dispose()
    {
        foreach (var worker in _workers)
            worker.Dispose();
        
        _created.Dispose();
    }
}