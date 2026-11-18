using FieldStaff.Application.Features.Customers.DTOs;
using FieldStaff.Application.Features.Customers.Interfaces;
using FieldStaff.Domain.Entities;
using FieldStaff.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FieldStaff.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly FieldStaffDbContext _context;

        public CustomerRepository(FieldStaffDbContext context)
        {
            _context = context;  
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .AsNoTracking()
                .Where(c => !c.IsDeleted)
                .OrderBy(c => c.CustomerName)
                .ToListAsync();
        }

        public async Task<(List<Customer> Customers, int TotalRecords)>
            GetPagedAsync(CustomerQueryParameters parameters)
        {
            IQueryable<Customer> query = _context.Customers
                .AsNoTracking()
                .Where(c => !c.IsDeleted);

          
            if (!string.IsNullOrWhiteSpace(parameters.Search))
            {
                string search = parameters.Search.Trim();

                query = query.Where(c =>
                    EF.Functions.Like(c.CustomerName, $"%{search}%") ||
                    EF.Functions.Like(c.Email, $"%{search}%") ||
                    EF.Functions.Like(c.ServiceAddress, $"%{search}%"));
            }

            
            if (parameters.IsActive.HasValue)
            {
                query = query.Where(c =>
                    c.IsActive == parameters.IsActive.Value);
            }

         
            int totalRecords = await query.CountAsync();

           
            bool isAscending =
                parameters.SortDirection.ToLower() == "asc";

            var orderedQuery = parameters.SortBy.ToLower() switch
            {
                "customername" => isAscending
                        ? query.OrderBy(c => c.CustomerName)
                        : query.OrderByDescending(c => c.CustomerName),
                    

                "email" => isAscending
                        ? query.OrderBy(c => c.Email)
                        : query.OrderByDescending(c => c.Email),
                    

                "serviceaddress" => isAscending
                        ? query.OrderBy(c => c.ServiceAddress)
                        : query.OrderByDescending(c => c.ServiceAddress),
                   

                "isactive" => isAscending
                        ? query.OrderBy(c => c.IsActive)
                        : query.OrderByDescending(c => c.IsActive),
                    

                "createdat" => isAscending
                        ? query.OrderBy(c => c.CreatedAt)
                        : query.OrderByDescending(c => c.CreatedAt),
                    

                 _ => query.OrderBy(c => c.CustomerName)
                   
            };

           
            orderedQuery = orderedQuery.ThenBy(c => c.CustomerId);

            List<Customer> customers = await orderedQuery
                .Skip((parameters.PageNumber - 1) * parameters.PageSize)
                .Take(parameters.PageSize)
                .ToListAsync();

            return (customers, totalRecords);
        }

        public async Task<Customer?> GetByIdAsync(int customerId)
        {
            return await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId && !c.IsDeleted);

        }
        public async Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null)
        {
            IQueryable<Customer> query = _context.Customers
                .Where(c => c.Email == email && !c.IsDeleted);
            
            if (excludeCustomerId.HasValue)
            {
                query = query.Where(c =>
                c.CustomerId != excludeCustomerId);
            }

            return await query.AnyAsync();
        }


        public async Task<Customer> CreateAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            return customer;
        }

        public Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);

            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
