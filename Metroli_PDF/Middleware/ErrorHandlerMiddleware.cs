using Domain.Models;
using System.Net;
using Newtonsoft.Json;

namespace Metroli_PDF.Middleware;

public sealed class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ErrorHandler ex)
        {
            await ErrorHandlerAsync(context, ex);
        }
        //catch (Exception ex) // Captura de cualquier otra excepción
        //{
        //    await ErrorHandlerAsync(context, ex);
        //}
    }

    private async Task ErrorHandlerAsync(HttpContext context, Exception ex)
    {
        string message = null;

        context.Response.ContentType = "application/json";

        switch (ex)
        {
            case ErrorHandler eh:

                context.Response.StatusCode = (int)eh.Code;

                message = eh.Message;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(MessageResult<object>.Of(message, eh.ExceptionData, eh.Status, eh.InternalResponse)));

                break;

            case Exception e:

                message = string.IsNullOrWhiteSpace(e.Message) ? "Error desconocido" : e.Message;

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                await context.Response.WriteAsync(JsonConvert.SerializeObject(MessageResult<object>.Of(message, ex.Data, context.Response.StatusCode)));

                break;
        }

        //await context.Response.WriteAsync(message);
    }
}
