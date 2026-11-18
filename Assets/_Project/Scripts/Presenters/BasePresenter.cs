using System;
using R3;
using UnityEngine;
using Zenject;

public class BasePresenter : IDisposable
{
    private readonly IBasePresentationModel _model;
    private readonly BaseView _view;
    private readonly BaseSelection _baseSelection;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();
    
    public BasePresenter(IBasePresentationModel model, BaseView view,BaseSelection baseSelection)
    {
        _model = model;
        _view = view;
        _baseSelection = baseSelection;
    }

    public void Initialize()
    {
        _view.SetPosition(_model.Position);

        _model.Storage.Count.Subscribe(_view.SetResourceCount).AddTo(_subscriptions);
        _model.FlagPosition.Subscribe(UpdateFlag).AddTo(_subscriptions);
        
        _baseSelection.Selected.Select(selectedBase => selectedBase == _model).Subscribe(_view.SetSelected).AddTo(_subscriptions);
    }

    private void UpdateFlag(Vector3? flagPosition)
    {
        if (flagPosition.HasValue)
        {
            _view.ShowFlag(flagPosition.Value);
            return;
        }
        
        _view.HideFlag();
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }
    
    public class Factory : PlaceholderFactory<IBasePresentationModel, BaseView, BasePresenter> {}
}