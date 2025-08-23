using System.Security.Cryptography;

namespace Shadowchats.Authentication.Infrastructure.Identity;

public class SaltsManager : ISaltsManager
{
    public byte[] GenerateDynamic() => RandomNumberGenerator.GetBytes(DynamicSaltSizeInBytes);

    public byte[] CombineStaticAndDynamicSalts(IEnumerable<byte> dynamicSalt) =>
        StaticSalt.Concat(dynamicSalt).ToArray();

    public int DynamicSaltSizeInBytes { get; } = 64;
    
    private static readonly byte[] StaticSalt =
    [
        233, 135, 9, 179, 31, 107, 55, 87, 204, 145, 192, 69, 43, 164, 117, 210, 220, 182, 196, 196, 24, 82, 147, 122,
        140, 120, 44, 26, 40, 176, 234, 251, 33, 12, 91, 3, 54, 140, 22, 194, 21, 72, 111, 40, 149, 21, 117, 151, 201,
        205, 23, 69, 1, 102, 140, 250, 39, 7, 2, 166, 181, 253, 97, 230
    ]; // "6YcJsx9rN1fMkcBFK6R10ty2xMQYUpN6jHgsGiiw6vshDFsDNowWwhVIbyiVFXWXyc0XRQFmjPonBwKmtf1h5g==" in Base64
}