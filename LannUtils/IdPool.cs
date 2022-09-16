namespace LannUtils; 

public class IdPool {
    private int _current;
    private readonly List<int> _pool;
    
    public IdPool() {
        _pool = new List<int>();
        _current = 0;
    }

    public int NextId() {
        if (_pool.Count <= 0) return _current++;
        _pool.RemoveAt(0);
        return _pool.First();

    }

    public void Dispose(int id) {
        _pool.Add(id);
    }
}