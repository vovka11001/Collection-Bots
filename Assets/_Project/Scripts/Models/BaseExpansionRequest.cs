using UnityEngine;

public class BaseExpansionRequest 
{
    public BaseExpansionRequest(Vector3 position, WorkerModel builder)
    {
       Position = position;
       Builder = builder;
    }   
    
    public Vector3 Position { get; }
    public WorkerModel Builder { get; }
}