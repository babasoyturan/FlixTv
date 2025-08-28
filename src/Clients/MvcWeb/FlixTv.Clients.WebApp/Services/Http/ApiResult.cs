using System.Net;

namespace FlixTv.Clients.WebApp.Services.Http
{
    public sealed class ApiResult<T>
    {
        public bool IsSuccess { get; init; }
        public T? Data { get; init; }
        public List<string> Errors { get; init; } = new();
        public HttpStatusCode StatusCode { get; init; }

        public static ApiResult<T> Success(T? data, HttpStatusCode code)
            => new() { IsSuccess = true, Data = data, StatusCode = code };

        public static ApiResult<T> Fail(IEnumerable<string> errors, HttpStatusCode code)
            => new() { IsSuccess = false, Errors = errors.ToList(), StatusCode = code };
    }
}
