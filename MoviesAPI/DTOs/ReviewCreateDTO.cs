using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.DTOs
{
    public class ReviewCreateDTO
    {
        public string Remark { get; set; }
        [Range(1 ,5)]
        public int Mark { get; set; }
    }
}
