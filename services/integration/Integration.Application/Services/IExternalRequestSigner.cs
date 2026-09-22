namespace Integration.Application.Services;

public interface IExternalRequestSigner
{
    bool VerifySignature(string payload, string secret, string signature, string algorithm = "HMAC-SHA256");
    string GenerateSignature(string payload, string secret, string algorithm = "HMAC-SHA256");
}
