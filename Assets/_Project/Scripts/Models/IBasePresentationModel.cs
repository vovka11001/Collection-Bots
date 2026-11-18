using R3;
using UnityEngine;

public interface IBasePresentationModel 
{ 
   Vector3 Position { get; }
   IReadOnlyResourceStorage Storage { get; }
   ReadOnlyReactiveProperty<Vector3?> FlagPosition { get; }
}