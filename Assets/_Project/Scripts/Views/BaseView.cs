using TMPro;
using UnityEngine;
using Zenject;

public class BaseView : MonoBehaviour
{
    [SerializeField] private TMP_Text _resourceCountText;
    [SerializeField] private string _resourceCountFormat = "{0}";

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetResourceCount(int count)
    {
        _resourceCountText.text = string.Format(_resourceCountFormat, count);
    }
    
    public class Factory : PlaceholderFactory<BaseView>{}
}