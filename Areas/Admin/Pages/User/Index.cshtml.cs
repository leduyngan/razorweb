using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asp13EntityFramework.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace App.Admin.User
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        public IndexModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        [TempData]
        public string StatusMessage { get; set; }
        public List<UserAndRole> users { set; get; }
        public class UserAndRole : AppUser
        {
            public string RoleNames { get; set; }
        }
        public const int Item_Per_Page = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPage { get; set; }
        public int totalUsers { get; set; }
        public async Task OnGet()
        {
            totalUsers = await _userManager.Users.CountAsync();
            countPage = (int)Math.Ceiling((double)totalUsers / Item_Per_Page);

            if (currentPage < 1)
                currentPage = 1;
            if (currentPage > countPage)
                currentPage = countPage;

            var qr1 = _userManager.Users.OrderBy(u => u.UserName)
                    .Skip((currentPage - 1) * Item_Per_Page)
                    .Take(Item_Per_Page)
                    .Select(u => new UserAndRole()
                    {
                        Id = u.Id,
                        UserName = u.UserName,
                    });
            users = await qr1.ToListAsync();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }
        }
        public void OnPost() => RedirectToPage();
    }
}
