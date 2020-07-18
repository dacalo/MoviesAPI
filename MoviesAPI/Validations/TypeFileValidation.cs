using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MoviesAPI.Validations
{
    public class TypeFileValidation : ValidationAttribute
    {
        private readonly string[] _validTypes;

        public TypeFileValidation(string[] validTypes)
        {
            _validTypes = validTypes;
        }

        public TypeFileValidation(GroupTypeFile groupTypeFile)
        {
            if(groupTypeFile == GroupTypeFile.Image)
            {
                _validTypes = new string[] { "image/jpeg", "image/png", "image/gif" };
            }
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            IFormFile formFile = value as IFormFile;

            if (formFile == null)
                return ValidationResult.Success;

            if (!_validTypes.Contains(formFile.ContentType))
                return new ValidationResult($"El tipo de archivo debe ser uno de los siguientes: {string.Join(", ", _validTypes)}");

            return ValidationResult.Success;
        }

    }
}
