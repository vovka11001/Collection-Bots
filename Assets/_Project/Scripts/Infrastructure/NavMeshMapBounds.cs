using UnityEngine;
using UnityEngine.AI;

public class NavMeshMapBounds : IMapBounds
{
    private const float MaxProjectionDistance = 0.5f;

    public bool TryProjectOnMap(Vector3 point, out Vector3 mapPoint)
    {
        if (NavMesh.SamplePosition(point, out NavMeshHit navMeshHit, MaxProjectionDistance, NavMesh.AllAreas))
        {
            mapPoint = navMeshHit.position;
            return true;
        }
        
        mapPoint = default;
        
        return false;
    }
}