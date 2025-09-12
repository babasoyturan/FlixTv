using FlixTv.Common.Models.ResponseModels.Movies;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class MoviesListPartialViewModel
    {
        public IList<GetAllMoviesQueryResponse> Movies { get; set; } = new List<GetAllMoviesQueryResponse>();

        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Math.Max(1, PageSize));

        // filtrləri paging linklərinə yapışdırmaq üçün (querystring)
        public Dictionary<string, string?> Query { get; set; } = new();
    }
}
