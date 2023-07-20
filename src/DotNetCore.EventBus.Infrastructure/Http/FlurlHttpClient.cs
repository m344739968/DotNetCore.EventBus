using DotNetCore.EventBus.Infrastructure.Json;
using DotNetCore.EventBus.Infrastructure.Models.Constant;
using DotNetCore.EventBus.Infrastructure.Models.Options;
using Flurl.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Text;
namespace DotNetCore.EventBus.Infrastructure.Http;

public class FlurlHttpClient
{ 
    private readonly ILogger<FlurlHttpClient> _logger;
    private readonly IFlurlClient _flurlClient;
    private readonly IOptions<AppSettings> _appSettings;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="logger"></param>
    public FlurlHttpClient(ILogger<FlurlHttpClient> logger, IHttpClientFactory httpClientFactory, IOptions<AppSettings> appSettings)
    {
        _logger = logger;
        _flurlClient = new FlurlClient(httpClientFactory.CreateClient());
        _appSettings = appSettings;
    }

    /// <summary>
    /// 获取token
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<string> GetToken(string url, string clientId, string clientSecret, TimeSpan? expiry = null)
    {
        var cacheKey = $"{CacheKeyConstant.Prefix}:{clientId}:token";
        var token = await RedisHelper.GetAsync(cacheKey);
        if (!string.IsNullOrEmpty(token))
        {
            return token;
        }
        var result = await _flurlClient.Request(url)
                    .WithHeader("content-type", "application/x-www-form-urlencoded")
                    .PostUrlEncodedAsync(new
                    {
                        grant_type = "client_credentials",
                        client_id = clientId,
                        client_secret = clientSecret,
                    })
                    .ReceiveJson<JObject>();
        token = result.GetValue("access_token")?.ToString();
        var timeSpan = !expiry.HasValue ? TimeSpan.FromHours(1) : expiry;
        await RedisHelper.SetAsync(cacheKey, token, timeSpan.Value);
        return token;
    }

    /// <summary>
    /// PostJson
    /// </summary>
    /// <param name="apiUrl"></param>
    /// <param name="token"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public async Task<IFlurlResponse> PostJson(string url,string request, bool isValidToken, string token)
    {
        var content = new StringContent(request, Encoding.UTF8);
        var headers = new Dictionary<string, string>();
        headers.Add("content-type", "application/json; charset=utf-8");
        if (isValidToken)
        {
            headers.Add("Authorization", $"Bearer {token}");
        }
        var result = await _flurlClient.Request(url)
            .WithHeaders(headers)
            .PostAsync(content);
        return result;
    }
}