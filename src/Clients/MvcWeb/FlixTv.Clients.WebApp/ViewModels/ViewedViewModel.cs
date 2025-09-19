using FlixTv.Common.Models.ResponseModels.ViewData;

namespace FlixTv.Clients.WebApp.ViewModels
{
    public class ViewedViewModel
    {
        public IList<GetViewDataQueryResponse> ViewDatas { get; set; } = new List<GetViewDataQueryResponse>();
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages => (int)Math.Ceiling((double)Math.Max(0, TotalCount) / Math.Max(1, PageSize));
    }
}
