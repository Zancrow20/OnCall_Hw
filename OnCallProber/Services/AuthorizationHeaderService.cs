using Microsoft.Extensions.Options;
using OnCallProber.Configs;

namespace OnCallProber.Services;

public class AuthorizationHeaderService
{
    private readonly SignatureEncoder _signatureEncoder;
    private readonly OnCallExporterConfiguration _config;
    
    public AuthorizationHeaderService(SignatureEncoder signatureEncoder, 
        IOptions<OnCallExporterConfiguration> options)
    {
        _signatureEncoder = signatureEncoder;
        _config = options.Value;
    }

    public string GetAuthorizationHeader(string endpoint, HttpMethod httpMethod, string body)
    {
        var strMethod = httpMethod.Method.ToUpper();
        var window = DateTimeOffset.Now.ToUnixTimeSeconds() / 30;

        var value = $"{window} {strMethod} {endpoint} {body}";
        var signature = _signatureEncoder.ComputeSignature(value);
        
        return $"hmac {_config.AppName}:{signature}";
    }
}