using System.Collections.Generic;

namespace MoviesAPI.DTOs
{
    public class MovieDetailsDTO : MovieDTO
    {
        public List<GenderDTO> Genders { get; set; }
        public List<ActorMovieDetailDTO> Actors { get; set; }
    }
}
