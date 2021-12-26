using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using asp13EntityFramework.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.Role
{
    [Authorize(Policy = "AllowEditRole")]
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, MyBlogContext conext) : base(roleManager, conext)
        {
        }

        public class InputModel
        {
            [DisplayName("Tên của role")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string Name { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public List<IdentityRoleClaim<string>> Claims { get; set; }
        public IdentityRole role { set; get; }
        public async Task<IActionResult> OnGet(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role != null)
            {
                Input = new InputModel()
                {
                    Name = role.Name
                };
                Claims = await _conext.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
                return Page();
            }
            return NotFound("Không tìm thấy role");
        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (roleid == null) return NotFound("Không tìm thấy role");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            Claims = await _conext.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
            if (!ModelState.IsValid)
            {
                return Page();
            }
            role.Name = Input.Name;
            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Bạn vừa đổi tên: {Input.Name}";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }
    }
}
