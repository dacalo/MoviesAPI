using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class MovieExistAttribute : Attribute, IAsyncResourceFilter
    {
        private readonly ApplicationDbContext _context;

        public MovieExistAttribute(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var movieIdObject = context.HttpContext.Request.RouteValues["movieId"];
            if (movieIdObject == null)
                return;

            var movieId = int.Parse(movieIdObject.ToString());

            var existMovie = await _context.Movies.AnyAsync(m => m.Id == movieId);
            if (!existMovie)
                context.Result = new NotFoundResult();
            else
                await next();
        }
    }
}
