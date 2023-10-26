
namespace WebAPI.Application.Results.Abstract;

public interface IResult
{
    bool Success { get; }
    string Message { get; }
}
