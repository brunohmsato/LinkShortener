using LinkShortener.Application.Contracts;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace LinkShortener.Presentation.Handlers;

public class GlobalExceptionHandler(IProblemDetailsService problemDetailsService, IConfiguration configuration) : IExceptionHandler
{
    private readonly IConfiguration _configuration = configuration;

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = new ProblemDetails
        {
            Status = exception switch
            {
                ArgumentNullException       => StatusCodes.Status400BadRequest,             // Parâmetro nulo ou inválido
                ArgumentException           => StatusCodes.Status400BadRequest,             // Argumento inválido
                KeyNotFoundException        => StatusCodes.Status404NotFound,               // Registro não encontrado
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,           // Acesso não autorizado
                NotImplementedException     => StatusCodes.Status501NotImplemented,         // Funcionalidade não implementada
                OperationCanceledException  => StatusCodes.Status408RequestTimeout,         // Operação cancelada
                TimeoutException            => StatusCodes.Status504GatewayTimeout,         // Tempo limite de requisição expirado
                HttpRequestException        => StatusCodes.Status503ServiceUnavailable,     // Erros ao chamar serviços externos
                _                           => StatusCodes.Status500InternalServerError     // Erro interno do servidor
            },
            Title = exception switch
            {
                ArgumentNullException       => "Requisição inválida: Parâmetro obrigatório ausente.",
                ArgumentException           => "Requisição inválida: Argumento fornecido não é válido.",
                KeyNotFoundException        => "Recurso não encontrado.",
                UnauthorizedAccessException => "Acesso negado.",
                NotImplementedException     => "Funcionalidade ainda não implementada.",
                OperationCanceledException  => "A operação foi cancelada pelo cliente ou pelo servidor.",
                TimeoutException            => "A requisição demorou muito tempo para ser processada.",
                HttpRequestException        => "Falha ao se comunicar com um serviço externo.",
                _                           => "Ocorreu um erro inesperado no servidor."
            },
            Type = exception.GetType().FullName, // Nome completo da exceção
            Detail = exception.Message,
            Instance = httpContext.Request.Path, // Caminho da requisição
            Extensions =
            {
                ["traceId"] = httpContext.TraceIdentifier,      // ID da requisição para rastreamento
                ["timestamp"] = DateTime.UtcNow.ToString("o"),  // Timestamp no formato ISO 8601
                ["method"] = httpContext.Request.Method,        // Método HTTP usado na requisição
                ["stackTrace"] = exception.StackTrace,          // StackTrace para depuração
            }
        };

        var userId = httpContext.User.FindFirst("IdUsuario")?.Value;
        var userUnidade = httpContext.User.FindFirst("IdUnidadeNegocio")?.Value;

        // Obtendo a primeira linha da stack trace
        string? primeiraLinhaStackTrace = exception.StackTrace?
            .Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault()?.Trim();

        // Extraindo o número da linha do erro usando regex
        short? numeroLinhaErro = null;
        if (primeiraLinhaStackTrace != null)
        {
            var match = Regex.Match(primeiraLinhaStackTrace, @":line (\d+)");
            if (match.Success && short.TryParse(match.Groups[1].Value, out var linha))
                numeroLinhaErro = linha;
        }

        _ = new ErrorLogDto
        {
            IdAplicacao = 1,
            MensagemErro = exception.Message,
            StackTrace = exception.StackTrace,
            TipoErro = exception.GetType().Name,
            CodigoErro = problemDetails.Status.ToString(),
            IdUsuario = short.TryParse(userId, out var idUduario) ? idUduario : null,
            IpUsuario = httpContext.Connection.RemoteIpAddress?.ToString(),
            RequestPath = httpContext.Request.Method + " " + httpContext.Request.Path,
            ClasseErro = problemDetails.Instance,
            LinhaErro = numeroLinhaErro,
            IdUnidadeNegocio = short.TryParse(userUnidade, out var idUnidade) ? idUnidade : null,
        };

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            Exception = exception,
            HttpContext = httpContext,
            ProblemDetails = problemDetails
        });
    }
}