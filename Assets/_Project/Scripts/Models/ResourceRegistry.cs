using System;
using System.Collections.Generic;
using System.Linq;
using R3;

public class ResourceRegistry : IDisposable
{
    private readonly List<ResourceModel> _freeResources = new List<ResourceModel>();
    private readonly List<ResourceModel> _busyResources = new List<ResourceModel>();
    private readonly ReactiveProperty<int> _freeCount = new ReactiveProperty<int>();
    
    public ReadOnlyReactiveProperty<int> FreeCount => _freeCount;
    
    public bool Contains(ResourceModel resourceModel)
    {
        return _freeResources.Contains(resourceModel) || _busyResources.Contains(resourceModel);
    }

    public void Register(ResourceModel resourceModel)
    {
        if (Contains(resourceModel))
            return;
        
        _freeResources.Add(resourceModel);
        UpdateFreeCount();
    }

    public bool TryTakeFree(out ResourceModel resourceModel)
    {
        resourceModel = _freeResources.FirstOrDefault();

        if (resourceModel == null)
            return false;
        
        _freeResources.Remove(resourceModel);
        _busyResources.Add(resourceModel);
        
        return true;
    }

    public void Dispose()
    {
        _freeCount.Dispose();
    }

    private void UpdateFreeCount()
    {
        _freeCount.Value = _freeResources.Count;
    }
}