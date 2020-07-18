namespace MoviesAPI.DTOs
{
    public class PaginationDTO
    {
        private int _recordsPage = 10;
        private readonly int _maxRecordsPage = 50;

        public int Page { get; set; } = 1;

        public int RecordsPage 
        { 
            get => _recordsPage;
            set
            {
                _recordsPage = (value > _maxRecordsPage) ? _maxRecordsPage : value;
            }
        }

    }
}
