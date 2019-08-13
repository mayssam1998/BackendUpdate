using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
using SuS.Application.BranchManager.Query.GetBranch;
using SuS.Common;
using SuS.Domain.Entities.SuSModels;
using SuS.Persistence;
using Type = System.Type;

namespace SuS.Application.BranchManager.Command.AddUpdateBranch
{
    public class AddUpdateBranchCommandHandler : IRequestHandler<AddUpdateBranchCommand, GetBranchViewModel>
    {
        private readonly SupplierDbContext _context;
        private IMapper _mapper;
        //private IRabbitMqSender _rabbitMq;

        public AddUpdateBranchCommandHandler(SupplierDbContext context, IMapper mapper, IRabbitMqSender rabbitMq)
        {
            _context = context;
            _mapper = mapper;
           // _rabbitMq = rabbitMq;
        }


        public async Task<GetBranchViewModel> Handle(AddUpdateBranchCommand request,
            CancellationToken cancellationToken)
        {
            
            if (string.IsNullOrEmpty(request.branchName)
                || string.IsNullOrEmpty(request.branchNumber)
                || string.IsNullOrEmpty(request.supplierName)
                || string.IsNullOrEmpty(request.supplierNumber)
                || string.IsNullOrEmpty(request.supplierType)
                || string.IsNullOrEmpty(request.PostCode)
                || string.IsNullOrEmpty(request.Address.Street)
                || string.IsNullOrEmpty(request.Address.City)
                || string.IsNullOrEmpty(request.Address.Country.Name)
                || string.IsNullOrEmpty(request.Address.Country.Iso2)
                || string.IsNullOrEmpty(request.Address.Country.Iso3)
                || string.IsNullOrEmpty(request.Address.Country.Region.Name)
                || string.IsNullOrEmpty(request.supplierAlternateName.Count.ToString())) return null;
            
              var DBBranch = _context.Branch
                  .SingleOrDefault(x =>
                      x.Number.Equals(request.branchNumber, StringComparison.InvariantCultureIgnoreCase));
              var DBSupplier = _context.Supplier.SingleOrDefault(x =>
                  x.Number.Equals(request.supplierNumber, StringComparison.InvariantCultureIgnoreCase));
              var DBCity = _context.City.SingleOrDefault(x =>
                  x.Name.Equals(request.Address.City));
              Console.WriteLine("-------------------" + request.Address.City);
              var DBStreet = _context.Street.SingleOrDefault(x =>
                  x.Name.Equals(request.Address.Street));
              var DBCountry = _context.Country.SingleOrDefault(x =>
                  x.Name.Equals(request.Address.Country.Name));
              var DBregion = _context.Region.SingleOrDefault(x =>
                  x.Name.Equals(request.Address.Country.Region.Name));

              if (DBregion == null)
              {
                  //Create region
                  DBregion = _context.Region.Add(new Region
                  {
                      Name = request.Address.Country.Region.Name
                  }).Entity;

              }

              if (DBCountry == null)
              {
                  //Create country
                  DBCountry = _context.Country.Add(new Country
                  {
                      Name = request.Address.Country.Name,
                      Iso2 = request.Address.Country.Iso2,
                      Iso3 = request.Address.Country.Iso3,
                      Region = DBregion
                  }).Entity;

              }

              if (DBCity == null)
              {
                  //Create new city
                  DBCity = _context.City.Add(new City
                  {
                      Name = request.Address.City,
                      Country = DBCountry
                  }).Entity;

              }

              if (DBStreet == null)
              {
                  //Create new street
                  DBStreet = _context.Street.Add(new Street {Name = request.Address.Street, City = DBCity}).Entity;

              }
              //ICollection<Branch> branchesArray=new List<Branch>();

              if (DBSupplier == null)
              {
                  return null;
              }


              if (DBBranch == null)
              {
                  //Create new branch
                  DBBranch = _context.Branch.Add(new Branch
                  {
                      Name = request.branchName,
                      Number = request.branchNumber,
                      PostCode = request.PostCode,
                      Street = DBStreet,
                      Supplier = DBSupplier
                  }).Entity;


              }
              else
              {
                  //Update branch
                  DBBranch.Name = request.branchName;
                  DBBranch.PostCode = request.PostCode;
                  DBBranch.Street = DBStreet;
                  DBBranch.Supplier = DBSupplier;
              }

              try
              {

                  _context.SaveChanges();

              }
              catch (Exception e)
              {
                  return null;
              }


              return _mapper.Map<GetBranchViewModel>(DBBranch);
         
        }
    }
}