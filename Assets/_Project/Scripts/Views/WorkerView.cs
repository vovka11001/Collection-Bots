using UnityEngine;
using UnityEngine.AI;
using R3;
using Zenject;

[RequireComponent(typeof(NavMeshAgent))]
public class WorkerView : MonoBehaviour
{
    [SerializeField] private Transform _holdPoint;
    
    private Subject<Unit> _arrived = new Subject<Unit>();
    
    private NavMeshAgent _agent;
    private bool _isMoving;
 
    public Observable<Unit> Arrived => _arrived;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _arrived.AddTo(this);
        
        Observable.EveryUpdate().Where(_ => _isMoving).Where(_ => HasReachedDestination()).Subscribe(_ => CompleteMovement())
            .AddTo(this);
    }

    public void PlaceAt(Vector3 position)
    {
        transform.position = position;
        _agent.enabled = true;
    }

    public void MoveTo(Vector3 destination)
    {
        _isMoving = true;
        _agent.SetDestination(destination);
    }

    public void Hold(ResourceView resourceView)
    {
        resourceView.AttachTo(_holdPoint);
    }
    
    private bool HasReachedDestination()
    {
        return _agent.pathPending == false && _agent.remainingDistance <= _agent.stoppingDistance;
    }

    private void CompleteMovement()
    {
        _isMoving = false;
        _arrived.OnNext(Unit.Default);
    }
    
    public class Factory : PlaceholderFactory<WorkerView> { }
}