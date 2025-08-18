using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Application.Messaging;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductGateway _productGateway;
        private readonly ICustomerGateway _customerGateway;
        private readonly IBranchGateway _branchGateway;
        private readonly IMapper _mapper;
        private readonly IMessageBus _messageBus;
        public CreateSaleHandler(
            ISaleRepository saleRepository,
            IProductGateway productGateway,
            ICustomerGateway customerGateway,
            IBranchGateway branchGateway,
            IMapper mapper,
            IMessageBus messageBus)
        {
            _saleRepository = saleRepository;
            _productGateway = productGateway;
            _customerGateway = customerGateway;
            _branchGateway = branchGateway;
            _mapper = mapper;
            _messageBus = messageBus;
        }
        public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
        {
            var validator = new CreateSaleValidator();
            var validationResult = await validator.ValidateAsync(command, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var customer = await _customerGateway.GetByIdAsync(command.CustomerId, cancellationToken)
                      ?? throw new DomainException("Customer not found.");
            var branch = await _branchGateway.GetByIdAsync(command.BranchId, cancellationToken)
                           ?? throw new DomainException("Branch not found.");

            var sale = Sale.Create(command.Number, DateTimeOffset.UtcNow, customer.Id, customer.Name, branch.Id, branch.Name);

            foreach (var it in command.Items)
            {
                var product = await _productGateway.GetByIdAsync(it.ProductId, cancellationToken)
                    ?? throw new DomainException($"Product {it.ProductId} not found.");
                sale.AddItem(product.Id, product.Name, it.Quantity, product.Price);
            }

            var saleCreated = await _saleRepository.CreateAsync(sale, cancellationToken);

            var message = new IntegrationMessage(EventsConsts.SaleCreated, new SaleCreatedEvent(saleCreated), DateTimeOffset.Now);
            await _messageBus.PublishAsync(message, cancellationToken);

            return _mapper.Map<CreateSaleResult>(saleCreated);
        }
    }
}
