using TMPro;
using UnityEngine;
using Zenject;

public class BaseView : MonoBehaviour
{
    [SerializeField] private TMP_Text _resourceCountText;
    [SerializeField] private string _resourceCountFormat = "{0}";
    [SerializeField] private Renderer _bodyRenderer;
    [SerializeField] private Color _selectedColor = Color.yellow;
    [SerializeField] private Transform _flag;
    
    private Color _defaultColor;

    private void Awake()
    {
        _defaultColor = _bodyRenderer.material.color;
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    public void SetResourceCount(int count)
    {
        _resourceCountText.text = string.Format(_resourceCountFormat, count);
    }

    public void SetSelected(bool isSelected)
    {
        _bodyRenderer.material.color = isSelected ? _selectedColor : _defaultColor;
    }

    public void ShowFlag(Vector3 position)
    {
        _flag.position = position;
        _flag.gameObject.SetActive(true);
    }

    public void HideFlag()
    {
        _flag.gameObject.SetActive(false);
    }
    
    public class Factory : PlaceholderFactory<BaseView>{}
}