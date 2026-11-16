using TMPro;
using UnityEngine;

public class ResourceCounterView : MonoBehaviour
{
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private string _countFormat = "Ресурсы: {0}";

    public void SetCount(int count)
    {
        _countText.text = string.Format(_countFormat, count);
    }
}