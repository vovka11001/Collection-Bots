using UnityEngine;
using UnityEngine.AI;

public class NavMeshRandomPointProvider : IRandomPointProvider
{
    private const float SampleMaxDistance = 2f;

    private GameConfig _config;
    private Vector3 _levelCenter = Vector3.zero;

    public NavMeshRandomPointProvider(GameConfig config)
    {
        _config = config;
    }

    public bool TryGetRandomPoint(out Vector3 point)
    {
        Vector2 randomOffset = Random.insideUnitCircle * _config.LevelRadius;
        Vector3 candidatePoint = _levelCenter + Vector3.right * randomOffset.x + Vector3.forward * randomOffset.y;

        if (NavMesh.SamplePosition(candidatePoint, out NavMeshHit navMeshHit, SampleMaxDistance, NavMesh.AllAreas))
        {
            point = navMeshHit.position;
            return true;
        }
        
        point = default;
        return false;
    }
}
