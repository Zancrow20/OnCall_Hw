using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace OnCallSLA.Services;

public class PrometheusClient
{
    private readonly string _baseUrl;
    private readonly HttpClient _httpClient;

    public PrometheusClient(string baseUrl, HttpClient httpClient)
    {
        _httpClient = httpClient;
        _baseUrl = $"{baseUrl}/api/v1/query";
    }

    public async Task<string> GetLastValue(string query, DateTimeOffset dateTimeOffset, string defaultValue)
    {
        var uri = $"{_baseUrl}?query={query}&time={dateTimeOffset.ToUnixTimeSeconds()}";
        
        try
        {
            var response = await _httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var prometheusResponse = JsonConvert.DeserializeObject<PrometheusResponse>(content);
            
            if (prometheusResponse is null)
                return defaultValue;

            var result = prometheusResponse.Data.Result;
            return result.Length == 0 
                ? defaultValue 
                : result[0].value[1].ToString();
        }
        catch
        {
            return defaultValue;
        }
    }

    public class PrometheusResponse
    {
        [JsonPropertyName("data")]
        public Data Data { get; set; }
        
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
    
    public class Data
    {
        [JsonPropertyName("resultType")]
        public string ResultType { get; set; }
        
        [JsonPropertyName("result")]
        public dynamic[] Result { get; set; }
    }
}

