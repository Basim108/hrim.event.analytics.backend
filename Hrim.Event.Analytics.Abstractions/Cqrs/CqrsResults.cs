using Hrim.Event.Analytics.Abstractions.Enums;

namespace Hrim.Event.Analytics.Abstractions.Cqrs; 

/// <summary> In order to avoid throwing an necessary exception, CQRS commands and queries could use this record to provide more information about execution result</summary>
public record CqrsResult<TResult>(TResult Result, CqrsResultCode StatusCode, string? Info =null);

/// <summary> In order to avoid throwing an necessary exception, CQRS commands and queries could use this record to provide more information about execution result </summary>
public record CqrsVoidResult(CqrsResultCode StatusCode, string? Info=null);