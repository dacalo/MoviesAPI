namespace MoviesAPI.DTOs
{
    public class FilterMoviesDTO
    {
        public int Page { get; set; } = 1;
        public int RecordsPage { get; set; } = 10;

        public PaginationDTO Pagination
        {
            get
            {
                return new PaginationDTO() { Page = Page, RecordsPage = RecordsPage };
            }
        }

        public string Title { get; set; }
        public int GenderId { get; set; }
        public bool InTheaters { get; set; }
        public bool NextPremieres { get; set; }
        public string FieldOrder { get; set; }
        public bool OrderAscending { get; set; }
    }
}
