using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sodium;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using Microsoft.Win32;

namespace Bluube.Auth;

public class BluubeAuth
{
    private readonly string _appId;
    private readonly string _ownerId;
    private readonly string _version;
    private readonly string _apiUrl;
    private readonly HttpClient _client;
    private readonly byte[] _serverPublicKey;
    private readonly string _hwid;

    private string? _sessionId;
    private bool _isInitialized;
    private bool _isAuthenticated;
    private JObject? _userData;
    private string? _lastMessage;

    private System.Threading.Timer? _heartbeatTimer;
    private int _heartbeatInterval = 30000; // 30 seconds
    private DateTime? _lastValidHeartbeat;

    private const int SignatureMaxSkewSeconds = 600;

    public string? LastMessage => _lastMessage;
    public JObject? UserData => _userData;
    public bool IsInitialized => _isInitialized;
    public bool IsAuthenticated => _isAuthenticated;

    public BluubeAuth(string appId, string ownerId, string version = "1.0", string? apiBaseUrl = null)
    {
        _appId = appId;
        _ownerId = ownerId;
        _version = version;
        _apiUrl = NormalizeBaseUrl(apiBaseUrl ?? "https://api.bluube.com");
        
        _serverPublicKey = HexToBytes("f86ac4fb026c6f58159e3d4e8d807ff17c96151cc4b7a8b0624d4e9a1e072bb8");

        var handler = new HttpClientHandler { UseCookies = true };
        _client = new HttpClient(handler);
        _client.DefaultRequestHeaders.Add("User-Agent", "BluubeAuth-CSharp");
        _client.Timeout = TimeSpan.FromSeconds(15);

        _hwid = GetHwid();
    }

