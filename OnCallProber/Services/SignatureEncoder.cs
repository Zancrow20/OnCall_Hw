using System.Security.Cryptography;
using System.Text;

namespace OnCallProber.Services;

public class SignatureEncoder : IDisposable
{
    private readonly HMACSHA512 _encoder;

    public SignatureEncoder(string key)
    {
        _encoder = new HMACSHA512(Encoding.UTF8.GetBytes(key));
    }
    
    public string ComputeSignature(string value)
    {
        var hash = _encoder.ComputeHash(Encoding.UTF8.GetBytes(value));

        return Convert.ToBase64String(hash)
            .Replace('+', '-')
            .Replace('/', '_');
    }

    public void Dispose() => _encoder.Dispose();
}