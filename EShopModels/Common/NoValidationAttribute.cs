using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EShopModels.Common
{
    public class NoValidationAttribute : ValidationAttribute
    { 
        protected override ValidationResult? IsValid(object? value, ValidationContext context)
        {
            //if (context.Items.Count != 0)
            //{
            //    // Get the properties that contain the [NoValidationAttribute]
            //    var properties = context.Items.First().Value.GetType()
            //        .GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(a =>
            //            a.GetCustomAttributes(true).OfType<NoValidationAttribute>().Any());

            //    // For each of the properties remove the modelstate errors.
            //    foreach (var property in properties)
            //    {
            //        foreach (var key in context.ModelState.Keys.Where(k =>
            //            k.StartsWith($"{property.Name}["))) // Item index is part of key [
            //        {
            //            context.ModelState.Remove(key);
            //        }
            //    }
            //}

            return base.IsValid(value, context);
        }
         
        public override bool IsValid(object? value)
        {
            return base.IsValid(value);
        }
    }

    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //NoValidationAttribute.RemoveValidation(context);
            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }
        } 
    }
}
