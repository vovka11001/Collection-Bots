using R3;
using UnityEngine;

public interface IWorkerPresentationModel 
{
   ReadOnlyReactiveProperty<ResourceModel> CarriedResource { get; }
   Observable<Vector3> MoveRequested { get; }

   void ReportArrived();
}