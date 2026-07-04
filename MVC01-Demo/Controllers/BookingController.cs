using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVC01_Demo.Controllers
{
    [Authorize]

    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            return View(await _bookingService.GetAllSessionsAsync(ct));
        }

        // get members
        [HttpGet]
        public async Task<IActionResult> GetMembersForUpcomingSession(int id, CancellationToken ct)
            => View(await _bookingService.GetMembersForUpcomingBySessionIdAsync(id, ct));


        [HttpGet]
        public async Task<IActionResult> GetMembersForOngoingSession(int id, CancellationToken ct)
                => View(await _bookingService.GetMembersForOngoingBySessionIdAsync(id, ct));



        // create
        [HttpGet]
        public async Task<IActionResult> Create(int id, CancellationToken ct)
        {
            var members = await _bookingService.GetMembersForDropDownAsync(id, ct);

            // ✅ Check if members exist
            if (members == null || !members.Any())
            {
                TempData["ErrorMessage"] = "No available members to book.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.SessionId = id;

            var model = new CreateBookingViewModel
            {
                SessionId = id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingViewModel model, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdown if validation fails
                var members = await _bookingService.GetMembersForDropDownAsync(model.SessionId, ct);
                ViewBag.Members = new SelectList(members, "Id", "Name");
                ViewBag.SessionId = model.SessionId;
                return View(model);
            }


            var result = await _bookingService.CreateNewBookingAsync(model, ct);

            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
                result.success ? "Booking created successfully" : result.error;

            if (!result.success)
            {
                // Repopulate dropdown if booking fails
                var members = await _bookingService.GetMembersForDropDownAsync(model.SessionId, ct);
                ViewBag.Members = new SelectList(members, "Id", "Name");
                ViewBag.SessionId = model.SessionId;
                return View(model);
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = model.SessionId });
        }


        // cancel booking
        [HttpPost]
        public async Task<IActionResult> Cancel(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.CancelBookingAsync(memberId, sessionId, ct);
            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
                result.success ? "Booking Cancelled successfully." : result.error;

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });

        }

        // mark as attended
        [HttpPost]
        public async Task<IActionResult> Attended(int memberId, int sessionId, CancellationToken ct)
        {
            var result = await _bookingService.MarkAttendedAsync(memberId, sessionId, ct);
            

            TempData[result.success ? "SuccessMessage" : "ErrorMessage"] =
               result.success ? "Attendance recorded successfully." : result.error;

            return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });

        }
    }
}
