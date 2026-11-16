using R3;

public interface IReadOnlyResourceStorage
{
    ReadOnlyReactiveProperty<int> Count { get; }
}