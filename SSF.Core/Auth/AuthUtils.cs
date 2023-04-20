using System.Security.Cryptography;
using System.Text;

namespace SFF.Auth;

public static class AuthUtils
{
    private static readonly SHA256 HashAlg = SHA256.Create();
    private static readonly RandomNumberGenerator RNG = RandomNumberGenerator.Create();

    public static string ComputeCheckSum(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input + "5@lty!!");
        var hashBytes = HashAlg.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes, hashBytes.Length - 3, 3).Replace("-", "");
    }

    public static string ComputeHash(string input)
    {
        var bytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = HashAlg.ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    public static string Generate()
    {
        var key = new byte[32];
        RNG.GetBytes(key);
        var firstPart = Convert.ToBase64String(key)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
        var secondPart = ComputeCheckSum(firstPart);
        return string.Join("_", firstPart, secondPart);
    }
}
