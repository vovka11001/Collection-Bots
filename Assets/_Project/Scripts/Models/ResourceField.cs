using System;
using System.Collections.Generic;
using R3;

public class ResourceField : IDisposable
{
   private readonly List<ResourceModel> _resources = new List<ResourceModel>();
   private readonly ReactiveProperty<int> _count = new ReactiveProperty<int>();
   private readonly Subject<ResourceModel> _added = new Subject<ResourceModel>();
   private readonly Subject<ResourceModel> _removed = new Subject<ResourceModel>();
   
   public ReadOnlyReactiveProperty<int> Count => _count;
   public IReadOnlyList<ResourceModel> Resources => _resources;
   public Observable<ResourceModel> Added => _added;
   public Observable<ResourceModel> Removed => _removed;

   public void Add(ResourceModel resource)
   {
      _resources.Add(resource);
      _added.OnNext(resource);
      _count.Value = _resources.Count;
   }

   public void Remove(ResourceModel resource)
   {
      if (_resources.Remove(resource))
      {
         _count.Value = _resources.Count;
         _removed.OnNext(resource);
      }
   }

   public void Dispose()
   {
      _count.Dispose();
      _added.Dispose();
      _removed.Dispose();
   }
}