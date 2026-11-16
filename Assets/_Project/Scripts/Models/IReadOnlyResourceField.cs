using System.Collections.Generic;
using R3;

public interface IReadOnlyResourceField
{
    IReadOnlyList<ResourceModel> Resources { get; }
    ReadOnlyReactiveProperty<int> Count { get; }
    Observable<ResourceModel> Added { get; }
    Observable<ResourceModel> Removed { get; }
}
