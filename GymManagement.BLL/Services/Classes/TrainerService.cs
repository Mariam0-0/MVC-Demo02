using AutoMapper;
using GymManagement.BLL.Common;
using GymManagement.BLL.Services.Interfaces;
using GymManagement.BLL.ViewModels.TrainerViewModels;
using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Classes
{
    public class TrainerService : ITrainerService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            
        }
        public async Task<Result> CreateTrainerAsync(CreateTrainerViewModel model, CancellationToken ct = default)
        {
            var repo =  _unitOfWork.GetRepository<Trainer>();

            // check unique email and phone
            if (await repo.AnyAsync(T => T.Email == model.Email, ct))
                return Result.Fail("A Trainer with this email already exists");

            if (await repo.AnyAsync(T => T.Phone == model.Phone, ct))
                return Result.Fail("A Trainer with this phone already exists");

            var entity = _mapper.Map<Trainer>(model);
            repo.AddAsync(entity);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to create trainer");
            
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync(CancellationToken ct = default)
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(ct:ct);
            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            return trainer is null ? null : _mapper.Map<TrainerViewModel>(trainer); 
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId, CancellationToken ct = default)
        {
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId, ct);
            return trainer is null ? null : _mapper.Map<TrainerToUpdateViewModel>(trainer);

        }

        public async Task<Result> RemoveTrainerAsync(int trainerId, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<Trainer>();
            var trainer = await repo.GetByIdAsync(trainerId, ct);

            if (trainer is null)
                return Result.NotFound("Trainer Not Found");

            // can't delete if trainer has future session assigned
            var hasFutureSession = await _unitOfWork.GetRepository<Session>()
                                .AnyAsync(s=> s.TrainerId == trainerId && s.StartDate > DateTime.Now, ct);

            if (hasFutureSession)
                return Result.Fail("Cannot delete a trainer with assigned future sessions");

            repo.DeleteAsync(trainer);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result.Ok();
        }

        public async Task<Result> UpdateTrainerDetailsAsync(int trainerId, TrainerToUpdateViewModel model, CancellationToken ct = default)
        {
            var repo = _unitOfWork.GetRepository<Trainer>();
            var trainer = await repo.GetByIdAsync(trainerId, ct);

            if (trainer is null)
                return Result.NotFound("Trainer not found");


            if (await repo.AnyAsync(T => T.Email == model.Email && T.Id != trainerId, ct))
                return Result.Fail("A Trainer with this email already exists");

            if (await repo.AnyAsync(T => T.Phone == model.Phone && T.Id != trainerId, ct))
                return Result.Fail("A Trainer with this phone already exists");

            _mapper.Map(model,trainer);
            trainer.UpdatedAt = DateTime.Now;
            repo.UpdateAsync(trainer);

            var result = await _unitOfWork.SaveChangesAsync(ct);
            return result > 0 ? Result.Ok() : Result.Fail("Failed to update trainer");

        }
    }
}
