using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using MoviesAPI.Services;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController : CustomBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFilesStore _filesStore;
        private readonly string _container = "actors";

        public ActorsController(
            ApplicationDbContext context,
            IMapper mapper,
            IFilesStore filesStore) : base(context, mapper, filesStore)
        {
            _context = context;
            _mapper = mapper;
            _filesStore = filesStore;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery]PaginationDTO paginationDTO)
        {
            return await Get<Actor, ActorDTO>(paginationDTO);
        }

        [HttpGet("{id}", Name = "GetActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            return await Get<Actor, ActorDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult>Post([FromForm] ActorCreateDTO actorCreateDTO)
        {
            var actor = _mapper.Map<Actor>(actorCreateDTO);

            if(actorCreateDTO.Image != null)
            {
                using(var memoryStream = new MemoryStream())
                {
                    await actorCreateDTO.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreateDTO.Image.FileName);
                    actor.Image = await _filesStore.SaveFile(content, extension, _container, actorCreateDTO.Image.ContentType);
                }
            }

            _context.Actors.Add(actor);
            await _context.SaveChangesAsync();
            var dto = _mapper.Map<ActorDTO>(actor);
            return new CreatedAtRouteResult("GetActor", new { id = actor.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult>Put(int id, [FromForm] ActorCreateDTO actorCreateDTO)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(x => x.Id == id);
            if (actor == null)
                return NotFound();
            actor = _mapper.Map(actorCreateDTO, actor);

            if (actorCreateDTO.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreateDTO.Image.CopyToAsync(memoryStream);
                    var content = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreateDTO.Image.FileName);
                    actor.Image = await _filesStore.EditFile(content, extension, _container, actor.Image, actorCreateDTO.Image.ContentType);
                }
            }
            
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult>Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
        {
            return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult>Delete(int id)
        {
            return await DeleteWithImage<Actor>(id, _container);
        }
    }
}
