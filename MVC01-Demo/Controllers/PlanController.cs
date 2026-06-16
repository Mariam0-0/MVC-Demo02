using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVC01_Demo.Contexts;

namespace MVC01_Demo.Controllers
{
    public class PlanController : Controller
    {
        // 1) DB connection 
        //private readonly GymDbContext context;

        private readonly IGenericRepository<Plan> _planRepository;
        public PlanController(IGenericRepository<Plan> planRepository)
        {
            _planRepository = planRepository;
        }

        // 2) Get :: BaseURL/Plan/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var plans = await _planRepository.GetAllAsync(ct: ct); // pass by name
            return View(plans);
        }

        // 3) Get :: BaseURL/Plan/Details/{id}
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var plan = await _planRepository.GetByIdAsync(id, ct);
            if (plan == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }
    }
}
