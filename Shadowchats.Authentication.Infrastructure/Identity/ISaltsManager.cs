namespace Shadowchats.Authentication.Infrastructure.Identity;

internal interface ISaltsManager
{
    byte[] GenerateDynamic();

    byte[] CombineStaticAndDynamicSalts(IEnumerable<byte> dynamicSalt);

    int DynamicSaltSizeInBytes { get; }
}