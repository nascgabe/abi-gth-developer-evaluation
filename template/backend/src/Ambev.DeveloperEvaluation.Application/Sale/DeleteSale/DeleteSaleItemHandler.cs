using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Ambev.DeveloperEvaluation.Application.Sale.DeleteSale
{
    /// <summary>
    /// Handler for processing DeleteSaleItemCommand requests.
    /// </summary>
    public class DeleteSaleItemHandler : IRequestHandler<DeleteSaleItemCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly ILogger<DeleteSaleItemHandler> _logger;

        public DeleteSaleItemHandler(ISaleRepository saleRepository, ILogger<DeleteSaleItemHandler> logger)
        {
            _saleRepository = saleRepository;
            _logger = logger;
        }

        /// <summary>
        /// Handles the DeleteSaleCommand request.
        /// </summary>
        /// <param name="request">The DeleteSale command containing the SaleId to delete</param>
        /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
        /// <returns>True if the sale was successfully deleted</returns>
        /// <exception cref="DomainException">Thrown when the sale with the provided ID does not exist</exception>
        public async Task<bool> Handle(DeleteSaleItemCommand request, CancellationToken cancellationToken)
        {
            var existingSale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            if (existingSale is null)
            {
                throw new DomainException($"The sale with ID {request.SaleId} does not exist.");
            }

            var saleItem = existingSale.Items.FirstOrDefault(item => item.Id == request.ItemId);
            if (saleItem is null)
            {
                throw new DomainException($"The sale item with ID {request.ItemId} does not exist.");
            }

            var isDeleted = await _saleRepository.DeleteSaleItemAsync(request.ItemId, cancellationToken);
            if (!isDeleted)
            {
                throw new DomainException($"Failed to delete the sale item with ID {request.ItemId}. Please try again.");
            }

            existingSale.Items.Remove(saleItem);

            _logger.LogInformation($"SaleItemDeleted: Sale item with ID {request.ItemId} was successfully deleted.");

            return true;
        }
    }
}