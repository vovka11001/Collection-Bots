using UnityEngine;

public interface IBasePresentationModel 
{ 
   Vector3 Position { get; }
   IReadOnlyResourceStorage Storage { get; }
}