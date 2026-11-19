using System;
using R3;
using UnityEngine;

public class WorkerModel : IWorkerPresentationModel, IDisposable
{
    private readonly ReactiveProperty<WorkerState> _state = new ReactiveProperty<WorkerState>(WorkerState.Idle);
    private readonly ReactiveProperty<ResourceModel> _carriedResource = new ReactiveProperty<ResourceModel>();
    private readonly Subject<Vector3> _moveRequested = new Subject<Vector3>();
    private readonly Subject<ResourceModel> _delivered = new Subject<ResourceModel>();
    private readonly Subject<Unit> _buildSiteReached = new Subject<Unit>();
    
    private ResourceModel _targetResource;
    private Vector3 _dropPoint;
    
    public WorkerModel(Vector3 spawnPosition)
    {
        SpawnPosition = spawnPosition;
    }
    
    public Vector3 SpawnPosition { get; }
    public ReadOnlyReactiveProperty<WorkerState> State => _state;
    public ReadOnlyReactiveProperty<ResourceModel> CarriedResource => _carriedResource;
    public Observable<Vector3> MoveRequested => _moveRequested;
    public Observable<ResourceModel> Delivered => _delivered;
    public Observable<Unit> BuildSiteReached => _buildSiteReached;
    public bool IsIdle => _state.Value == WorkerState.Idle;

    public void Assign(ResourceModel resourceModel,Vector3 dropPoint)
    {
        if(IsIdle == false)
            return;
        
        _targetResource = resourceModel;
        _dropPoint = dropPoint;
        _state.Value = WorkerState.MovingToResource;
        _moveRequested.OnNext(resourceModel.Position);
    }

    public void AssignBuild(Vector3 destination)
    {
        if (IsIdle == false)
            return;
        
        _state.Value = WorkerState.MovingToBuildSite;
        _moveRequested.OnNext(destination);
    }

    public void RedirectBuild(Vector3 destination)
    {
        if (_state.Value != WorkerState.MovingToBuildSite)
            return;
        
        _moveRequested.OnNext(destination);
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
            
            case WorkerState.MovingToBuildSite:
                ReachBuildSite();
                break;
        }
    }

    public void Dispose()
    {
        _moveRequested.Dispose();
        _delivered.Dispose();
        _carriedResource.Dispose();
        _state.Dispose();
        _buildSiteReached.Dispose();
    }

    private void ReachBuildSite()
    {
        _buildSiteReached.OnNext(Unit.Default);
        _state.Value = WorkerState.Idle;
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