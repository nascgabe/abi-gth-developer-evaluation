using Ambev.DeveloperEvaluation.Application.Sale.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sale.GetSale.Commands;
using Ambev.DeveloperEvaluation.Application.Sale.UpdateSale.Commands;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSales;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateItemQuantity;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sale.GetSale;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using static Ambev.DeveloperEvaluation.WebApi.Features.Sale.GetSale.GetSalesResponse;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public SalesController(IMediator mediator, IMapper mapper, IUserRepository userRepository)
        {
            _mediator = mediator;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Creates a new sale
        /// </summary>
        /// <param name="request">The sale creation request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
        {
            var authenticatedUser = HttpContext.User.Identity?.Name;

            if (string.IsNullOrEmpty(authenticatedUser))
                return Unauthorized(new ApiResponse { Success = false, Message = "User is not authenticated" });

            var user = await _userRepository.GetByUsernameAsync(authenticatedUser, cancellationToken);
            if (user == null)
                return NotFound(new ApiResponse { Success = false, Message = "Authenticated user not found in the database" });

            request.Client = user.Username;

            var command = _mapper.Map<CreateSaleCommand>(request);
            var result = await _mediator.Send(command, cancellationToken);

            return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
            {
                Success = true,
                Message = "Sale created successfully",
                Data = _mapper.Map<CreateSaleResponse>(result)
            });
        }

        /// <summary>
        /// Retrieves all sales
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The all sale details/returns>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseWithData<List<GetSalesResponse>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllSales(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAllSalesCommand(), cancellationToken);

            return Ok(new ApiResponseWithData<List<SaleResponse>>
            {
                Success = true,
                Message = "Sales retrieved successfully",
                Data = _mapper.Map<List<SaleResponse>>(result)
            });
        }

        /// <summary>
        /// Retrieves a sale by its ID
        /// </summary>
        /// <param name="id">The unique identifier of the sale</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The sale details if found</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponseWithData<GetSalesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSaleById([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetSaleCommand(id), cancellationToken);

            return Ok(new ApiResponseWithData<SaleResponse>
            {
                Success = true,
                Message = "Sale retrieved successfully",
                Data = _mapper.Map<SaleResponse>(result)
            });
        }

        /// <summary>
        /// Cancel a sale by ID
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Sale Cancel</returns>
        [HttpPut("{saleId}/cancel")]
        public async Task<IActionResult> CancelSale(Guid saleId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CancelSaleCommand(saleId), cancellationToken);

            if (!result)
                return NotFound(new ApiResponse { Success = false, Message = "Sale not found or already cancelled" });

            return Ok(new ApiResponse { Success = true, Message = "Sale cancelled successfully" });
        }

        /// <summary>
        /// Partial cancellation of a sale item by its ID
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Partial cancellation item sale</returns>
        [HttpPut("{saleId}/items/{itemId}/partial-cancellation")]
        public async Task<IActionResult> UpdateItemQuantity(Guid saleId,
            Guid itemId, [FromBody] UpdateItemQuantityRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateItemQuantityCommand(saleId, itemId, request.Quantity), cancellationToken);

            if (!result)
                return BadRequest(new ApiResponse { Success = false, Message = "Invalid quantity or item not found" });

            return Ok(new ApiResponse { Success = true, Message = "Item quantity updated successfully" });
        }

        /// <summary>
        /// Delete a item sale by ID
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Delete a item sale</returns>
        [HttpDelete("{saleId}/items/{itemId}")]
        public async Task<IActionResult> DeleteSaleItem(Guid saleId, Guid itemId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteSaleItemCommand(saleId, itemId), cancellationToken);

            if (!result)
                return NotFound(new ApiResponse { Success = false, Message = "Item not found" });

            return Ok(new ApiResponse { Success = true, Message = "Item deleted successfully" });
        }

        /// <summary>
        /// Delete a sale by ID
        /// </summary>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Delete a sale</returns>
        [HttpDelete("{saleId}")]
        public async Task<IActionResult> DeleteSale(Guid saleId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new DeleteSaleCommand(saleId), cancellationToken);

            if (!result)
                return NotFound(new ApiResponse { Success = false, Message = "Sale not found" });

            return Ok(new ApiResponse { Success = true, Message = "Sale deleted successfully" });
        }
    }
}