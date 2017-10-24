using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RiskyTodo.Data;
using RiskyTodo.Models;
using RiskyTodo.Models.TodoViewModels;

namespace RiskyTodo.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TodoController(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var todos = await _context.Todos.Where(t => t.OwnerId == user.Id).ToListAsync();

            return View(todos);
        }

        public IActionResult Create()
        {
            return View(new CreateViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateViewModel model)
        {
            if(ModelState.IsValid)
            {
                // Assign to current user
                var user = await _userManager.GetUserAsync(User);
            
                _context.Todos.Add(new Todo
                {
                    Name = model.Name,
                    OwnerId = user.Id
                });
                
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        public IActionResult Search()
        {
            return View(new SearchViewModel());
        }

        // Search with a SQL injection vulnerability
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Search(SearchViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var query = $"SELECT * FROM Todos WHERE OwnerId = '{user.Id}' AND instr(Name, '{viewModel.SearchTerm}')";

                viewModel.Results = await _context.Todos
                    .FromSql(query)
                    .ToListAsync();
            }

            return View(viewModel);
        }

        public IActionResult SearchSafe1()
        {
            return View("Search", new SearchViewModel());
        }

        // Fix the SQL injection vulnerability
        // by using EF to generate a parameterized query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchSafe1(SearchViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                viewModel.Results = await _context.Todos
                    .Where(t => t.OwnerId == user.Id && t.Name.Contains(viewModel.SearchTerm))
                    .ToListAsync();
            }

            return View("Search", viewModel);
        }

        public IActionResult SearchSafe2()
        {
            return View("Search", new SearchViewModel());
        }

        // Fix the SQL injection vulnerability by using a parameterized query
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchSafe2(SearchViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                
                viewModel.Results = await _context.Todos
                    .FromSql($"SELECT * FROM Todos WHERE OwnerId = {user.Id} AND instr(Name, {viewModel.SearchTerm})")
                    .ToListAsync();
            }

            return View("Search", viewModel);
        }
    }
}