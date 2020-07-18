using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Movie : IId, IImage
    {
        public int Id { get; set; }
        [Required]
        [StringLength(300)]
        public string Title { get; set; }
        public bool InTheaters { get; set; }
        public DateTime PremireDate { get; set; }
        public string Image { get; set; }

        public List<MoviesActors> MoviesActors { get; set; }
        public List<MoviesGenders> MoviesGenders { get; set; }
        public List<MoviesCinemas> MoviesCinemas { get; set; }
    }
}
