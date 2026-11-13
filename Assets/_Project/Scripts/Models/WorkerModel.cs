using System;
using R3;
using UnityEngine;

public class WorkerModel : IDisposable
{
    private readonly Vector3 _dropPoint;
    private readonly ReactiveProperty<WorkerState> _state = new ReactiveProperty<WorkerState>(WorkerState.Idle);
    private readonly ReactiveProperty<ResourceModel> _carriedResource = new ReactiveProperty<ResourceModel>();
    private readonly Subject<Vector3> _moveRequested = new Subject<Vector3>();
    private readonly Subject<ResourceModel> _delivered = new Subject<ResourceModel>();
    
    private ResourceModel _targetResource;

    public WorkerModel(Vector3 homePosition, Vector3 dropPoint)
    {
        _dropPoint = dropPoint;
        HomePosition = homePosition;
    }
    
    public Vector3 HomePosition { get; }
    public ReadOnlyReactiveProperty<WorkerState> State => _state;
    public ReadOnlyReactiveProperty<ResourceModel> CarriedResource => _carriedResource;
    public Observable<Vector3> MoveRequested => _moveRequested;
    public Observable<ResourceModel> Delivered => _delivered;
    public bool IsIdle => _state.Value == WorkerState.Idle;

    public void Assign(ResourceModel resourceModel)
    {
        if(IsIdle == false)
            return;
        
        _targetResource = resourceModel;
        _state.Value = WorkerState.MovingToResource;
        _moveRequested.OnNext(resourceModel.Position);
    }

    public void ReportArrived()
    {
        switch (_state.Value)
        {
            case WorkerState.MovingToResource:
                PickUpTarget();
                break;
            
            case WorkerState.CarryingToBase:
                DeliverCarried();
                break;
        }
    }

    public void Dispose()
    {
        _moveRequested.Dispose();
        _delivered.Dispose();
        _carriedResource.Dispose();
        _state.Dispose();
    }

    private void PickUpTarget()
    {
        _carriedResource.Value = _targetResource;
        _targetResource = null;
        _state.Value = WorkerState.CarryingToBase;
        _moveRequested.OnNext(_dropPoint);
    }
    
    private void DeliverCarried()
    {
        ResourceModel deliveredResource = _carriedResource.Value;

        _carriedResource.Value = null;
        _delivered.OnNext(deliveredResource);
        _state.Value = WorkerState.Idle;
    }
}