
using FieldStaff.Application.Features.Common.DTOs;
using FieldStaff.Application.Features.Customers.DTOs;
using FieldStaff.Application.Features.Customers.Interfaces;
using FieldStaff.Domain.Entities;

namespace FieldStaff.Application.Features.Customers.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        
        public async Task<CustomerResponseDto> CreateCustomerAsync(
            CreateCustomerDto request,
            int? createdByUserId)
        {
            string email = request.Email?.Trim() ?? string.Empty;

           
            if (!string.IsNullOrWhiteSpace(email))
            {
                bool emailExists =
                    await _customerRepository.EmailExistsAsync(email);

                if (emailExists)
                {
                    throw new InvalidOperationException(
                        "A customer with this email already exists.");
                }
            }

            var customer = new Customer
            {
                CustomerName = request.CustomerName.Trim(),
                Email = email,
                ServiceAddress = request.ServiceAddress.Trim(),
                IsActive = request.IsActive,
                IsDeleted = false,

                CreatedBy = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _customerRepository.CreateAsync(customer);

            await _customerRepository.SaveChangesAsync();

            return MapToResponseDto(customer);
        }

      
        public async Task<CustomerResponseDto?> GetCustomerByIdAsync(
            int customerId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return null;
            }

            return MapToResponseDto(customer);
        }

     
        public async Task<List<CustomerResponseDto>> GetAllCustomersAsync()
        {
            var customers =
                await _customerRepository.GetAllAsync();

            return customers
                .Select(MapToResponseDto)
                .ToList();
        }

        
        public async Task<PaginatedResponse<CustomerResponseDto>>
            GetPagedCustomersAsync(CustomerQueryParameters parameters)
        {
            
            if (parameters.PageNumber < 1)
            {
                throw new InvalidOperationException(
                    "Page number must be greater than 0.");
            }

            if (parameters.PageSize < 1 || parameters.PageSize > 100)
            {
                throw new InvalidOperationException(
                    "Page size must be between 1 and 100.");
            }


            var allowedSortColumns = new[]
            {
                "customerName",
                "email",
                "serviceAddress",
                "isActive",
                "createdAt"
            };

            if (!allowedSortColumns.Contains(
                parameters.SortBy,
                StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    "Invalid sorting column.");
            }

           
            if (parameters.SortDirection.ToLower() != "asc" &&
                parameters.SortDirection.ToLower() != "desc")
            {
                throw new InvalidOperationException(
                    "Sort direction must be either asc or desc.");
            }

          
            parameters.Search = parameters.Search?.Trim();
            parameters.SortBy = parameters.SortBy.Trim();
            parameters.SortDirection =
                parameters.SortDirection.ToLower();

            var (customers, totalRecords) =
                await _customerRepository.GetPagedAsync(parameters);

            var customerDtos = customers
                .Select(MapToResponseDto)
                .ToList();

            return new PaginatedResponse<CustomerResponseDto>
            {
                Data = customerDtos,
                PageNumber = parameters.PageNumber,
                PageSize = parameters.PageSize,
                TotalRecords = totalRecords,
                TotalPages = (int)Math.Ceiling(
                    totalRecords / (double)parameters.PageSize),

                HasPreviousPage = parameters.PageNumber > 1,

                HasNextPage =
                    parameters.PageNumber <
                    (int)Math.Ceiling(
                        totalRecords / (double)parameters.PageSize)
            };
        }

        
        public async Task<CustomerResponseDto?> UpdateCustomerAsync(
            int customerId,
            UpdateCustomerDto request,
            int? updatedByUserId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return null;
            }

            string email = request.Email?.Trim() ?? string.Empty;

            
            if (!string.IsNullOrWhiteSpace(email))
            {
                bool emailExists =
                    await _customerRepository.EmailExistsAsync(
                        email,
                        customerId);

                if (emailExists)
                {
                    throw new InvalidOperationException(
                        "Another customer with this email already exists.");
                }
            }

            customer.CustomerName = request.CustomerName.Trim();
            customer.Email = email;
            customer.ServiceAddress = request.ServiceAddress.Trim();
            customer.IsActive = request.IsActive;

            customer.UpdatedBy = updatedByUserId;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);

            await _customerRepository.SaveChangesAsync();

            return MapToResponseDto(customer);
        }

        
        public async Task<bool> DeleteCustomerAsync(
            int customerId,
            int? deletedByUserId)
        {
            var customer =
                await _customerRepository.GetByIdAsync(customerId);

            if (customer == null)
            {
                return false;
            }

            customer.IsDeleted = true;
            customer.IsActive = false;

            customer.UpdatedBy = deletedByUserId;
            customer.UpdatedAt = DateTime.UtcNow;

            await _customerRepository.UpdateAsync(customer);

            await _customerRepository.SaveChangesAsync();

            return true;
        }

       
        private static CustomerResponseDto MapToResponseDto(
            Customer customer)
        {
            return new CustomerResponseDto
            {
                CustomerId = customer.CustomerId,
                CustomerName = customer.CustomerName,
                Email = customer.Email,
                ServiceAddress = customer.ServiceAddress,
                IsActive = customer.IsActive,
                CreatedAt = customer.CreatedAt,
                CreatedBy = customer.CreatedBy,
                UpdatedAt = customer.UpdatedAt,
                UpdatedBy = customer.UpdatedBy
            };
        }
    }
}
