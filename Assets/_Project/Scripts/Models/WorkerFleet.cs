using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkerFleet : IDisposable
{
    private readonly List<WorkerModel> _workers = new List<WorkerModel>();

    public WorkerFleet(BaseLocation baseLocation)
    {
        foreach (Vector3 spawnPoint in baseLocation.WorkerSpawnPoints)
        {
            _workers.Add(new WorkerModel(spawnPoint, baseLocation.DropPoint));
        }
    }
    
    public IReadOnlyList<WorkerModel> Workers => _workers;

    public void Dispose()
    {
        foreach (WorkerModel workerModel in _workers)
        {
            workerModel.Dispose();
        }
    }
}