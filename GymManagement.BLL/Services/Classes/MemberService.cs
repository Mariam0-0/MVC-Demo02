using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
 
    public class MemberService : IMemberService
    {
        //DB connection
        private readonly IGenericRepository<Member> _memberRepo;
        public MemberService(IGenericRepository<Member> memberRepo)
        {
            _memberRepo = memberRepo;
        }

        public async Task<bool> CreateMemberAsync(CreateMemberViewModel model, CancellationToken ct = default)
        {
            // email & phone exists or not
            var emailExist = await _memberRepo.AnyAsync(X => X.Email == model.Email);
            var phoneExist = await _memberRepo.AnyAsync(X => X.Phone == model.Phone);

            if (emailExist || phoneExist) return false;

            var member = new Member()
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                Address = new Address()
                {
                    BuildingNumber = model.BuildingNumber,
                    City = model.City,
                    Street = model.Street,
                },
                HealthRecord = new HealthRecord()
                {
                    BloodType = model.HealthRecordViewModel.BloodType,
                    Height = model.HealthRecordViewModel.Height,
                    Weight = model.HealthRecordViewModel.Weight,
                    Note = model.HealthRecordViewModel.Note,
                }
            };

            var result = await _memberRepo.AddAsync(member);
            return result > 0;



        }

        public async Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default)
        {
            var members = await _memberRepo.GetAllAsync(ct: ct);

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
    }
}
