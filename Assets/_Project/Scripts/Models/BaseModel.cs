using System;
using R3;
using UnityEngine;
using Zenject;

public class BaseModel : IBasePresentationModel, IDisposable
{
   private const int EmptyCount = 0;

   private readonly GameConfig _config;
   private readonly ResourceRegistry _resourceRegistry = new ResourceRegistry();
   private readonly ResourceField _resourceField;
   private readonly WorkerCollection _workerCollection;
   private readonly ResourceStorage _storage = new ResourceStorage();
   private readonly WorkerFleet _fleet = new WorkerFleet();
   private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
   private readonly ReactiveProperty<Vector3?> _flagPosition = new ReactiveProperty<Vector3?>();
   private readonly Subject<BaseExpansionRequest> _expansionRequest = new Subject<BaseExpansionRequest>();

   private WorkerModel _builder;
   private IDisposable _builderArrivalSubscription = Disposable.Empty;
   
   public BaseModel(Vector3 position, GameConfig config, ResourceField resourceField
      , WorkerCollection workerCollection)
   {
      Position = position;
      DropPoint = position + config.DropPointOffset;
      _config = config;
      _resourceField = resourceField;
      _workerCollection = workerCollection;
   }
   
   public Vector3 Position { get; }
   public Vector3 DropPoint { get; }
   public IReadOnlyResourceStorage Storage => _storage;
   public ReadOnlyReactiveProperty<int> WorkerCount => _fleet.Count;
   public ReadOnlyReactiveProperty<Vector3?> FlagPosition => _flagPosition;
   public Observable<BaseExpansionRequest> ExpansionRequest => _expansionRequest;
   
   private bool IsExpansionPriority => _flagPosition.Value.HasValue && _fleet.Count.CurrentValue >= _config.MinWorkersForExpansion;

   public void Initialize()
   {
      Observable.CombineLatest(_fleet.IdleCount,_resourceRegistry.FreeCount,_flagPosition
         ,(idleWorkerCount,freeResourceCount, _) => CanDispatch(idleWorkerCount,freeResourceCount))
         .DistinctUntilChanged().Where(canDispatch => canDispatch).Subscribe(_ => DispatchAll()).AddTo(_subscriptions);
      
      _fleet.Delivered.Subscribe(AcceptDelivery).AddTo(_subscriptions);
   }
   
   public void CreateWorker()
   {
      AddWorker(_workerCollection.Create(DropPoint));
   }

   public void AddWorker(WorkerModel worker)
   {
      _fleet.Add(worker);
   }
   
   public bool KnowsResource(ResourceModel resource)
   {
      return _resourceRegistry.Contains(resource);
   }

   public void RegisterResource(ResourceModel resource)
   {
      _resourceRegistry.Register(resource);
   }

   public void PlaceFlag(Vector3 position)
   {
      if (_builder != null)
         _builder.RedirectBuild(GetBuildApproachPoint(position));
      
      _flagPosition.Value = position;
   }

   public void Dispose()
   {
      _subscriptions.Dispose();
      _builderArrivalSubscription.Dispose();
      _fleet.Dispose();
      _storage.Dispose();
      _flagPosition.Dispose();
      _expansionRequest.Dispose();
      _resourceRegistry.Dispose();
   }

   private bool CanDispatch(int idleWorkerCount, int freeResourceCount)
   {
      return idleWorkerCount > EmptyCount && (freeResourceCount > EmptyCount || CanSendBuilder());
   }

   private bool CanSendBuilder()
   {
      return IsExpansionPriority && _builder == null && _storage.Count.CurrentValue >= _config.BaseCost;
   }

   private void DispatchAll()
   {
      TrySendBuilder();
      DispatchToResources();
   }

   private void TrySendBuilder()
   {
      if (CanSendBuilder() == false)
         return;

      WorkerModel builder = _fleet.FindIdleWorker();

      if (builder == null || _storage.TrySpend(_config.BaseCost) == false)
         return;
      
      _builder = builder;
      _builderArrivalSubscription = builder.BuildSiteReached.Take(1).Subscribe(_ => CompleteExpansion());
      
      _builder.AssignBuild(GetBuildApproachPoint(_flagPosition.Value.Value));
   }

   private void DispatchToResources()
   {
      WorkerModel idleWorker = _fleet.FindIdleWorker();

      while (idleWorker != null && _resourceRegistry.TryTakeFree(out ResourceModel resourceModel))
      {
         idleWorker.Assign(resourceModel,DropPoint);
         idleWorker = _fleet.FindIdleWorker();
      }
   }

   private void CompleteExpansion()
   {
      WorkerModel builder = _builder;
      Vector3 newBasePosition = _flagPosition.Value.Value;

      _builder = null;
      _builderArrivalSubscription.Dispose();
      _fleet.Remove(builder);
      _flagPosition.Value = null;
      
      _expansionRequest.OnNext(new BaseExpansionRequest(newBasePosition, builder));

      TryBuyWorkers();
   }

   private void AcceptDelivery(ResourceModel resourceModel)
   {
      _resourceRegistry.Remove(resourceModel);
      _resourceField.Remove(resourceModel);
      _storage.AddResource();
      
      TrySendBuilder();
      TryBuyWorkers();
   }

   private void TryBuyWorkers()
   {
      while(IsExpansionPriority == false && _storage.TrySpend(_config.WorkerCost))
         CreateWorker();
   }

   private Vector3 GetBuildApproachPoint(Vector3 flagPosition)
   {
      return flagPosition + _config.DropPointOffset;
   }
   
   public class Factory : PlaceholderFactory<Vector3,BaseModel> {}
}