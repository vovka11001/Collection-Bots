using UnityEngine;

[CreateAssetMenu(fileName =  "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private float _resourceSpawnIntervalSeconds = 2f;
    [SerializeField] private int _maxResourcesOnField = 10;
    [SerializeField] private float _levelRadius = 20f;
    [SerializeField] private float _scanIntervalSeconds = 1f;
    
    public float ResourceSpawnIntervalSeconds => _resourceSpawnIntervalSeconds;
    public int MaxResourcesOnField => _maxResourcesOnField;
    public float LevelRadius => _levelRadius;
    public float ScanIntervalSeconds => _scanIntervalSeconds;
}