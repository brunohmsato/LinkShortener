namespace LinkShortener.Application.Contracts;

public class ErrorLogDto
{
    public short IdAplicacao { get; set; }
    public string? MensagemErro { get; set; }
    public string? StackTrace { get; set; }
    public string? TipoErro { get; set; }
    public string? CodigoErro { get; set; }
    public short? IdUsuario { get; set; }
    public string? IpUsuario { get; set; }
    public string? RequestPath { get; set; }
    public string? ClasseErro { get; set; }
    public short? LinhaErro { get; set; }
    public short? IdUnidadeNegocio { get; set; }
}