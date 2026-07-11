using System;

namespace Core;

public class ValidationException : Exception
{
    public ValidationError Errors { get; }

    public ValidationException(ValidationError errors)
        : base("Ошибка валидации")
    {
        Errors = errors;
    }
}