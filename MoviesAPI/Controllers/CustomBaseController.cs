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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomBaseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IFilesStore _filesStore;
        
        public CustomBaseController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public CustomBaseController(ApplicationDbContext context, IMapper mapper, IFilesStore filesStore)
        {
            _context = context;
            _mapper = mapper;
            _filesStore = filesStore;
        }

        protected async Task<List<TDTO>> Get<TEntity, TDTO>() where TEntity : class
        {
            var entities = await _context.Set<TEntity>().AsNoTracking().ToListAsync();
            return _mapper.Map<List<TDTO>>(entities);
        }

        protected async Task<ActionResult<List<TDTO>>>Get<TEntity, TDTO>(PaginationDTO paginationDTO) where TEntity : class
        {
            var queryable = _context.Set<TEntity>().AsQueryable();
            return await Get<TEntity, TDTO>(paginationDTO, queryable);
        }

        protected async Task<ActionResult<List<TDTO>>> Get<TEntity, TDTO>(PaginationDTO paginationDTO, IQueryable<TEntity> queryable) where TEntity : class
        {
            await HttpContext.InsertParametersPagination(queryable, paginationDTO.RecordsPage);
            var entities = await queryable.Paginate(paginationDTO).ToListAsync();
            return _mapper.Map<List<TDTO>>(entities);
        }

        protected async Task<ActionResult<TDTO>> Get<TEntity, TDTO>(int id) where TEntity : class, IId
        {
            var entity = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return NotFound();

            return _mapper.Map<TDTO>(entity);
        }

        protected async Task<ActionResult> Post<TCreate, TEntity, TReading>(TCreate createDTO, string namePath) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(createDTO);
            _context.Add(entity);
            await _context.SaveChangesAsync();
            var dtoReading = _mapper.Map<TReading>(entity);
            return new CreatedAtRouteResult(namePath, new { id = entity.Id }, dtoReading);
        }

        protected async Task<ActionResult> Put<TCreate, TEntity>(int id, TCreate createDTO) where TEntity : class, IId
        {
            var entity = _mapper.Map<TEntity>(createDTO);
            entity.Id = id;
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult>Patch<TEntity, TDTO>(int id, JsonPatchDocument<TDTO> patchDocument) where TDTO : class where TEntity : class, IId
        {
            if (patchDocument == null)
                return BadRequest();

            var entity = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            if (entity == null)
                return NotFound();

            var entityDTO = _mapper.Map<TDTO>(entity);

            patchDocument.ApplyTo(entityDTO, ModelState);

            var isValid = TryValidateModel(entityDTO);

            if (!isValid)
                return BadRequest(ModelState);

            _mapper.Map(entityDTO, entity);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        protected async Task<ActionResult>Delete<TEntity>(int id) where TEntity : class, IId, new()
        {
            var existEntity = await _context.Set<TEntity>().AsNoTracking().AnyAsync(x => x.Id == id);
            if (!existEntity)
                return NotFound();

            _context.Remove(new TEntity { Id = id });
            await _context.SaveChangesAsync();

            return NoContent();
        }
        protected async Task<ActionResult> DeleteWithImage<TEntity>(int id, string container) where TEntity : class, IId, IImage, new()
        {
            var entity = await _context.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (entity == null)
                return NotFound();
            
            await _filesStore.DeleteFile(entity.Image, container);
            _context.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

    }
}
