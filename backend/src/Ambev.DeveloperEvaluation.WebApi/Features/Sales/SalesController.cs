using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;
using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [Authorize]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public SalesController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        // <summary>
        /// Creates a new sale
        /// </summary>
        /// <param name="request">The sale creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale ID</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var validator = new CreateSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation Failed",
                    Errors = validationResult.Errors
                    .Select(error => (ValidationErrorDetail)error)
                });

            var command = _mapper.Map<CreateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return Created(message: "Sale created successfully", data: _mapper.Map<CreateSaleResponse>(response));
        }

        /// <summary>
        /// Retrieves a sale by their ID
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details with items if found</returns>
        /// or <see cref="ApiResponse"/> if not found.
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new GetSaleRequest { Id = id };
            var validator = new GetSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation Failed",
                    Errors = validationResult.Errors
                    .Select(error => (ValidationErrorDetail)error)
                });

            var query = _mapper.Map<GetSaleQuery>(request.Id);
            var response = await _mediator.Send(query, cancellationToken);

            return Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<GetSaleResponse>(response)
            });
        }

        /// <summary>
        /// Retrieves all sales.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>All sales with details</returns>
        [HttpGet()]
        [ProducesResponseType(typeof(ApiResponseWithData<List<GetSaleResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllSales(CancellationToken cancellationToken)
        {
            var query = new GetAllSalesQuery();

            var response = await _mediator.Send(query, cancellationToken);

            return Ok(data: _mapper.Map<List<GetSaleResponse>>(response), message: "All sales retrieved successfully");
        }

        /// <summary>
        /// Cancels a sale by its unique identifier.
        /// Validates the request and cancels the sale if found and valid.
        /// </summary>
        /// <param name="id">The unique identifier of the sale to cancel.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="NoContentResult"/> if the cancellation is successful,
        /// <see cref="ApiResponse"/> with validation errors if the request is invalid,
        /// or <see cref="ApiResponse"/> if not found.
        /// </returns>
        [HttpPatch("CancelSale/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new CancelSaleRequest { Id = id };
            var validator = new CancelSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation Failed",
                    Errors = validationResult.Errors
                    .Select(error => (ValidationErrorDetail)error)
                });

            var command = _mapper.Map<CancelSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Modifies the items of an existing sale.
        /// Only sale items can be added, updated, or removed; other sale properties cannot be changed.
        /// </summary>
        /// <param name="request">The request containing the list of items.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="NoContentResult"/> if the modification is successful,
        /// <see cref="ApiResponse"/> with validation errors if the request is invalid.
        /// or <see cref="ApiResponse"/> if not found.
        /// </returns>
        [HttpPut()]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModifySale([FromBody] ModifySaleRequest request, CancellationToken cancellationToken)
        {
            var validator = new ModifySaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Validation Failed",
                    Errors = validationResult.Errors
                    .Select(error => (ValidationErrorDetail)error)
                });

            var command = _mapper.Map<ModifySaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return NoContent();
        }
    }
}
