using System;
using R3;
using UnityEngine;

public class BaseSelection : IDisposable
{
    private readonly IMapBounds _mapBounds;
    private readonly ReactiveProperty<BaseModel> _selected = new ReactiveProperty<BaseModel>();

    public BaseSelection(IMapBounds mapBounds)
    {
        _mapBounds = mapBounds;
    }
    
    public ReactiveProperty<BaseModel> Selected => _selected;

    public void Select(BaseModel baseModel)
    {
        _selected.Value = baseModel;
    }

    public bool TryPlaceFlag(Vector3 point)
    {
        BaseModel selectedBase = _selected.Value;

        if (selectedBase == null)
            return false;
        
        if(_mapBounds.TryProjectOnMap(point,out Vector3 mapPoint) == false)
            return false;
        
        selectedBase.PlaceFlag(mapPoint);
        _selected.Value = null;
        
        return true;
    }

    public void Dispose()
    {
        _selected.Dispose();
    }
}