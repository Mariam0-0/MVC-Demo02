using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using MVC01_Demo.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class UnitOfWork : IUnitOfWork
    {
        // DB Connection
        private readonly GymDbContext _dbContext;
        private readonly Dictionary<string, object> _repositories = [];
        public UnitOfWork(GymDbContext dbContext, 
            ISessionRepository sessionRepository,
            IBookingRepository bookingRepository,
            IMembershipRepository membershipRepository)
        {
            _dbContext = dbContext;
            SessionRepository = sessionRepository;
            BookingRepository = bookingRepository;
            MembershipRepository = membershipRepository;
        }

        public ISessionRepository SessionRepository { get; }
        public IBookingRepository BookingRepository { get; }
        public IMembershipRepository MembershipRepository { get; }


        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity, new()
        {
            // check if repo exist ??IDictionary<> of repos
            var typeName = typeof(TEntity).Name;

            // if name exist in dic
            if (_repositories.TryGetValue(typeName, out object? value))
                return (IGenericRepository<TEntity>)value;


            // if not exist, create, then add to dic, then return repo
            else
            {
                var repo = new GenericRepository<TEntity>(_dbContext);
                _repositories[typeName] = repo;
                return repo;
            }

        }

        public async Task<int> SaveChangesAsync(CancellationToken ct = default)
            => await _dbContext.SaveChangesAsync(ct);
    }
}
