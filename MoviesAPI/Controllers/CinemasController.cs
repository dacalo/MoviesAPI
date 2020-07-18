using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CinemasController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly GeometryFactory _geometryFactory;

        public CinemasController(
            ApplicationDbContext context,
            IMapper mapper,
            GeometryFactory geometryFactory)
            : base(context, mapper)
        {
            _context = context;
            _geometryFactory = geometryFactory;
        }

        [HttpGet]
        public async Task<ActionResult<List<CinemaDTO>>> Get()
        {
            return await Get<Cinema, CinemaDTO>();
        }

        [HttpGet("{id:int}", Name = "GetCinema")]
        public async Task<ActionResult<CinemaDTO>>Get(int id)
        {
            return await Get<Cinema, CinemaDTO>(id);
        }

        [HttpGet("Nearby")]
        public async Task<ActionResult<List<CinemaNearDTO>>>Nearby([FromQuery]CinemaNearFilterDTO filter)
        {
            var locationUser = _geometryFactory.CreatePoint(new Coordinate(filter.Longitude, filter.Latitude));

            var cinemas = await _context.Cinemas
                .Where(x => x.Location.IsWithinDistance(locationUser, filter.DistanceKms * 1000))
                .OrderBy(x => x.Location.Distance(locationUser))
                .Select(x => new CinemaNearDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    Longitude = x.Location.X,
                    Latitude = x.Location.Y,
                    DistanceMeters = Math.Round(x.Location.Distance(locationUser))
                }).ToListAsync();

            return cinemas;
        }

        [HttpPost]
        public async Task<ActionResult>Post([FromBody]CinemaCreateDTO cinemaCreateDTO)
        {
            return await Post<CinemaCreateDTO, Cinema, CinemaDTO>(cinemaCreateDTO, "GetCinema");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult>Put(int id, [FromBody]CinemaCreateDTO cinemaCreateDTO)
        {
            return await Put<CinemaCreateDTO, Cinema>(id, cinemaCreateDTO);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult>Delete(int id)
        {
            return await Delete<Cinema>(id);
        }
    }
}
