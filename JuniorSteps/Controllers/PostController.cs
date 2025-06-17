using JuniorSteps.Data;
using JuniorSteps.Models;
using JuniorSteps.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JuniorSteps.Controllers
{
    public class PostController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        public PostController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [HttpGet("/secret-url")]
        [ServiceFilter(typeof(SimpleAuthFilter))]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
            return View();
        }



        [ValidateAntiForgeryToken]
        [HttpPost("/secret-url")]
        public async Task<IActionResult> Create(PostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories.ToList(), "Id", "Name");
                return View(model);
            }

            string uniqueFileName = null;
            if (model.ImageFile != null)
            {
                string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder); // yoksa oluştur

                var extension = Path.GetExtension(model.ImageFile.FileName);
                uniqueFileName = Guid.NewGuid().ToString().Substring(0, 8) + extension;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
            }

            var post = new Post
            {
                Title = model.Title,
                Description = model.Description,
                Content = model.Content,
                Created = DateTime.UtcNow,
                CategoryId = model.CategoryId,
                ImagePath = "/uploads/" + uniqueFileName
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Post");
        }


        public async Task<IActionResult> Index(int page = 1, string? category = null, string? search = null)
        {
            int pageSize = 6;

            var query = _context.Posts
                .Include(p => p.Category)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
                query = query.Where(p => p.Category.Name == category);

            if (!string.IsNullOrEmpty(search))
            {
                var keywords = search.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                foreach (var keyword in keywords)
                {
                    var lowerKeyword = keyword.ToLower();
                    query = query.Where(p =>
                        p.Title.ToLower().Contains(lowerKeyword) ||
                        p.Description.ToLower().Contains(lowerKeyword) ||
                        p.Content.ToLower().Contains(lowerKeyword));
                }
            }

            var totalPosts = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalPosts / pageSize);

            if (page < 1)
                return RedirectToAction("Index", new { page = 1, category, search });
            if (page > totalPages && totalPages != 0)
                return RedirectToAction("Index", new { page = totalPages, category, search });

            var posts = await query
                .OrderByDescending(p => p.Created)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var categoryCounts = await _context.Posts
                .Where(p => p.CategoryId != null)
                .GroupBy(p => p.Category.Name)
                .Select(g => new CategoryCountViewModel
                {
                    CategoryName = g.Key,
                    PostCount = g.Count()
                }).ToListAsync();

            var viewModel = new PostIndexViewModel
            {
                Posts = posts,
                CategoryCounts = categoryCounts,
                CurrentPage = page,
                TotalPages = totalPages
            };
            var latestPost = await _context.Posts.OrderByDescending(p=>p.Created).FirstOrDefaultAsync();    
            ViewBag.FirstPostId = latestPost?.Id ?? 1;
            return View(viewModel);
        }

        public async Task<IActionResult> Detail(int id)
        {
            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return View(post);
        }


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Manage");
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
            {
                return RedirectToAction("Index", "Post");
            }

            var post = _context.Posts.Find(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [ServiceFilter(typeof(SimpleAuthFilter))]
        public async Task<IActionResult> Manage()
        {
            if (HttpContext.Session.GetString("IsAdmin") != "true")
                return RedirectToAction("Login", "Admin");

            var posts = await _context.Posts.Include(p => p.Category).ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync(); 

            return View(posts);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            var viewModel = new PostCreateViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                Content = post.Content,
                CategoryId = post.CategoryId,
                ImagePath = post.ImagePath
            };

            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
            return View(viewModel);
        }
        [HttpPost("Post/Edit/{id}")]
        public async Task<IActionResult> Edit(PostCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name");
                return View(model);
            }

            var post = await _context.Posts.FindAsync(model.Id);
            if (post == null) return NotFound();

            post.Title = model.Title;
            post.Description = model.Description;
            post.Content = model.Content;   
            post.CategoryId = model.CategoryId;

            await _context.SaveChangesAsync();
            return RedirectToAction("Manage");
        }

    }
}
