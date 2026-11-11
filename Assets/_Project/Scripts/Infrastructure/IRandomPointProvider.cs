using UnityEngine;

public interface IRandomPointProvider
{
    bool TryGetRandomPoint(out Vector3 point);
}