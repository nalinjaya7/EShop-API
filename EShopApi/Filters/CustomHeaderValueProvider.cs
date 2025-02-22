using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EShopApi.Filters
{
    public class CustomModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        { 
           return Task.CompletedTask;
        }
    }
}