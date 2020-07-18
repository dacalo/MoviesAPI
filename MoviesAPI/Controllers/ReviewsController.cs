using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/Movies/{MovieId:int}/Reviews")]
    [ApiController]
    [ServiceFilter(typeof(MovieExistAttribute))]
    public class ReviewsController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ReviewsController(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            this._context = context;
            this._mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<ReviewDTO>>>Get(int movieId, [FromQuery]PaginationDTO paginationDTO)
        {
            var queryable = _context.Reviews.Include(x => x.User).AsQueryable();
            queryable = queryable.Where(x => x.MovieId == movieId);
            return await Get<Review, ReviewDTO>(paginationDTO, queryable);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult>Post(int movieId, [FromBody]ReviewCreateDTO reviewCreateDTO)
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            var reviewExist = await _context.Reviews.AnyAsync(x => x.MovieId == movieId && x.UserId == userId);

            if (reviewExist)
                return BadRequest("El usuario ya ha escrito un review de ésta película");

            var review = _mapper.Map<Review>(reviewCreateDTO);
            
            review.UserId = userId;
            review.MovieId = movieId;

            _context.Add(review);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }


        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult>Put(int reviewId, [FromBody]ReviewCreateDTO reviewCreateDTO)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            
            if (review == null) return NotFound();

            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;

            if (review.UserId != userId) return BadRequest("No tiene permisos de editar este review");

            review = _mapper.Map(reviewCreateDTO, review);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult>Delete(int reviewId)
        {
            var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
            if (review == null) return NotFound();
            var userId = HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value;
            if (review.UserId != userId) return Forbid();

            _context.Remove(review);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
