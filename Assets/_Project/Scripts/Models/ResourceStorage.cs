using System;
using R3;

public class ResourceStorage : IReadOnlyResourceStorage, IDisposable
{
    private readonly ReactiveProperty<int> _count = new ReactiveProperty<int>();
    
    public ReadOnlyReactiveProperty<int> Count => _count;

    public void AddResource()
    {
        _count.Value++;
    }

    public bool TrySpend(int amount)
    {
        if (_count.Value < amount)
        {
            return false;
        }
        
        _count.Value -= amount;
        return true;
    }

    public void Dispose()
    {
        _count.Dispose();
    }
}