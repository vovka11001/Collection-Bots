using System;
using R3;
using UnityEngine;
using Zenject;

public class ResourceRegistryDebugLogger : IInitializable, IDisposable
{
   private readonly ResourceRegistry _resourceRegistry;
   private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
   
   public ResourceRegistryDebugLogger(ResourceRegistry resourceRegistry)
   {
      _resourceRegistry = resourceRegistry;
   }

   public void Initialize()
   {
      _resourceRegistry.FreeCount.Subscribe(LogFreeCount).AddTo(_subscriptions);
   }

   public void Dispose()
   {
      _subscriptions.Dispose();
   }

   private void LogFreeCount(int freeCount)
   {
      Debug.Log($"{freeCount} free resource count");
   }
}
