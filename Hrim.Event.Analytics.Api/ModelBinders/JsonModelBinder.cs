using Hrim.Event.Analytics.Api.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.ModelBinders; 

/// <summary>
/// As json deserialization is not used with a query string, this model binder enforce json deserialization. 
/// </summary>
public class JsonModelBinder<TResult>: IModelBinder {
    private readonly ILogger<JsonModelBinder<TResult>> _logger;

    /// <summary> </summary>
    public JsonModelBinder(ILogger<JsonModelBinder<TResult>> logger) {
        _logger = logger;
    }
    
    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var rawData = bindingContext.ValueProvider
                                    .GetValue(bindingContext.ModelName)
                                    .FirstValue;
        var jsonSettings = JsonSettingsFactory.Get();
        rawData = JsonConvert.SerializeObject(rawData, jsonSettings); // turns value to valid json
        try
        {
            var result = JsonConvert.DeserializeObject<TResult>(rawData, jsonSettings); //manually deserializing value
            bindingContext.Result = ModelBindingResult.Success(result);
        }
        catch (JsonSerializationException ex)
        {
            // "failed" result is set by default
            _logger.LogError(ApiLogs.JSON_MODEL_BINDER_DESERIALIZATION_ERROR, bindingContext.ModelName, ex.Message, ex.StackTrace ?? "");
        }
        return Task.CompletedTask;
    }
}