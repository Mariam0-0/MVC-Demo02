using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace MVC01_Demo.Controllers
{
   
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;
        public SessionController(ISessionService sessionService)
        {
            _sessionService = sessionService;
            
        }
        // GET ::base/Session/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        #region Create Actions
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await DropDownList();

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Create(CreateSessionViewModel model, CancellationToken ct)
        {
            if(!ModelState.IsValid)
            {
                await DropDownList();
                return View(model);
            }

            var result = await _sessionService.CreateSessionAsync(model, ct);
            if(result.success)
            {
                TempData["SuccessMessage"] = "Session Created";
                return RedirectToAction(nameof(Index));
            }
            TempData["ErrorMessage"] = result.error;
            
            await DropDownList();
            return View(model);
        } 
        private async Task DropDownList()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDown(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _sessionService.GetCategoryForDropDown(), "Id", "CategoryName");
        }
        #endregion

    }
}
