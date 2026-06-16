using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace MVC01_Demo.Controllers
{
    public class MemberController : Controller
    {

        // service
        private readonly IMemberService _memService;
        public MemberController(IMemberService memService)
        {
            _memService = memService;
        }
        #region Get Members

        // GET :: baseUrl/members/index => list all members

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memService.GetAllAsync(ct);
            return View(members);
        }

        // GET :: baseurl/members/details/{id} => get user info by id

        // GET :: baseurl/members/healthrecorddetails/{id} => get data + health record of member

        #endregion

        #region Create
        // GET :: baseurl/members/create => show empty form

        [HttpGet]
        public IActionResult Create()
            => View();

        // POST :: baseurl/members/create/{member} => submit form
        [HttpPost]
        public async Task<IActionResult> CreateMember(CreateMemberViewModel model, CancellationToken ct)
        {
            // check model state
            if(!ModelState.IsValid) return View(nameof(Create), model);


            var result = await _memService.CreateMemberAsync(model, ct);

            if (result)
            {
                TempData["SuccessMessage"] = "Member Created Successfully";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to Create Member";
            }

                return RedirectToAction(nameof(Index));
        }
        
        #endregion

        #region Edit
        // GET :: baseurl/members/edit/{id} ==> show edit form

        // POST :: baseurl/members/edit/{member} => submit edit form
        #endregion

        #region Delete

        // GET :: baseurl/members/delete/{id} => show validation page
        #endregion
        
    }
}
