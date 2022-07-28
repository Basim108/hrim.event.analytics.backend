using Microsoft.AspNetCore.Mvc;

namespace Hrim.Event.Analytics.Api.V1.Controllers;

[ApiController]
[Route("v1/event")]
public class EventController: ControllerBase {
    [HttpGet]
    public Task<string> GetAllAsync(CancellationToken cancellation)
        => Task.FromResult("hello world!");
}