using Microsoft.AspNetCore.Http;
using System.Text;

/// <summary>
/// Session giả lập dùng cho unit test.
/// Lưu dữ liệu trong Dictionary&lt;string, byte[]&gt; trong bộ nhớ.
/// 
/// Lưu ý quan trọng:
///   HttpContext.Session.GetString(key) và SetString(key, value) trong ASP.NET Core
///   là EXTENSION METHODS (SessionExtensions) gọi TryGetValue / Set bên dưới.
///   Do đó class này CHỈ cần implement đúng ISession (đặc biệt TryGetValue và Set),
///   KHÔNG cần tự định nghĩa thêm GetString / SetString.
/// </summary>
public class DefaultSession : ISession
{
    private readonly Dictionary<string, byte[]> _data = new();

    public bool IsAvailable => true;
    public string Id => Guid.NewGuid().ToString();
    public IEnumerable<string> Keys => _data.Keys;

    public void Clear() => _data.Clear();

    public void Remove(string key) => _data.Remove(key);

    public void Set(string key, byte[] value)
    {
        if (value == null)
            _data.Remove(key);
        else
            _data[key] = value;
    }

    public bool TryGetValue(string key, out byte[]? value)
        => _data.TryGetValue(key, out value);

    public Task LoadAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;

    public Task CommitAsync(CancellationToken cancellationToken = default)
        => Task.CompletedTask;
}