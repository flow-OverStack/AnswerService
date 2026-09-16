using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace AnswerService.Api.Controllers.Base;

/// <inheritdoc />
[Consumes(MediaTypeNames.Application.Json)]
[Route("api/v{version:apiVersion}/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[ApiController]
public class BaseController : ControllerBase;