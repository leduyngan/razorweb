using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using asp13EntityFramework.models;

namespace asp13EntityFramework.Pages_Blog
{
    public class IndexModel : PageModel
    {
        private readonly asp13EntityFramework.models.MyBlogContext _context;

        public IndexModel(asp13EntityFramework.models.MyBlogContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; }
        public const int Item_Per_Page = 10;
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { get; set; }
        public int countPage { get; set; }

        public async Task OnGetAsync(string SearchString)
        {
            int totalArticle = await _context.articles.CountAsync();
            countPage = (int)Math.Ceiling((double)totalArticle / Item_Per_Page);

            if (currentPage < 1)
                currentPage = 1;
            if (currentPage > countPage)
                currentPage = countPage;

            var qr = (from a in _context.articles
                      orderby a.Created descending
                      select a)
                     .Skip((currentPage - 1) * Item_Per_Page)
                     .Take(Item_Per_Page);
            if (!string.IsNullOrEmpty(SearchString))
            {
                Article = await qr.Where(a => a.Title.Contains(SearchString)).ToListAsync();
            }
            else
            {
                Article = await qr.ToListAsync();
            }

        }
    }
}
