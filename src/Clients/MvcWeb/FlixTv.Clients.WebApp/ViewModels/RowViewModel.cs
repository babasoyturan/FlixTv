using FlixTv.Common.Models.ResponseModels.Movies;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class RowViewModel
    {
        public string RowKey { get; set; }
        public string RowTitle { get; set; }
        public IList<GetAllMoviesQueryResponse> Movies { get; set; }
    }
}
