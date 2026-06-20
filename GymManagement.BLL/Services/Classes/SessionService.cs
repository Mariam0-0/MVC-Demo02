using AutoMapper;
using AutoMapper.Execution;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.SessionViewModel;
using GymManagement.DAL.Models;
using GymManagement.DAL.Models.Enums;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagmemnt.BLL.ViewModels.SessionViewModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        public async Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct)
        {
            // use eager loading
            var sessions = await _unitOfWork.SessionRepository.GetSessionsWithTrainerAndCategory(ct);


            if (sessions == null || !sessions.Any()) return null;
            var mappedSession = sessions.Select(S => new SessionViewModel()
            {
                Id = S.Id,
                Capacity = S.Capacity,
                CategoryName = S.Category.CategoryName,
                TrainerName = S.Trainer.Name,
                StartDate = S.StartDate,
                EndDate = S.EndDate,
                Description = S.Description,
            });

            // booking slots
            foreach (var session in mappedSession)
            {
                session.AvailableSlots = session.Capacity - await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(session.Id, ct);

            }
            return mappedSession;
        }
        public async Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default)
        {
            // validations
            if (model.EndDate <= model.StartDate) return Result.Validation("End date must be greater than start date.");
            if (model.StartDate <= DateTime.Now) return Result.Validation("Start date has to be in future");
            if (model.Capacity < 1 || model.Capacity > 25) return Result.Validation("Capacity must be between 1 and 25");

            // get trainer
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if(trainer == null) return Result.NotFound("Trainer not found");

            // get category
            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(model.CategoryId);
            if (category == null) return Result.NotFound("Category not found");

            // check if trainer speciality == category specialty
            bool isValid = Enum.TryParse<Specialty>(category.CategoryName,true,out var categorySpecialty);
            if(!isValid || trainer.Specialty != categorySpecialty) return Result.Validation("Trainer and category must have the same specialty");

            // createsessionmodel => session
            var session = _mapper.Map<Session>(model);

            _unitOfWork.GetRepository<Session>().AddAsync(session);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ?Result.Ok(): Result.Fail("Failed to create session");

        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown(CancellationToken ct = default)
        {
            var result= await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(result);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDown(CancellationToken ct = default)
        {
            var result =  await _unitOfWork.GetRepository<Category>().GetAllAsync(ct: ct);
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(result);

        }

        public async Task<Result<SessionViewModel>> GetSessionByIdAsync(int sessionId, CancellationToken ct = default)
        {
            // get seesion
            var session = await _unitOfWork.SessionRepository.GetSessionByIdWithTrainerAndCategory(sessionId, ct);
            if (session is null)
                return Result<SessionViewModel>.NotFound("Session Not Found");

            else
            {
                // session => sessionViewModel
                var mappedSession = _mapper.Map<SessionViewModel>(session);
                // catName,TrainerName,AvailableSlots

                mappedSession.AvailableSlots = mappedSession.Capacity - await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(sessionId, ct);

                return Result<SessionViewModel>.Ok(mappedSession);
            }
        }

        public async Task<Result<UpdateSessionViewModel>> GetSessionToUpdate(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result<UpdateSessionViewModel>.NotFound("Session Not Found");
            
            if (session.StartDate <= DateTime.Now)
                return Result<UpdateSessionViewModel>.Fail("Cannot Update Ongoing Session");

            // cannot update session with booking
            var bookingCount = await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(sessionId, ct);
            if (bookingCount > 0)
                return Result<UpdateSessionViewModel>.Fail("Cannot update session already booked");

            // session => UpdateSessionViewModel
            var mappedSession = _mapper.Map<Session, UpdateSessionViewModel>(session);
            return Result<UpdateSessionViewModel>.Ok(mappedSession);
        }

        public async Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(id, ct);
            if (session is null) return Result.NotFound("Session Not Found");

            if (session.StartDate <= DateTime.Now)
                return Result.Fail("Cannot update session that already started");

            if (model.EndDate <= model.StartDate) return Result.Validation("End date must be after start date");

            var bookedCount = await _unitOfWork.SessionRepository.CountOfBookedSlotsAsync(id);
            if (bookedCount > 0)
                return Result.Fail("Cannot update session that is already booked");

            if (model.StartDate <= DateTime.Now)
                return Result.Validation("start date must be in the future");

            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(model.TrainerId);
            if (trainer is null) return Result.NotFound("Trainer Not Found");

            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);

            var isValid = Enum.TryParse<Specialty>(category?.CategoryName, true, out var categorySpeciality);
            if (!isValid || trainer.Specialty != categorySpeciality)
                return Result.Validation("Trainer and category not matched");

            // updateSessionViewModel => session map
            // reverse map works different (send src and dest only in circular brackets)
            _mapper.Map(model, session);

            session.UpdatedAt = DateTime.Now;

            _unitOfWork.SessionRepository.UpdateAsync(session);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update session"); 
        }

        public async Task<Result> DeleteSessionAsync(int sessionId, CancellationToken ct = default)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId, ct);
            if (session is null) return Result.NotFound("Session Not Found");
            if (session.EndDate >= DateTime.Now) return Result.Fail("Cannot delete ongoing session");

            _unitOfWork.GetRepository<Session>().DeleteAsync(session);
            var result = await _unitOfWork.SaveChangesAsync();
            return result > 0 ? Result.Ok() : Result.Fail("Failed to Delete Session");


        }
    }
}
