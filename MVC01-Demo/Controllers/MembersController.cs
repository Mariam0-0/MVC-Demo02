using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;

namespace MVC01_Demo.Controllers
{
    [Authorize(Roles ="SuperAdmin")]
    public class MembersController : Controller
    {

        // service
        private readonly IMemberService _memService;
        private readonly IAttachmentService _attachService;
        public MembersController(IMemberService memService, IAttachmentService attachmentService)
        {
            _memService = memService;
            _attachService = attachmentService;
        }
        #region Get Members

        // GET :: baseUrl/members/index => list all members

        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var members = await _memService.GetAllAsync(ct);
            return View(members);
        }

        // GET :: baseurl/members/details/{id} => get user info by id


        public async Task<IActionResult> MemberDetails(int id, CancellationToken ct)
        {
            var member = await _memService.GetMemberDetailsByIdAsync(id, ct);
            if(member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";

            }
            return View(member);
        }
        // GET :: baseurl/members/healthrecorddetails/{id} => get data + health record of member
        
        public async Task<IActionResult> HealthRecordDetails(int id, CancellationToken ct)
        {
            var record = await _memService.GetMemberHealthRecordAsync(id, ct);
            if(record is null)
            {
                TempData["ErrorMessage"] = "NoHealth Record Found";
                return RedirectToAction(nameof(Index));
            }
            return View(record);
        }

        // for images
        // action to get membersPhotos
        [HttpGet]
        public async Task<IActionResult> Picture(int id)
        {
            var member =  await _memService.GetMemberDetailsByIdAsync(id);
            if (member is null || String.IsNullOrWhiteSpace(member.Photo)) return NotFound();

            var result = _attachService.GetFile(member.Photo, "MembersPhotos");
            if (result is null) return NotFound();
            return File(result.Value.stream, result.Value.contentType);
        }

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

        [HttpGet]
        public async Task<IActionResult> EditMember(int id, CancellationToken ct)
        {
            var member = await _memService.GetMemberToUpdateAsync(id, ct);
            if(member == null)
            {
                TempData["ErrorMessage"] = "MemberNot Found";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // POST :: baseurl/members/edit/{member} => submit edit form
        [HttpPost]
        public async Task<IActionResult> EditMember([FromRoute] int id, MemberToUpdateViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _memService.UpdateMemberAsync(id, model, ct);
            if (result)
                TempData["SuccessMessage"] = "Member Updated Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Update Member";

            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Delete

        // GET :: baseurl/members/delete/{id} => show validation page
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var member = await _memService.GetMemberDetailsByIdAsync(id, ct);
            if(member is null)
            {
                TempData["ErrorMessage"] = "Member Not Found";
                return RedirectToAction(nameof(Index));
            }
            return View();

        }

        public async Task<IActionResult> DeleteConfirmed ([FromRoute]int id, CancellationToken ct)
        {
            var result = await _memService.DeleteMemberAsync(id, ct);

            if (result)
                TempData["SuccessMessage"] = "Member Deleted Successfully";
            else
                TempData["ErrorMessage"] = "Failed to Delete Member"; 
            return RedirectToAction(nameof(Index));
        }
        #endregion
        
    }
}
