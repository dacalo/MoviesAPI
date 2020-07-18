namespace MoviesAPI.DTOs
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public string Remark { get; set; }
        public int Mark { get; set; }
        public int MovieId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}
