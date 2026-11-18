
using FieldStaff.API.Authorization.Attributes;
using FieldStaff.Application.Features.Customers.DTOs;
using FieldStaff.Application.Features.Customers.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // CREATE CUSTOMER
        [HasPermission("CUSTOMER_CREATE")]
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(
            [FromBody] CreateCustomerDto request)
        {
            try
            {
                int? createdByUserId = GetCurrentUserId();

                var result = await _customerService.CreateCustomerAsync(
                    request,
                    createdByUserId);

                return CreatedAtAction(
                    nameof(GetCustomerById),
                    new { id = result.CustomerId },
                    result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // GET ALL CUSTOMERS
        [HasPermission("CUSTOMER_VIEW")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var result = await _customerService.GetAllCustomersAsync();

            return Ok(result);
        }

        // GET PAGINATED CUSTOMERS
        [HasPermission("CUSTOMER_VIEW")]
        [HttpGet]
        public async Task<IActionResult> GetCustomers(
            [FromQuery] CustomerQueryParameters parameters)
        {
            try
            {
                var result =
                    await _customerService.GetPagedCustomersAsync(parameters);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // GET CUSTOMER BY ID
        [HasPermission("CUSTOMER_VIEW")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetCustomerById(int id)
        {
            var result =
                await _customerService.GetCustomerByIdAsync(id);

            if (result == null)
            {
                return NotFound(new
                {
                    message = "Customer not found."
                });
            }

            return Ok(result);
        }

        // UPDATE CUSTOMER
        [HasPermission("CUSTOMER_UPDATE")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCustomer(
            int id,
            [FromBody] UpdateCustomerDto request)
        {
            try
            {
                int? updatedByUserId = GetCurrentUserId();

                var result = await _customerService.UpdateCustomerAsync(
                    id,
                    request,
                    updatedByUserId);

                if (result == null)
                {
                    return NotFound(new
                    {
                        message = "Customer not found."
                    });
                }

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE CUSTOMER (SOFT DELETE)
        [HasPermission("CUSTOMER_DELETE")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            int? deletedByUserId = GetCurrentUserId();

            bool deleted = await _customerService.DeleteCustomerAsync(
                id,
                deletedByUserId);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Customer not found."
                });
            }

            return Ok(new
            {
                message = "Customer deleted successfully."
            });
        }

        // GET CURRENT LOGGED-IN USER ID
        private int? GetCurrentUserId()
        {
            var userIdClaim =
                User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out int userId))
            {
                return userId;
            }

            return null;
        }
    }
}
            