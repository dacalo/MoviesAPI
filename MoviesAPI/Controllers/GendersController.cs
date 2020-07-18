using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Data;
using MoviesAPI.DTOs;
using MoviesAPI.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GendersController : CustomBaseController
    {
        public GendersController(ApplicationDbContext context, IMapper mapper): base(context, mapper)
        {
        }

        [HttpGet]
        public async Task<ActionResult<List<GenderDTO>>> Get()
        {
            return await Get<Gender, GenderDTO>();
        }

        [HttpGet("{id:int}", Name = "GetGender")]
        public async Task<ActionResult<GenderDTO>> Get(int id)
        {
            return await Get<Gender, GenderDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult>Post([FromBody]GenderCreateDTO genderCreateDTO)
        {
            return await Post<GenderCreateDTO, Gender, GenderDTO>(genderCreateDTO, "GetGender");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult>Put(int id, [FromBody] GenderCreateDTO genderCreateDTO)
        {
            return await Put<GenderCreateDTO, Gender>(id, genderCreateDTO);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            return await Delete<Gender>(id);
        }
    }
}
