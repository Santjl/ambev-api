using Ambev.DeveloperEvaluation.Application.Common.Ports;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale
{
    public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
    {
        private readonly ISaleRepository _saleRepository;
        private readonly IProductGateway _productGateway;
        private readonly ICustomerGateway _customerGateway;
        private readonly IBranchGateway _branchGateway;
        private readonly IValidator<CreateSaleCommand> _validator;
        private readonly IMapper _mapper;
        public CreateSaleHandler(
            ISaleRepository saleRepository,
            IProductGateway productGateway,
            ICustomerGateway customerGateway,
            IBranchGateway branchGateway,
            IMapper mapper)
        {
            _saleRepository = saleRepository;
            _productGateway = productGateway;
            _customerGateway = customerGateway;
            _branchGateway = branchGateway;
            _validator = new CreateSaleValidator(_branchGateway, _customerGateway, _productGateway);
            _mapper = mapper;
        }
        public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken ct)
        {
            await _validator.ValidateAndThrowAsync(command, ct);

            var customer = await _customerGateway.GetByIdAsync(command.CustomerId, ct)
                      ?? throw new DomainException("Customer not found.");
            var branch = await _branchGateway.GetByIdAsync(command.BranchId, ct)
                           ?? throw new DomainException("Branch not found.");

            var sale = Sale.Create(command.Number, DateTimeOffset.UtcNow, customer.Id, customer.Name, branch.Id, branch.Name);

            foreach (var it in command.Items)
            {
                var product = await _productGateway.GetByIdAsync(it.ProductId, ct)
                    ?? throw new DomainException($"Product {it.ProductId} not found.");
                sale.AddItem(product.Id, product.Name, it.Quantity, product.Price);
            }

            var saleCreated = await _saleRepository.CreateAsync(sale, ct);

            return _mapper.Map<CreateSaleResult>(saleCreated);
        }
    }
}
