using System.Collections.Generic;

namespace MoviesAPI.DTOs
{
    public class MoviesIndexDTO
    {
        public List<MovieDTO> FuturePremieres { get; set; }
        public List<MovieDTO> InTheaters { get; set; }
    }
}
