using Microsoft.AspNetCore.Http;
using MoviesAPI.Validations;

namespace MoviesAPI.DTOs
{
    public class ActorCreateDTO : ActorPatchDTO
    {
        [SizeFileValidation(sizeMaxInMegaByts: 4)]
        [TypeFileValidation(groupTypeFile: GroupTypeFile.Image)]
        public IFormFile Image { get; set; }
    }
}
