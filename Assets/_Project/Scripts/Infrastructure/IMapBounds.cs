using UnityEngine;

public interface IMapBounds
{
    bool TryProjectOnMap(Vector3 point, out Vector3 mapPoint);
}