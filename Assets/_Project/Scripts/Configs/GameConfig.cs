using UnityEngine;

[CreateAssetMenu(fileName =  "GameConfig", menuName = "Configs/GameConfig")]
public class GameConfig : ScriptableObject
{
    [SerializeField] private float _resourceSpawnIntervalSeconds = 2f;
    [SerializeField] private int _maxResourcesOnField = 10;
    [SerializeField] private float _levelRadius = 20f;
    [SerializeField] private float _scanIntervalSeconds = 1f;
    [SerializeField] private Vector3 _startBasePosition = Vector3.zero;
    [SerializeField] private Vector3 _dropPointOffset = new Vector3(0f, 0f, -3.2f);
    [SerializeField] private int _initialWorkerCount = 3;
    [SerializeField] private int _workerCost = 3;
    [SerializeField] private int _baseCost = 5;
    [SerializeField] private int _minWorkersForExpansion = 5;
    
    public float ResourceSpawnIntervalSeconds => _resourceSpawnIntervalSeconds;
    public int MaxResourcesOnField => _maxResourcesOnField;
    public float LevelRadius => _levelRadius;
    public float ScanIntervalSeconds => _scanIntervalSeconds;
    public Vector3 StartBasePosition => _startBasePosition;
    public Vector3 DropPointOffset => _dropPointOffset;
    public int InitialWorkerCount => _initialWorkerCount;
    public int WorkerCost => _workerCost;
    public int BaseCost => _baseCost;
    public int MinWorkersForExpansion => _minWorkersForExpansion;
}