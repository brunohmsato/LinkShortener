namespace LinkShortener.Application.Interfaces;

public interface IQrCodeService
{
    byte[] Generate(string shortUrl);
}