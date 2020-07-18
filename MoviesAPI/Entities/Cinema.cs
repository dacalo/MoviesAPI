using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Cinema : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }

        public Point Location { get; set; }
        public List<MoviesCinemas> MoviesCinemas { get; set; }
    }
}
