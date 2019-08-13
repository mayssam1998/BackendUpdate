using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SuS.Application.Interfaces.Models;
using SuS.Domain.Entities.SuSModels;
using SuS.Persistence;

namespace SuS.Application.SupplierManager.Query.GetSupplier
{
    public class GetSupplierQueryHandler : IRequestHandler<GetSupplierQuery, GetSupplierViewModel>
    {
        private readonly SupplierDbContext _context;
        private IMapper _mapper;

        public GetSupplierQueryHandler(SupplierDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<GetSupplierViewModel> Handle(GetSupplierQuery request, CancellationToken cancellationToken)
        {
            var SuppliersList = _context.Supplier
                .Include(x => x.Branch)
                .ThenInclude(x => x.Street)
                .ThenInclude(x => x.City)
                .ThenInclude(x => x.Country)
                .ThenInclude(x => x.Region)
                .Include(x => x.AlternateName)
                .Include(x => x.Type)
                .Where(x =>
                    (string.IsNullOrEmpty(request.SupplierSearch) || x.Name.Contains(request.SupplierSearch, StringComparison.InvariantCultureIgnoreCase))).ToList();

            return new GetSupplierViewModel()
            {
                supplierList = _mapper.Map<List<SupplierViewModel>>(SuppliersList),
                supplierCount = SuppliersList.Count()
            };
        }
    }
}