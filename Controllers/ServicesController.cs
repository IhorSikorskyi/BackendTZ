using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController(IServiceManagementService serviceManagementService) : BaseController
{
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceItemResponse>> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.CreateServiceAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPut("update/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceItemResponse>> UpdateServiceAsync(Guid id, UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.UpdateServiceAsync(id, request, cancellationToken);
        return Ok(response);
    }

    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceItemResponse>> DeleteServiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.DeleteServiceAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpGet("get/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceItemResponse>> GetServiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.GetServiceAsync(id, cancellationToken);
        return Ok(response);
    }

    [HttpGet("get-all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IList<ServiceItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<ServiceItemResponse>>> GetAllServicesAsync(CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.GetAllServicesAsync(cancellationToken);
        return Ok(response);
    }
}