using System;
using System.Collections.Generic;
using R3;
using Zenject;

public class ResourceViewSpawner : IInitializable,IDisposable
{
    private readonly ResourceField _resourceField;
    private readonly ResourceView.Factory _resourceViewFactory;
    private readonly Dictionary<ResourceModel, ResourceView> _views =  new Dictionary<ResourceModel, ResourceView>();
    private readonly CompositeDisposable _subscritptions = new CompositeDisposable();

    public ResourceViewSpawner(ResourceView.Factory resourceViewFactory, ResourceField resourceField)
    {
        _resourceField = resourceField;
        _resourceViewFactory = resourceViewFactory;
    }

    public void Initialize()
    {
        _resourceField.Added.Subscribe(CreateView).AddTo(_subscritptions);
        _resourceField.Removed.Subscribe(RemoveView).AddTo(_subscritptions);
    }

    public void Dispose()
    {
        _subscritptions.Dispose();
    }

    private void CreateView(ResourceModel resourceModel)
    {
        ResourceView view = _resourceViewFactory.Create();
        view.SetPosition(resourceModel.Position);
        _views.Add(resourceModel, view);
    }

    private void RemoveView(ResourceModel resourceModel)
    {
        if (!_views.TryGetValue(resourceModel, out var view) == false)
            return;

        _views.Remove(resourceModel);
        view.Remove();
    }
}