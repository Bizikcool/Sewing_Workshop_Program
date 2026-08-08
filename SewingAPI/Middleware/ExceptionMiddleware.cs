using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using SewingAPI.Domain.Exceptions;
using DomainValidationException = SewingAPI.Domain.Exceptions.ValidationException;

namespace SewingAPI.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainValidationException ex)
            {
                await WriteProblemAsync(context, StatusCodes.Status400BadRequest, ex.Message, ex.Errors);
            }
            catch (NotFoundException ex)
            {
                await WriteProblemAsync(context, StatusCodes.Status404NotFound, ex.Message);
            }
            catch (BusinessException ex)
            {
                await WriteProblemAsync(context, StatusCodes.Status409Conflict, ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                await WriteProblemAsync(context, StatusCodes.Status401Unauthorized, ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {ExceptionType} - {ExceptionMessage}", ex.GetType().Name, ex.Message);

                var detail = _environment.IsDevelopment()
                    ? $"Внутренняя ошибка сервера: {ex.Message}\n{ex.StackTrace}"
                    : "Внутренняя ошибка сервера.";

                await WriteProblemAsync(context, StatusCodes.Status500InternalServerError, detail);
            }
        }

        private static async Task WriteProblemAsync(HttpContext context, int statusCode, string detail, IReadOnlyList<string>? errors = null)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Detail = detail,
                Instance = context.Request.Path
            };

            if (errors is not null)
            {
                problem.Extensions["errors"] = errors;
            }

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
