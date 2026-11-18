using System;
using R3;
using UnityEngine;
using Zenject;

public class FlagPlacementPresenter : IInitializable, IDisposable
{
    private readonly PointerInputView _inputView;
    private readonly IBaseModelProvider _baseModelProvider;
    private readonly BaseSelection _baseSelection;
    private readonly CompositeDisposable _subscriptions = new CompositeDisposable();

    public FlagPlacementPresenter(PointerInputView inputView, IBaseModelProvider baseModelProvider,
        BaseSelection baseSelection)
    {
        _inputView = inputView;
        _baseModelProvider = baseModelProvider;
        _baseSelection = baseSelection;
    }

    public void Initialize()
    {
        _inputView.BaseClicked.Subscribe(SelectBase).AddTo(_subscriptions);
        _inputView.GroundClicked.Subscribe(point => _baseSelection.TryPlaceFlag(point)).AddTo(_subscriptions);
    }

    public void Dispose()
    {
        _subscriptions.Dispose();
    }

    private void SelectBase(BaseView baseView)
    {
        if (_baseModelProvider.TryGetModel(baseView, out BaseModel model))
            _baseSelection.Select(model);
    }
}