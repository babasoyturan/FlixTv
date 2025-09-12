namespace FlixTv.Clients.WebApp.ViewModels
{
    public class MoviesFilter
    {
        public string? SearchText { get; set; }
        public int? MinReleaseYear { get; set; }
        public int? MaxReleaseYear { get; set; }
        public short? AgeLimitation { get; set; }
        public int? MinDuration { get; set; }
        public int? MaxDuration { get; set; }
        public byte? MinRating { get; set; }
        public byte? MaxRating { get; set; }
        public List<string> Categories { get; set; } = new();

        // sort: "rating" | "popular" | "releaseYear" | "viewCount" | "createdDate"
        public string? OrderBy { get; set; } = "rating";

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 15;
    }
}
