using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Customers.DTOs;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Customers.Interfaces
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<(List<Customer> Customers, int TotalRecords)> GetPagedAsync(CustomerQueryParameters parameters);
        Task<Customer?> GetByIdAsync(int customerId);
        Task<bool> EmailExistsAsync(string email, int? excludeCustomerId = null);
        Task<Customer> CreateAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task SaveChangesAsync();
    }
}
