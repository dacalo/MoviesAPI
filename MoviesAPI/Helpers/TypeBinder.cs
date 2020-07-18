using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MoviesAPI.Helpers
{
    public class TypeBinder<T> : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var nameProperty = bindingContext.ModelName;
            var valuesProvider = bindingContext.ValueProvider.GetValue(nameProperty);

            if(valuesProvider == ValueProviderResult.None)
                return Task.CompletedTask;

            try
            {
                var valueDeserialize = JsonConvert.DeserializeObject<T>(valuesProvider.FirstValue);
                bindingContext.Result = ModelBindingResult.Success(valueDeserialize);
            }
            catch 
            {
                bindingContext.ModelState.TryAddModelError(nameProperty, "Valor inválido para tipo List<int>");
            }

            return Task.CompletedTask;
        }
    }
}
