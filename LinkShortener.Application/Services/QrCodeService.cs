using LinkShortener.Application.Interfaces;
using LinkShortener.Domain.Interfaces;
using QRCoder;

namespace LinkShortener.Application.Services;

public class QrCodeService(ILinkRepository links) : IQrCodeService
{
    public byte[] Generate(string shortUrl)
    {
        using var generator = new QRCodeGenerator();
        using var data = generator.CreateQrCode(shortUrl, QRCodeGenerator.ECCLevel.Q);
        using var qr = new PngByteQRCode(data);

        return qr.GetGraphic(20);
    }
}