    public async Task<bool> Initialize()
    {
        try
        {
            var ip = await GetPublicIp();
            var payload = new 
            { 
                appId = _appId, 
                ownerId = _ownerId, 
                version = _version,
                ip = ip
            };

            var res = await Post("/initialize", payload);
            if (res.Success)
            {
                _sessionId = res.SessionId;
                if (!string.IsNullOrEmpty(res.PublicKey))
                {
                    var receivedKey = Convert.FromBase64String(res.PublicKey);
                    if (!receivedKey.SequenceEqual(_serverPublicKey))
                        throw new Exception("Security Error: Invalid server public key.");
                }

                _isInitialized = true;
                _lastMessage = res.Message ?? "Successfully initialized.";
                StartHeartbeat();
                return true;
            }

            _lastMessage = res.Message ?? "Failed to initialize.";
            return false;
        }
        catch (SecurityException ex)
        {
            Terminate($"Security Breach: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _lastMessage = $"Error: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> Login(string username, string password)
    {
        if (!_isInitialized || string.IsNullOrEmpty(_sessionId))
        {
            _lastMessage = "Please call Initialize first.";
            return false;
        }

        try
        {
            var ip = await GetPublicIp();
            var payload = new
            {
                sessionId = _sessionId,
                appId = _appId,
                ownerId = _ownerId,
                version = _version,
                username = username,
                password = password,
                hwid = _hwid,
                ip = ip
            };

            var res = await Post("/auth/login", payload);
            _lastMessage = res.Message;

            if (res.Success)
            {
                _isAuthenticated = true;
                _userData = res.FullData?["user"] as JObject;
                return true;
            }
            return false;
        }
        catch (SecurityException ex)
        {
            Terminate($"Security Breach: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _lastMessage = $"Login failed: {ex.Message}";
            return false;
        }
    }

    public async Task<bool> Register(string key, string username, string password)
    {
        if (!_isInitialized || string.IsNullOrEmpty(_sessionId))
        {
            _lastMessage = "Please call Initialize first.";
            return false;
        }

        try
        {
            var ip = await GetPublicIp();
            var payload = new
            {
                sessionId = _sessionId,
                appId = _appId,
                ownerId = _ownerId,
                version = _version,
                licenseKey = key,
                username = username,
                password = password,
                hwid = _hwid,
                ip = ip
            };

            var res = await Post("/auth/register", payload);
            _lastMessage = res.Message;

            if (res.Success)
            {
                _isAuthenticated = true;
                _userData = res.FullData?["user"] as JObject;
                return true;
            }
            return false;
        }
        catch (SecurityException ex)
        {
            Terminate($"Security Breach: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            _lastMessage = $"Registration failed: {ex.Message}";
            return false;
        }
    }

    private async Task<ApiResponse> Post(string path, object payload)
    {
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _client.PostAsync(_apiUrl + path, content);
        var rawBody = await response.Content.ReadAsStringAsync();

        VerifySignature(rawBody, response.Headers);

        var data = JsonConvert.DeserializeObject<JObject>(rawBody);
        return new ApiResponse
        {
            Success = data?["success"]?.Value<bool>() ?? false,
            Message = data?["message"]?.Value<string>(),
            SessionId = (data?["sessionId"]?.Value<string>()) ?? (data?["session"]?["id"]?.Value<string>()),
            PublicKey = data?["publicKey"]?.Value<string>(),
            FullData = data
        };
    }

    private void VerifySignature(string body, HttpResponseHeaders headers)
    {
        if (!headers.TryGetValues("X-Bluube-Signature", out var sigValues) || 
            !headers.TryGetValues("X-Bluube-Timestamp", out var tsValues))
            throw new SecurityException("Missing response signature.");

        var sig = sigValues.First();
        var ts = tsValues.First();

        if (!long.TryParse(ts, out var tsLong))
            throw new SecurityException("Invalid signature timestamp.");

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Math.Abs(now - tsLong) > SignatureMaxSkewSeconds)
            throw new SecurityException("Response signature expired.");

        var signature = Convert.FromBase64String(sig);
        var msg = Encoding.UTF8.GetBytes(ts + body);

        if (!PublicKeyAuth.VerifyDetached(signature, msg, _serverPublicKey))
            throw new SecurityException("Integrity check failed. Response signature was tampered.");
    }

    private void StartHeartbeat()
    {
        _heartbeatTimer?.Dispose();
        _heartbeatTimer = new System.Threading.Timer(
            _ => { _ = HeartbeatTickAsync(); },
            null,
            _heartbeatInterval,
            _heartbeatInterval);
    }

    private async Task HeartbeatTickAsync()
    {
        try
        {
            await HeartbeatTick();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Environment.Exit(0);
        }
    }

    private async Task HeartbeatTick()
    {
        if (!_isInitialized || string.IsNullOrEmpty(_sessionId)) return;

        try
        {
            var ip = await GetPublicIp();
            var payload = new { sessionId = _sessionId, ip = ip, version = _version, hwid = _hwid };
            var res = await Post("/heartbeat", payload);

            if (!res.Success)
            {
                if (string.Equals(res.Message, "Invalid session", StringComparison.OrdinalIgnoreCase))
                    Environment.Exit(0);
                Terminate(res.Message ?? "Session terminated.");
            }
            _lastValidHeartbeat = DateTime.Now;
        }
        catch (SecurityException ex)
        {
            Terminate($"Security Breach: {ex.Message}");
        }
        catch (Exception)
        {
            Environment.Exit(0);
        }
    }

    private async Task<string?> GetPublicIp()
    {
        try
        {
            return await _client.GetStringAsync("https://api4.ipify.org");
        }
        catch { return null; }
    }

    private string GetHwid()
    {
        try
        {
            var currentUser = WindowsIdentity.GetCurrent();
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.User?.Value))
                return currentUser.User.Value;
            
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Cryptography");
            return key?.GetValue("MachineGuid")?.ToString() ?? "UNKNOWN_HWID";
        }
        catch { return "UNKNOWN_HWID"; }
    }

    private string NormalizeBaseUrl(string url)
    {
        var baseUri = url.Trim().TrimEnd('/');
        return baseUri + "/api/client";
    }

    private byte[] HexToBytes(string hex)
    {
        if (hex.StartsWith("0x")) hex = hex.Substring(2);
        var bytes = new byte[hex.Length / 2];
        for (var i = 0; i < bytes.Length; i++)
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }

    private void Terminate(string message)
    {
        Console.WriteLine("[TERMINATED] " + message);
        Environment.Exit(1);
    }

    private class ApiResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? SessionId { get; set; }
        public string? PublicKey { get; set; }
        public JObject? FullData { get; set; }
    }

    public class SecurityException : Exception
    {
        public SecurityException(string message) : base(message) { }
    }
}
