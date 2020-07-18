using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Entities
{
    public class Review : IId
    {
        public int Id { get; set; }
        public string Remark { get; set; }
        [Range(1, 5)]
        public int Mark { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public string UserId { get; set; }
        public IdentityUser User { get; set; }
    }
}
