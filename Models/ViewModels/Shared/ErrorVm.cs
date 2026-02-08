namespace MyRent.Models.ViewModels.Shared;

public sealed class ErrorVm
{
    public string Title { get; init; } = "Something went wrong";
    public string Message { get; init; } = "Please try again.";
    public int? StatusCode { get; init; }
}