using System.Collections.Generic;
using UnityEngine;

public class BaseLocation
{
    public BaseLocation(Vector3 dropPoint, IReadOnlyList<Vector3> workerSpawnPoints)
    {
        DropPoint = dropPoint;
        WorkerSpawnPoints = workerSpawnPoints;
    }
    
    public Vector3 DropPoint { get; }
    public IReadOnlyList<Vector3> WorkerSpawnPoints { get; }
}