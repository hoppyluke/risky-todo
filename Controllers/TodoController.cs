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
    }
}