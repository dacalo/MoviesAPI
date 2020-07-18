using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MoviesAPI.Validations
{
    public class SizeFileValidation : ValidationAttribute
    {
        private readonly int _sizeMaxInMegaByts;

        public SizeFileValidation(int sizeMaxInMegaByts)
        {
            _sizeMaxInMegaByts = sizeMaxInMegaByts;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value == null)
                return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
                return ValidationResult.Success;
            
            if(formFile.Length > _sizeMaxInMegaByts * 1024 * 1024)
                return new ValidationResult($"El peso del archivo no debe ser mayor a {_sizeMaxInMegaByts}mb");

            return ValidationResult.Success;
        }
    }
}
