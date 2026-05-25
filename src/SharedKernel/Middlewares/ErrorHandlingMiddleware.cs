using FluentValidation;
using Microsoft.AspNetCore.Http;
using SharedKernel.Exceptions;
using System.Text.Json;

namespace SharedKernel.Middlewares;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DomainException ex)
        {
            await HandleDomainExceptionAsync(context, ex);
        }
        catch (ValidationException ex)
        {
            await HandleValidationExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            await HandleUnhandledExceptionAsync(context, ex);
        }
    }

    private static Task HandleDomainExceptionAsync(HttpContext context, DomainException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

        var result = JsonSerializer.Serialize(new
        {
            error = exception.Code,
            message = exception.Message
        });

        return context.Response.WriteAsync(result);
    }

    private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

        var errors = exception.Errors.Select(e => new
        {
            field = e.PropertyName,
            message = e.ErrorMessage
        }).ToList();

        var result = JsonSerializer.Serialize(new
        {
            error = "VALIDATION_FAILED",
            message = "Ocorreram um ou mais erros de validação.",
            details = errors
        });

        return context.Response.WriteAsync(result);
    }

    private static Task HandleUnhandledExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var result = JsonSerializer.Serialize(new
        {
            error = "INTERNAL_SERVER_ERROR",
            message = exception.Message
        });

        return context.Response.WriteAsync(result);
    }
}
