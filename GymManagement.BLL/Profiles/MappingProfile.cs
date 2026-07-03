using AutoMapper;
using GymManagement.BLL.ViewModels.MembershipViewModels;
using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.BLL.ViewModels.PlanViewModels;
using GymManagement.BLL.ViewModels.SessionViewModel;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagmemnt.BLL.ViewModels.SessionViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Profiles
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            MemberProfiles();
            SessionProfiles();
            PlanProfiles();
            TrainerProfile();
            MembershipsProfile();
        }
        private void MemberProfiles()
        {
            CreateMap<Member, MemberViewModel>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()));


            CreateMap<HealthRecord, HealthRecordViewModel>().ReverseMap();

            CreateMap<Member, MemberToUpdateViewModel>()
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber));

            CreateMap<MemberToUpdateViewModel, Member>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Photo, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())   // Ignore Address, handled in AfterMap
                .AfterMap((src, dest) =>
                {
                    // ✅ Editable fields
                    dest.Email = src.Email;
                    dest.Phone = src.Phone;  // ✅ Now Phone updates!

                    // ✅ Address fields
                    if (dest.Address == null)
                    {
                        dest.Address = new Address();
                    }
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.City = src.City;
                    dest.Address.Street = src.Street;
                });

            CreateMap<CreateMemberViewModel, Member>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address()
                {
                    BuildingNumber = src.BuildingNumber,
                    City = src.City,
                    Street = src.Street,
                }))
                .ForMember(dest => dest.HealthRecord, opt => opt.MapFrom(src => src.HealthRecordViewModel))
                ;
        }
            
        private void SessionProfiles()
        {
            CreateMap<CreateSessionViewModel, Session>();
            CreateMap<Category, CategorySelectViewModel>();
            CreateMap<Trainer, TrainerSelectViewModel>();

            CreateMap<Session, SessionViewModel>()
                .ForMember(dest => dest.TrainerName, opt => opt.MapFrom(src => src.Trainer.Name))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
                ;

            CreateMap<Session, UpdateSessionViewModel>().ReverseMap();
        }
        private void PlanProfiles()
        {
            CreateMap<Plan, PlanViewModel>();
            CreateMap<Plan, UpdatePlanViewModel>().ForMember(dest => dest.PlanName, opt => opt.MapFrom(src => src.Name));
            CreateMap<UpdatePlanViewModel, Plan>()
           .ForMember(dest => dest.Name, opt => opt.Ignore())
           .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.Now));

        }

        private void TrainerProfile()
        {
            // CREATE: ViewModel -> Entity
            CreateMap<CreateTrainerViewModel, Trainer>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => new Address
                {
                    BuildingNumber = src.BuildingNumber,
                    Street = src.Street,
                    City = src.City
                }));

            // READ: Entity -> ViewModel
            CreateMap<Trainer, TrainerViewModel>()
                .ForMember(dest => dest.Address,
                    opt => opt.MapFrom(src => $"{src.Address.BuildingNumber} - {src.Address.Street} - {src.Address.City}"))
                .ForMember(dest => dest.DateOfBirth,
                    opt => opt.MapFrom(src => src.DateOfBirth.ToShortDateString()))
                .ForMember(dest => dest.Gender,
                    opt => opt.MapFrom(src => src.Gender.ToString()));

            // UPDATE READ: Entity -> ViewModel
            CreateMap<Trainer, TrainerToUpdateViewModel>()
                .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address.Street))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.Address.City))
                .ForMember(dest => dest.BuildingNumber, opt => opt.MapFrom(src => src.Address.BuildingNumber));

            // UPDATE WRITE: ViewModel -> Entity
            CreateMap<TrainerToUpdateViewModel, Trainer>()
                .ForMember(dest => dest.Name, opt => opt.Ignore())
                .ForMember(dest => dest.Address, opt => opt.Ignore())
                .AfterMap((src, dest) =>
                {
                    dest.Email = src.Email;
                    dest.Phone = src.Phone;
                    dest.Specialty = src.Specialty;

                    if (dest.Address == null)
                    {
                        dest.Address = new Address();
                    }
                    dest.Address.BuildingNumber = src.BuildingNumber;
                    dest.Address.City = src.City;
                    dest.Address.Street = src.Street;
                    dest.UpdatedAt = DateTime.Now;
                });
        }


        private void MembershipsProfile()
        {
            CreateMap<Membership, MemberShipForMemberViewModel>()
                     .ForMember(dist => dist.MemberName, Option => Option.MapFrom(Src => Src.Member.Name))
                     .ForMember(dist => dist.PlanName, Option => Option.MapFrom(Src => Src.Plan.Name))
                     .ForMember(dist => dist.StartDate, Option => Option.MapFrom(X => X.CreatedAt));

            CreateMap<Membership, MemberShipViewModel>()
                     .ForMember(dist => dist.MemberName, Option => Option.MapFrom(Src => Src.Member.Name))
                     .ForMember(dist => dist.PlanName, Option => Option.MapFrom(Src => Src.Plan.Name))
                                          .ForMember(dist => dist.StartDate, Option => Option.MapFrom(X => X.CreatedAt));

            CreateMap<CreateMemberShipViewModel, Membership>();
            CreateMap<Member, MemberSelectListViewModel>();
            CreateMap<Plan, PlanSelectListViewModel>();
        }
    }
}
