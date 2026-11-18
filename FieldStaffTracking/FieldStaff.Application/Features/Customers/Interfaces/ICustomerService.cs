
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Customers.DTOs;

namespace FieldStaff.Application.Features.Customers.Interfaces
{
    public interface ICustomerService
    {
        Task<CustomerResponseDto> CreateCustomerAsync(
            CreateCustomerDto request,
            int? createdByUserId);

        Task<CustomerResponseDto?> GetCustomerByIdAsync(int customerId);

        Task<List<CustomerResponseDto>> GetAllCustomersAsync();

        Task<PaginatedResponse<CustomerResponseDto>> GetPagedCustomersAsync(
            CustomerQueryParameters parameters);

        Task<CustomerResponseDto?> UpdateCustomerAsync(
            int customerId,
            UpdateCustomerDto request,
            int? updatedByUserId);

        Task<bool> DeleteCustomerAsync(
            int customerId,
            int? deletedByUserId);
    }
}
