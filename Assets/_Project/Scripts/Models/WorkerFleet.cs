using System;
using System.Collections.Generic;
using System.Linq;
using R3;

public class WorkerFleet : IDisposable
{
   private readonly List<WorkerModel> _workers = new List<WorkerModel>();
   private readonly Dictionary<WorkerModel,IDisposable> _workerSubscriptions = new Dictionary<WorkerModel,IDisposable>();
   private readonly ReactiveProperty<int> _count = new ReactiveProperty<int>();
   private readonly ReactiveProperty<int> _idleCount = new ReactiveProperty<int>();
   private readonly Subject<ResourceModel> _delivered  = new Subject<ResourceModel>();
   
   public IReadOnlyList<WorkerModel> Workers => _workers;
   public ReadOnlyReactiveProperty<int> Count => _count;
   public ReadOnlyReactiveProperty<int> IdleCount => _idleCount;
   public Observable<ResourceModel> Delivered => _delivered;

   public void Add(WorkerModel worker)
   {
      _workers.Add(worker);
      _count.Value =  _workers.Count;
      _workerSubscriptions[worker] = SubscribeToWorker(worker);
   }

   public void Remove(WorkerModel worker)
   {
      if (_workers.Remove(worker) == false)
         return;
      
      _workerSubscriptions[worker].Dispose();
      _workerSubscriptions.Remove(worker);
      
      _count.Value = _workers.Count;
      UpdateIdleCount();
   }
   
   public WorkerModel FindIdleWorker()
   {
      return _workers.FirstOrDefault(worker => worker.IsIdle);
   }

   public void Dispose()
   {
      foreach(IDisposable workerSubscription in _workerSubscriptions.Values)
         workerSubscription.Dispose();
      
      _count.Dispose();
      _idleCount.Dispose();
      _delivered.Dispose();
   }

   private IDisposable SubscribeToWorker(WorkerModel worker)
   {
      CompositeDisposable subscriptions = new CompositeDisposable();
      
      worker.State.Subscribe(_ => UpdateIdleCount()).AddTo(subscriptions);
      worker.Delivered.Subscribe(_delivered.OnNext).AddTo(subscriptions);
      
      return subscriptions;
   }

   private void UpdateIdleCount()
   {
      _idleCount.Value = _workers.Count(worker => worker.IsIdle);
   }
}