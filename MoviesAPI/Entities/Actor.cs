using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Actor : IId, IImage
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public string Image { get; set; }
        public List<MoviesActors> MoviesActors { get; set; }
    }
}
