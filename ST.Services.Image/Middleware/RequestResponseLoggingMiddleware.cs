﻿using Microsoft.IO;

namespace ST.Services.Image.Middleware;

public class RequestResponseLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;
    private readonly RecyclableMemoryStreamManager _recyclableMemoryStreamManager;

    public RequestResponseLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
    {
        _next = next;
        _logger = loggerFactory.CreateLogger<RequestResponseLoggingMiddleware>();
        _recyclableMemoryStreamManager = new RecyclableMemoryStreamManager();
    }

    // ReSharper disable once UnusedMember.Global
    public async Task Invoke(HttpContext context)
    {
        await LogRequest(context);
        await LogResponse(context);
    }

    private async Task LogRequest(HttpContext context)
    {
        context.Request.EnableBuffering();

        await using var requestStream = _recyclableMemoryStreamManager.GetStream();
        await context.Request.Body.CopyToAsync(requestStream);
        _logger.LogInformation($"[HTTP REQUEST INFORMATION]:{Environment.NewLine}" +
                               $"Schema:{context.Request.Scheme} " +
                               $"Host: {context.Request.Host} " +
                               $"Path: {context.Request.Path} " +
                               $"QueryString: {context.Request.QueryString} " +
                               $"Request Body: {ReadStreamInChunks(requestStream)}" +
                               $"{Environment.NewLine}" +
                               "__________________________________________________________________________________");

        context.Request.Body.Position = 0;
    }

    private async Task LogResponse(HttpContext context)
    {
        var originalBodyStream = context.Response.Body;

        await using var responseBody = _recyclableMemoryStreamManager.GetStream();
        context.Response.Body = responseBody;

        await _next(context);

        context.Response.Body.Seek(0, SeekOrigin.Begin);
        var text = await new StreamReader(context.Response.Body).ReadToEndAsync();
        context.Response.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation($"[HTTP RESPONSE INFORMATION]:{Environment.NewLine}" +
                               $"Schema:{context.Request.Scheme} " +
                               $"Host: {context.Request.Host} " +
                               $"Path: {context.Request.Path} " +
                               $"QueryString: {context.Request.QueryString} " +
                               $"Response Body: {text}" +
                               $"{Environment.NewLine}" +
                               "__________________________________________________________________________________");

        await responseBody.CopyToAsync(originalBodyStream);
    }

    private static string ReadStreamInChunks(Stream stream)
    {
        const int readChunkBufferLength = 4096;

        stream.Seek(0, SeekOrigin.Begin);

        using var textWriter = new StringWriter();
        using var reader = new StreamReader(stream);

        var readChunk = new char[readChunkBufferLength];
        int readChunkLength;

        do
        {
            readChunkLength = reader.ReadBlock(readChunk, 0, readChunkBufferLength);
            textWriter.Write(readChunk, 0, readChunkLength);
        } while (readChunkLength > 0);

        return textWriter.ToString();
    }
}