using UnityEngine;
using Zenject;

public class ResourceView : MonoBehaviour
{
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void Remove()
    {
        Destroy(gameObject);
    }
    
    public class Factory : PlaceholderFactory<ResourceView>
    {
    }
}