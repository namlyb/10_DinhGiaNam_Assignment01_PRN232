using System.Net.Http.Json;
using System.Text.Json;

namespace _10_DinhGiaNam_Assignment01_FrontEnd.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _factory;
    private readonly IHttpContextAccessor _http;
    private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };
    public ApiClient(IHttpClientFactory factory, IHttpContextAccessor http) { _factory = factory; _http = http; }

    private HttpClient Create()
    {
        var c = _factory.CreateClient("Backend");
        var s = _http.HttpContext?.Session;
        var role = s?.GetString("Role"); var id = s?.GetString("AccountId");
        if (!string.IsNullOrEmpty(role)) c.DefaultRequestHeaders.Add("X-User-Role", role);
        if (!string.IsNullOrEmpty(id)) c.DefaultRequestHeaders.Add("X-User-Id", id);
        return c;
    }

    public async Task<T?> GetAsync<T>(string url)
    {
        var r = await Create().GetAsync(url); if (!r.IsSuccessStatusCode) return default;
        return await r.Content.ReadFromJsonAsync<T>(_json);
    }

    public async Task<(bool ok, string message, T? data)> SendAsync<T>(HttpMethod method, string url, object? body = null)
    {
        var req = new HttpRequestMessage(method, url);
        if (body != null) req.Content = JsonContent.Create(body);
        var r = await Create().SendAsync(req);
        var txt = await r.Content.ReadAsStringAsync();
        T? data = default;
        if (r.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(txt)) try { data = JsonSerializer.Deserialize<T>(txt, _json); } catch { }
        string msg = r.IsSuccessStatusCode ? "Success" : txt;
        if (!r.IsSuccessStatusCode && !string.IsNullOrWhiteSpace(txt))
        {
            try { using var d = JsonDocument.Parse(txt); if (d.RootElement.TryGetProperty("message", out var m)) msg = m.GetString() ?? txt; } catch { }
        }
        return (r.IsSuccessStatusCode, msg, data);
    }
}
