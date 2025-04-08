using Ambev.DeveloperEvaluation.Domain.Repositories;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sale.DeleteSale
{
    /// <summary>
    /// Handler for processing DeleteSaleCommand requests.
    /// </summary>
    public class DeleteSaleHandler : IRequestHandler<DeleteSaleCommand, bool>
    {
        private readonly ISaleRepository _saleRepository;

        public DeleteSaleHandler(ISaleRepository saleRepository)
        {
            _saleRepository = saleRepository;
        }

        /// <summary>
        /// Handles the DeleteSaleCommand request.
        /// </summary>
        /// <param name="request">The DeleteSale command containing the SaleId to delete</param>
        /// <param name="cancellationToken">Cancellation token to handle request cancellation</param>
        /// <returns>True if the sale was successfully deleted</returns>
        /// <exception cref="DomainException">Thrown when the sale with the provided ID does not exist</exception>
        public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
        {
            var existingSale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
            if (existingSale is null)
            {
                throw new DomainException($"Sale with ID {request.SaleId} not found.");
            }

            var isDeleted = await _saleRepository.DeleteAsync(request.SaleId, cancellationToken);
            if (!isDeleted)
            {
                throw new DomainException($"Failed to delete sale with ID {request.SaleId}. Please try again.");
            }

            return true;
        }
    }
}