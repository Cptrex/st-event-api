﻿using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ST.Events.API.Filters;

public class StValidationFilter : Attribute, IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        // only handle ValidationException
        if (context.Exception is not ValidationException) return;
        var exceptionMessage = context.Exception.Message;

        context.ExceptionHandled = true;

        context.Result = new ContentResult
        {
            Content = $"Возникло исключение: \n {exceptionMessage}"
        };
    }
}