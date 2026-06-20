using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModel;
using GymManagmemnt.BLL.ViewModels.SessionViewModel;
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

        #region Get 
        // GET ::base/Session/Index
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var sessions = await _sessionService.GetAllSessionsAsync(ct);
            return View(sessions);
        }

        // get :: base/session/detaild/{id}

        [HttpGet]
        public async Task<IActionResult> Details(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionByIdAsync(id, ct);
            if (result.success)
                return View(result.value);
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region Update

        [HttpGet]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var result = await _sessionService.GetSessionToUpdate(id, ct);
            if (result.success)
            {
                await GetTrainerList();
                return View(result.value);
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                return RedirectToAction($"{nameof(Index)}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                await GetTrainerList();

                return View(model);
            }
            var result = await _sessionService.UpdateSessionAsync(id, model, ct);
            if (result.success)
            {
                TempData["SuccessMessage"] = "Session Updated";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = result.error;
                await GetTrainerList();

                return View(model);
            }

        }

        private async Task GetTrainerList()
        {
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDown(), "Id", "Name");
        }
        #endregion

        #region Delete
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _sessionService.GetSessionByIdAsync(id, ct);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Session Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();

        }

        public async Task<IActionResult> DeleteConfirmed([FromRoute] int id, CancellationToken ct)
        {
            var result = await _sessionService.DeleteSessionAsync(id, ct);

            if (result.success)
                TempData["SuccessMessage"] = "Session Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Delete Session";
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
