using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using asp13EntityFramework.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace App.Admin.Role
{
    [Authorize(Roles = "Admin")]
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, MyBlogContext conext) : base(roleManager, conext)
        {
        }

        public class InputModel
        {
            [DisplayName("Kiểu (tên) claim")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimType { get; set; }
            [DisplayName("Giá trị")]
            [Required(ErrorMessage = "Phải nhập {0}")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} phải dài từ {2} đến {1} ký tự")]
            public string ClaimValue { get; set; }
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public IdentityRole role { get; set; }
        public IdentityRoleClaim<string> claim { get; set; }
        public async Task<IActionResult> OnGet(int? claimid)
        {
            if (claimid == null)
            {
                return NotFound("Không tìm thấy claim");
            }
            claim = _conext.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy claim");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            Input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? claimid)
        {
            if (claimid == null)
            {
                return NotFound("Không tìm thấy claim");
            }
            claim = _conext.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy claim");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }
            if (_conext.RoleClaims.Any(c => c.RoleId == role.Id && c.ClaimType == Input.ClaimType && c.ClaimValue == Input.ClaimValue && c.Id != claimid))
            {
                ModelState.AddModelError(string.Empty, "Claim này đã có trong role");
                return Page();
            }

            claim.ClaimType = Input.ClaimType;
            claim.ClaimValue = Input.ClaimValue;
            await _conext.SaveChangesAsync();

            StatusMessage = "Vừa cập nhật claim";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int? claimid)
        {
            if (claimid == null)
            {
                return NotFound("Không tìm thấy claim");
            }
            claim = _conext.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Không tìm thấy claim");

            role = await _roleManager.FindByIdAsync(claim.RoleId);
            if (role == null)
            {
                return NotFound("Không tìm thấy role");
            }

            await _roleManager.RemoveClaimAsync(role, new Claim(claim.ClaimType, claim.ClaimValue));
            // _conext.RoleClaims.Remove(claim);
            // await _conext.SaveChangesAsync();
            StatusMessage = "Vừa xóa claim";

            return RedirectToPage("./Edit", new { roleid = role.Id });
        }
    }
}
