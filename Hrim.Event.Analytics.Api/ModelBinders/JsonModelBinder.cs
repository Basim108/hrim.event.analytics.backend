using Hrim.Event.Analytics.Api.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Hrim.Event.Analytics.Api.ModelBinders;

/// <summary>
///     As json deserialization is not used with a query string, this model binder enforce json deserialization.
/// </summary>
public class JsonModelBinder<TResult>: IModelBinder
{
    private readonly ILogger<JsonModelBinder<TResult>> _logger;

    /// <summary> </summary>
    public JsonModelBinder(ILogger<JsonModelBinder<TResult>> logger) { _logger = logger; }

    /// <inheritdoc />
    public Task BindModelAsync(ModelBindingContext bindingContext) {
        var rawData = bindingContext.ValueProvider
                                    .GetValue(key: bindingContext.ModelName)
                                    .FirstValue;
        var jsonSettings = JsonSettingsFactory.Get();
        rawData = JsonConvert.SerializeObject(value: rawData, settings: jsonSettings);
        try {
            var result = JsonConvert.DeserializeObject<TResult>(value: rawData, settings: jsonSettings);
            bindingContext.Result = ModelBindingResult.Success(model: result);
        }
        catch (JsonSerializationException ex) {
            // "failed" result is set by default
            _logger.LogError(message: ApiLogs.JSON_MODEL_BINDER_DESERIALIZATION_ERROR,
                             bindingContext.ModelName,
                             ex.Message,
                             ex.StackTrace ?? "");
        }

        return Task.CompletedTask;
    }
}