
using FieldStaff.Application.Features.Menus.DTOs;
using FieldStaff.Application.Features.Menus.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FieldStaff.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenusController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        // GET: api/menus
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var menus = await _menuService.GetAllAsync();

            return Ok(menus);
        }

        // GET: api/menus/5
        [HttpGet("{menuId:int}")]
        public async Task<IActionResult> GetById(int menuId)
        {
            var menu = await _menuService.GetByIdAsync(menuId);

            if (menu == null)
                return NotFound(new
                {
                    message = "Menu not found."
                });

            return Ok(menu);
        }

        //[HttpGet("my-menus")]
        //public async Task<IActionResult> GetMyMenus()
        //{
        //    var userId = GetCurrentUserId();

        //    if (userId == null)
        //        return Unauthorized(new
        //        {
        //            message = "Unable to identify the authenticated user."
        //        });

        //    var menus = await _menuService.GetMyMenusAsync(userId.Value);

        //    return Ok(menus);
        //}

        // POST: api/menus
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateMenuRequestDto request)
        {
            try
            {
                int? userId = GetCurrentUserId();

                var menu = await _menuService.CreateAsync(
                    request,
                    userId);

                return CreatedAtAction(
                    nameof(GetById),
                    new { menuId = menu.MenuId },
                    menu);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/menus/5
        [HttpPut("{menuId:int}")]
        public async Task<IActionResult> Update(
            int menuId,
            [FromBody] UpdateMenuRequestDto request)
        {
            try
            {
                int? userId = GetCurrentUserId();

                var menu = await _menuService.UpdateAsync(
                    menuId,
                    request,
                    userId);

                if (menu == null)
                    return NotFound(new
                    {
                        message = "Menu not found."
                    });

                return Ok(menu);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/menus/5
        [HttpDelete("{menuId:int}")]
        public async Task<IActionResult> Delete(int menuId)
        {
            try
            {
                int? userId = GetCurrentUserId();

                bool deleted = await _menuService.DeleteAsync(
                    menuId,
                    userId);

                if (!deleted)
                    return NotFound(new
                    {
                        message = "Menu not found."
                    });

                return Ok(new
                {
                    message = "Menu deleted successfully."
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET CURRENT AUTHENTICATED USER ID
        private int? GetCurrentUserId()
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            return int.TryParse(userId, out int id)
                ? id
                : null;
        }
    }
}
