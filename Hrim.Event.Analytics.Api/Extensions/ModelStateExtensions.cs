using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Hrim.Event.Analytics.Api.Extensions;

/// <summary> </summary>
public static class ModelStateExtensions {
    /// <summary>
    /// Adds fluent validation errors to the ControllerBase's ModelState 
    /// </summary>
    /// <param name="modelState"></param>
    /// <param name="validationResult"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public static void AddFluentErrors(this ModelStateDictionary modelState, ValidationResult validationResult) {
        if (modelState == null)
            throw new ArgumentNullException(nameof(modelState));
        if (validationResult == null)
            throw new ArgumentNullException(nameof(validationResult));

        foreach (var error in validationResult.Errors) {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }
    }
}