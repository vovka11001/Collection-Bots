using System;
using R3;
using UnityEngine;
using Zenject;

public class BaseModel : IBasePresentationModel, IDisposable
{
   private const int EmptyCount = 0;

   private readonly GameConfig _config;
   private readonly ResourceRegistry _resourceRegistry;
   private readonly ResourceField _resourceField;
   private readonly WorkerCollection _workerCollection;
   private readonly ResourceStorage _storage = new ResourceStorage();
   private readonly WorkerFleet _fleet = new WorkerFleet();
   private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

   public BaseModel(Vector3 position, GameConfig config, ResourceRegistry resourceRegistry, ResourceField resourceField
      , WorkerCollection workerCollection)
   {
      Position = position;
      DropPoint = position + config.DropPointOffset;
      _config = config;
      _resourceRegistry = resourceRegistry;
      _resourceField = resourceField;
      _workerCollection = workerCollection;
   }
   
   public Vector3 Position { get; }
   public Vector3 DropPoint { get; }
   public IReadOnlyResourceStorage Storage => _storage;
   public ReadOnlyReactiveProperty<int> WorkerCount => _fleet.Count;

   public void Initialize()
   {
      Observable.CombineLatest(_fleet.IdleCount,_resourceRegistry.FreeCount,CanDispatch)
         .DistinctUntilChanged()
         .Where(canDispatch => canDispatch)
         .Subscribe(_ => DispatchAll()).AddTo(_subscriptions);
      
      _fleet.Delivered.Subscribe(AcceptDelivery).AddTo(_subscriptions);
   }
   
   public void CreateWorker()
   {
      WorkerModel worker = _workerCollection.Create(DropPoint);
      _fleet.Add(worker);
   }

   public void Dispose()
   {
      _subscriptions.Dispose();
      _storage.Dispose();
      _fleet.Dispose();
   }

   private bool CanDispatch(int idleWorkerCount, int freeResourceCount)
   {
      return idleWorkerCount > EmptyCount && freeResourceCount > EmptyCount;
   }

   private void DispatchAll()
   {
      WorkerModel idleWorker = _fleet.FindIdleWorker();

      while (idleWorker != null && _resourceRegistry.TryTakeFree(out ResourceModel resourceModel))
      {
         idleWorker.Assign(resourceModel,DropPoint);
         idleWorker = _fleet.FindIdleWorker();
      }
   }

   private void AcceptDelivery(ResourceModel resourceModel)
   {
      _resourceRegistry.Remove(resourceModel);
      _resourceField.Remove(resourceModel);
      _storage.AddResource();
      
      TryBuyWorker();
   }

   private void TryBuyWorker()
   {
      if(_storage.TrySpend(_config.WorkerCost))
         CreateWorker();
   }
   
   public class Factory : PlaceholderFactory<Vector3,BaseModel> {}
}