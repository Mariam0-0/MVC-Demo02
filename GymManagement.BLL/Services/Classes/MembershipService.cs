using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class MembershipService : IMembershipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MembershipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;

        }
        public async Task<Result> CreateMembershipAsync(CreateMemberShipViewModel model, CancellationToken ct = default)
        {
            // check if member exists first
            var memberExists = await _unitOfWork.GetRepository<Member>().AnyAsync(m => m.Id == model.MemberId, ct);
            if (!memberExists)
                return Result.NotFound("Member not found");

            // check if plan exists and active
            var plan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(model.PlanId, ct);
            if (plan is null)
                return Result.NotFound("Plan not found");

            if (!plan.IsActive)
                return Result.Fail("Plan is inactive");

            // check if member has any active memberships
            var hasActive = await _unitOfWork.MembershipRepository.AnyAsync(m => m.MemberId == model.MemberId && m.EndDate> DateTime.Now, ct);
            if (hasActive)
                return Result.Fail("Member already has an active membership");

            var entity = new Membership
            {
                MemberId = model.MemberId,
                PlanId = model.PlanId,
                CreatedAt = DateTime.Now,
                EndDate = (model.StartDate ?? DateTime.Now).AddDays(plan.DurationDays),
            };

            _unitOfWork.MembershipRepository.AddAsync(entity);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to create new membership");
        }

        public async Task<Result> DeleteActiveMembershipAsync(int memberId, CancellationToken ct = default)
        {
            var active = await _unitOfWork.MembershipRepository.FirstOrDefaultAsync(
                                M => M.MemberId == memberId && M.EndDate > DateTime.Now, tracking: true, ct: ct);

            if (active == null) return Result.NotFound("No active membership found for this member");

            _unitOfWork.MembershipRepository.DeleteAsync(active);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to delete membership");
        }

        public async Task<IEnumerable<MemberShipViewModel>> GetAllMembershipsAsync(CancellationToken ct = default)
        {
            var memberships = await _unitOfWork.MembershipRepository.GetAllMembershipsWithMemberAndPlanAsync(m => m.EndDate > DateTime.Now, ct);
            return _mapper.Map<IEnumerable<MemberShipViewModel>>(memberships);
        }

        public async Task<IEnumerable<MemberSelectListViewModel>> GetMembersForDropDownAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<MemberSelectListViewModel>>(members);
        }

        public async Task<IEnumerable<PlanSelectListViewModel>> GetPlansForDropDownAsync(CancellationToken ct = default)
        {
            var plans = await _unitOfWork.GetRepository<Plan>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<PlanSelectListViewModel>>(plans);
        }
    }
}
