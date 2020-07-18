using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Helpers;
using MoviesAPI.Services;
using System.Linq.Dynamic.Core;
using Microsoft.Extensions.Logging;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFilesStore _filesStore;
        private readonly ILogger<MoviesController> _logger;
        private readonly string _container = "movies";

        public MoviesController(
            ApplicationDbContext context,
            IMapper mapper,
            IFilesStore filesStore,
            ILogger<MoviesController> logger) : base(context, mapper, filesStore)
        {
            _context = context;
            _mapper = mapper;
            _filesStore = filesStore;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<MoviesIndexDTO>> Get()
        {
            var top = 5;
            var today = DateTime.Today;

            var nextPremieres = await _context.Movies
                .Where(m => m.PremireDate > today)
                .OrderBy(m => m.PremireDate)
                .Take(top)
                .ToListAsync();

            var inTheaters = await _context.Movies
                .Where(m => m.InTheaters)
                .Take(top)
                .ToListAsync();

            var result = new MoviesIndexDTO();
            result.FuturePremieres = _mapper.Map<List<MovieDTO>>(nextPremieres);
            result.InTheaters = _mapper.Map<List<MovieDTO>>(inTheaters);

            return result;
        }

        [HttpGet("filter")]
        public async Task<ActionResult<List<MovieDTO>>> Filters([FromQuery] FilterMoviesDTO filterMoviesDTO)
        {
            var moviesQueryable = _context.Movies.AsQueryable();
            if (!string.IsNullOrEmpty(filterMoviesDTO.Title))
                moviesQueryable = moviesQueryable.Where(m => m.Title.Contains(filterMoviesDTO.Title));

            if (filterMoviesDTO.InTheaters)
                moviesQueryable = moviesQueryable.Where(m => m.InTheaters);
            else
                //moviesQueryable = moviesQueryable.Where(m => m.InTheaters == false);

            if (filterMoviesDTO.NextPremieres)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(m => m.PremireDate > today);
            }

            if (filterMoviesDTO.GenderId != 0)
                moviesQueryable = moviesQueryable
                    .Where(m => m.MoviesGenders
                    .Select(mg => mg.GenderId)
                    .Contains(filterMoviesDTO.GenderId));

            if(!string.IsNullOrEmpty(filterMoviesDTO.FieldOrder))
            {
                //if(filterMoviesDTO.FieldOrder == "Title")
                //{
                //    if (filterMoviesDTO.OrderAscendant)
                //        moviesQueryable.OrderBy(x => x.Title);
                //    else
                //        moviesQueryable.OrderByDescending(x => x.Title);
                //}
                var typeOrder = (filterMoviesDTO.OrderAscending) ? "ascending" : "descending";

                try
                {
                    moviesQueryable = moviesQueryable.OrderBy($"{filterMoviesDTO.FieldOrder} {typeOrder}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex.Message, ex);
                }

            }

            await HttpContext.InsertParametersPagination(moviesQueryable, filterMoviesDTO.RecordsPage);
            var movies = await moviesQueryable.Paginate(filterMoviesDTO.Pagination).ToListAsync();
            return _mapper.Map<List<MovieDTO>>(movies);
        }


        [HttpGet("{id}", Name = "GetMovie")]
        public async Task<ActionResult<MovieDetailsDTO>>Get(int id)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors).ThenInclude(x => x.Actor)
                .Include(x => x.MoviesGenders).ThenInclude(x => x.Gender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (movie == null)
                return NotFound();

            movie.MoviesActors = movie.MoviesActors.OrderBy(x => x.Order).ToList();

            return _mapper.Map<MovieDetailsDTO>(movie);
        }

        [HttpPost]
        public async Task<ActionResult>Post([FromForm] MovieCreateDTO movieCreateDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreateDTO);

            if (movieCreateDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreateDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreateDTO.Poster.FileName);
                    movie.Image = await _filesStore.SaveFile(content, extension, _container, movieCreateDTO.Poster.ContentType);
                }
            }

            AssignActorsOrder(movie);
            _context.Movies.Add(movie);
            await _context.SaveChangesAsync();
            var dto = _mapper.Map<MovieDTO>(movie);
            return new CreatedAtRouteResult("GetMovie", new { id = movie.Id }, dto);
        }


        private void AssignActorsOrder(Movie movie)
        {
            if(movie.MoviesActors != null)
            {
                for (int i = 0; i < movie.MoviesActors.Count; i++)
                {
                    movie.MoviesActors[i].Order = i;
                }
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Put(int id, [FromForm] MovieCreateDTO movieCreateDTO)
        {
            var movie = await _context.Movies
                .Include(x => x.MoviesActors)
                .Include(x=> x.MoviesGenders)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (movie == null)
                return NotFound();
            movie = _mapper.Map(movieCreateDTO, movie);

            if (movieCreateDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await movieCreateDTO.Poster.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(movieCreateDTO.Poster.FileName);
                    movie.Image = await _filesStore.EditFile(content, extension, _container, movie.Image, movieCreateDTO.Poster.ContentType);
                }
            }
            AssignActorsOrder(movie);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<MoviePatchDTO> patchDocument)
        {
            return await Patch<Movie, MoviePatchDTO>(id, patchDocument);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return await DeleteWithImage<Movie>(id, _container);
        }
    }
}
