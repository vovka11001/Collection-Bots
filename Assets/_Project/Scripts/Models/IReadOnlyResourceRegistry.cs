using R3;

public interface IReadOnlyResourceRegistry
{
    ReadOnlyReactiveProperty<int> FreeCount { get; }
}