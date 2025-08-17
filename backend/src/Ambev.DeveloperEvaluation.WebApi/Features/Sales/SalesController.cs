using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.ModifySale;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.ModifySale;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;
using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IProductGateway _productGateway;
        private readonly ICustomerGateway _customerGateway;
        private readonly IBranchGateway _branchGateway;

        public SalesController(IMapper mapper, IMediator mediator, 
            IProductGateway productGateway, ICustomerGateway customerGateway, 
            IBranchGateway branchGateway)
        {
            _mapper = mapper;
            _mediator = mediator;
            _productGateway = productGateway;
            _customerGateway = customerGateway;
            _branchGateway = branchGateway;
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
            var validator = new CreateSaleRequestValidator(_branchGateway, _customerGateway, _productGateway);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<CreateSaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return Created(string.Empty,new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<CreateSaleResponse>(response)
            });
        }

        /// <summary>
        /// Retrieves a sale by their ID
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details with items if found</returns>
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
                return BadRequest(validationResult.Errors);

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

            return Ok(new ApiResponseWithData<List<GetSaleResponse>>
            {
                Success = true,
                Message = "All sales retrieved successfully",
                Data = _mapper.Map<List<GetSaleResponse>>(response)
            });
        }

        /// <summary>
        /// Cancels a sale by its unique identifier.
        /// Validates the request and cancels the sale if found and valid.
        /// </summary>
        /// <param name="id">The unique identifier of the sale to cancel.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="ApiResponseWithData{CancelSaleResponse}"/> with the cancelled sale details if successful,
        /// <see cref="ApiResponse"/> with validation errors if the request is invalid,
        /// or <see cref="ApiResponse"/> if the sale is not found.
        /// </returns>
        [HttpPatch("CancelSale/{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<CancelSaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var request = new CancelSaleRequest { Id = id };
            var validator = new CancelSaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<CancelSaleCommand>(request.Id);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<GetSaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<GetSaleResponse>(response)
            });
        }

        /// <summary>
        /// Modifies the items of an existing sale.
        /// Only sale items can be added, updated, or removed; other sale properties cannot be changed.
        /// </summary>
        /// <param name="request">The request containing the sale ID and the new list of items.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>
        /// Returns <see cref="ApiResponseWithData{ModifySaleResponse}"/> with the updated sale details if successful,
        /// <see cref="ApiResponse"/> with validation errors if the request is invalid,
        /// or <see cref="ApiResponse"/> if the sale is not found.
        /// </returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<ModifySaleResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ModifySale([FromBody] ModifySaleRequest request, CancellationToken cancellationToken)
        {
            var validator = new ModifySaleRequestValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var command = _mapper.Map<ModifySaleCommand>(request);
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new ApiResponseWithData<ModifySaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<ModifySaleResponse>(response)
            });
        }
    }
}
