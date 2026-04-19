namespace Domain.Common;

public record Error(string Id, ErrorType Type, string Description);


public static class Errors
{
    public static Error AccountNotFound { get; } = new("AccountNotFound", ErrorType.NotFound, "Account not found.");
    public static Error InsufficientFunds { get; } = new("InsufficientFunds", ErrorType.Validation, "Validation error");
}