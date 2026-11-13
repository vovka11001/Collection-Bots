public interface IResourceViewProvider
{
    bool TryGetView(ResourceModel resource, out ResourceView view);
}