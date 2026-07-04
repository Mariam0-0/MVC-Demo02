using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.BookingViewModels;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.SessionViewModel;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<Result> CancelBookingAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session is null)
                return Result.NotFound("Session not found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot cancel a booking for a session that has already started");
            
            var booking  = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(B => B.SessionId == sessionId && B.MemberId == memberId, tracking: true, ct:ct);

            if (booking is null)
                return Result.NotFound("Booking not found");

            _unitOfWork.BookingRepository.DeleteAsync(booking);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Booking Cancellation Failed");
        }

        public async Task<Result> CreateNewBookingAsync(CreateBookingViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(model.SessionId,ct);
            if (session is null)
                return Result.NotFound("Session not found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot cancel a booking for a session that has already started");


            var hasActiveMembership = await _unitOfWork.MembershipRepository
               .AnyAsync(m => m.MemberId == model.MemberId && m.EndDate > DateTime.Now, ct);
            if (!hasActiveMembership)
                return Result.Fail("Member does not have an active membership");

            // prevent double booking
            var bookedBefore = await _unitOfWork.BookingRepository.AnyAsync(B => B.SessionId == model.SessionId && B.MemberId == model.MemberId, ct);
            if (bookedBefore)
                return Result.Fail("Member already booked this session before");

            var bookedCount = await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(model.SessionId, ct);
            if (bookedCount > session.Capacity)
                return Result.Fail("Session is full");

            _unitOfWork.BookingRepository.AddAsync(new Booking
            {
                SessionId = model.SessionId,
                MemberId = model.MemberId,
                IsAttended = false,
                CreatedAt = DateTime.Now,
            });

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed To Book Session");

        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.SessionRepository.GetAllSessionsWithTrainerAndCategoryAsync(x => x.EndDate >= DateTime.Now);
            if (!bookings.Any()) 
                return null!;


            var MappedSession = _mapper.Map<IEnumerable<SessionViewModel>>(bookings);
            foreach (var item in MappedSession)
            {
                item.AvailableSlots = item.Capacity - await _unitOfWork.SessionRepository.GetCountOfBookedSlotsAsync(item.Id);
            }

            return MappedSession;
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(int sessionId, CancellationToken ct = default)
        {
            // Get all bookings for this session
            var bookings = await _unitOfWork.BookingRepository.GetAllExpAsync(
                b => b.SessionId == sessionId,
                tracking: false,
                ct: ct);

            var bookedMemberIds = bookings.Select(b => b.MemberId).ToList();

            // Get all members
            var allMembers = await _unitOfWork.GetRepository<Member>().GetAllAsync(tracking: false, ct: ct);

            // Filter available members in memory
            var availableMembers = allMembers.Where(m => !bookedMemberIds.Contains(m.Id)).ToList();

            // Map to view model
            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(availableMembers);
        }


        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForOngoingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = sessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
                IsAttended = b.IsAttended,
            }).ToList();
        }

        public async Task<IEnumerable<MemberForSessionViewModel>> GetMembersForUpcomingBySessionIdAsync(int sessionId, CancellationToken ct = default)
        {
            var bookings = await _unitOfWork.BookingRepository.GetBySessionIdAsync(sessionId, ct);
            return bookings.Select(b => new MemberForSessionViewModel
            {
                MemberId = b.MemberId,
                SessionId = sessionId,
                MemberName = b.Member.Name,
                BookingDate = b.CreatedAt.ToString("yyyy-MM-dd HH:mm"),
            }).ToList();
        }

        public async Task<Result> MarkAttendedAsync(int memberId, int sessionId, CancellationToken ct = default)
        {
            var booking = await _unitOfWork.BookingRepository.FirstOrDefaultAsync(b => b.MemberId == memberId && b.SessionId == sessionId, tracking: true, ct: ct);
            if (booking is null) return Result.NotFound("Booking not found.");

            booking.IsAttended = true;
            booking.UpdatedAt = DateTime.Now;
            _unitOfWork.BookingRepository.UpdateAsync(booking);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Mark As Attended");

        }
    }
}
