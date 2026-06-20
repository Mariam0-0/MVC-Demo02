using AutoMapper;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
 
    public class MemberService : IMemberService
    {
        //DB connection
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAttachmentService _attachmentService;

        public MemberService(IUnitOfWork unitOfWork, IMapper mapper, IAttachmentService attachmentService) {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _attachmentService = attachmentService;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // email & phone exists or not
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(X => X.Email == model.Email);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(X => X.Phone == model.Phone);

            if (emailExist || phoneExist) return false;

            // upload photo
            var storedPhotoName = await _attachmentService.UploadAsync(model.PhotoFile.OpenReadStream(), model.PhotoFile.FileName, "MembersPhotos");
            if(String.IsNullOrWhiteSpace(storedPhotoName)) return false;


            // creatememberviewmodel => member
            var member = _mapper.Map<Member>(model);
            member.Photo = storedPhotoName;

            _unitOfWork.GetRepository<Member>().AddAsync(member);
            var result = await _unitOfWork.SaveChangesAsync();
            if (result > 0) return true;
            else
            {
                // delete photo
                _attachmentService.Delete(storedPhotoName, "MembersPhotos");
                return false;
            }


        }

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var members = await _unitOfWork.GetRepository<Member>().GetAllAsync(ct: ct);

            if (!members.Any()) return [];

            // if data then send to view model
            List<MemberViewModel> memberVM = new List<MemberViewModel>();

            foreach (var member in members)
            {
                var memberViewModel = new MemberViewModel()
                {
                    Name = member.Name,
                    Email = member.Email,
                    Phone = member.Phone,
                    Photo = member.Photo,
                    Id = member.Id,
                    Gender = member.Gender.ToString(),

                };
                memberVM.Add(memberViewModel);
                
            }
            return memberVM;

        }

        public async Task<MemberViewModel> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default)
        {
            // get member
            var member =  await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct: ct);

            if (member == null) return null;

            // table member => return memberviewmodel

            var model = _mapper.Map<Member,MemberViewModel>(member);


            // check if use has active plan/membership
            var activeMembership = await _unitOfWork.GetRepository<Membership>().FirstOrDefaultAsync(X => X.MemberId == memberId && X.EndDate > DateTime.Now);
            if(activeMembership is not null)
            {
                var activePlan = await _unitOfWork.GetRepository<Plan>().GetByIdAsync(activeMembership.PlanId, ct: ct);
                model.PlanName = activePlan.Name;
                model.MembershipStartDate = activeMembership.CreatedAt.ToString();
                model.MembershipEndDate = activeMembership.EndDate.ToString();

            }
            return model;

            
        }

        public async Task<HealthRecordViewModel> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default)
        {
            var record = await _unitOfWork.GetRepository<HealthRecord>().FirstOrDefaultAsync(X=>X.MemberId == memberId, ct:ct);
            if (record is null) return null;
            else
            {
                // mep from health table => healthrecordviewmodel
                return _mapper.Map<HealthRecordViewModel>(record);
            }
            
        }

        public async Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct: ct);
            if (member is null) return null;
            else
            {
                return _mapper.Map<MemberToUpdateViewModel>(member);
            }
        }

        public async Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct = default)
        {
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);

            // check if mail and phone still unique
            var emailExist = await _unitOfWork.GetRepository<Member>().AnyAsync(X=>X.Email == model.Email && X.Id != memberId);
            var phoneExist = await _unitOfWork.GetRepository<Member>().AnyAsync(X=>X.Phone == model.Phone && X.Id != memberId);

            if (emailExist || phoneExist) return false;

            // membertoupdatemodel => member
            _mapper.Map<Member>(model);
            member.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<Member>().UpdateAsync(member);
            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0;
            
        }
        public async Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default)
        {
            // if member has active booking
            var member = await _unitOfWork.GetRepository<Member>().GetByIdAsync(memberId, ct);

            if (member is null) return false;

            // date validation will throw exception => will be handled later
            var hasActiveBooking = await _unitOfWork.GetRepository<Booking>().AnyAsync(X => X.MemberId == memberId && X.Session.StartDate > DateTime.Now);
            if (hasActiveBooking) return false;
            _unitOfWork.GetRepository<Member>().DeleteAsync(member);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0;
            
        }

    }
}
