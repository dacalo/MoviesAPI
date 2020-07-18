using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MoviesAPI.Helpers;
using MoviesAPI.Validations;
using System.Collections.Generic;

namespace MoviesAPI.DTOs
{
    public class MovieCreateDTO : MoviePatchDTO
    {
        [SizeFileValidation(sizeMaxInMegaByts:4)]
        [TypeFileValidation(groupTypeFile: GroupTypeFile.Image)]
        public IFormFile Poster { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GendersId { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorsMoviesCreateDTO>>))]
        public List<ActorsMoviesCreateDTO> Actors { get; set; }
    }
}
