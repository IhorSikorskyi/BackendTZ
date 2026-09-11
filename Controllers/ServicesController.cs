using BackendTZ.DTOs.Requests;
using BackendTZ.DTOs.Responses;
using BackendTZ.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendTZ.Controllers;

/// <summary>
/// Controller responsible for handling service management operations such as creating, updating, deleting, and retrieving service details.
/// </summary>
/// <param name="serviceManagementService">The service management service used to perform operations on services.</param>
[ApiController]
[Route("api/services")]
public class ServicesController(IServiceManagementService serviceManagementService) : BaseController
{
    /// <summary>
    /// Creates a new service with the provided details.
    /// </summary>
    /// <param name="request">The details of the service to be created.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the created service.</returns>
    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceItemResponse>> CreateServiceAsync(CreateServiceRequest request, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.CreateServiceAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing service with the provided details.
    /// </summary>
    /// <param name="id">The ID of the service to be updated.</param>
    /// <param name="request">The details of the service to be updated.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the updated service.</returns>
    [HttpPut("update/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceItemResponse>> UpdateServiceAsync(Guid id, UpdateServiceRequest request, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.UpdateServiceAsync(id, request, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Deletes an existing service by its ID.
    /// </summary>
    /// <param name="id">The ID of the service to be deleted.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the deleted service.</returns>
    [HttpDelete("delete/{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServiceItemResponse>> DeleteServiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.DeleteServiceAsync(id, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves the details of a service by its ID.
    /// </summary>
    /// <param name="id">The ID of the service to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The details of the specified service.</returns>
    [HttpGet("get/{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ServiceItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServiceItemResponse>> GetServiceAsync(Guid id, CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.GetServiceAsync(id, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves a list of all available services.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all available services.</returns>
    [HttpGet("get-all")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IList<ServiceItemResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IList<ServiceItemResponse>>> GetAllServicesAsync(CancellationToken cancellationToken)
    {
        var response = await serviceManagementService.GetAllServicesAsync(cancellationToken);
        return Ok(response);
    }
}