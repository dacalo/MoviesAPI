using System;

namespace MoviesAPI.DTOs
{
    public class MovieDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool InTheaters { get; set; }
        public DateTime PremireDate { get; set; }
        public string Poster { get; set; } 
    }
}
