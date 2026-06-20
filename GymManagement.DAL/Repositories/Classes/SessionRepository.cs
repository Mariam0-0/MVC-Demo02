using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MVC01_Demo.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDbContext _dbContext;
        public SessionRepository(GymDbContext dbContext):base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CountOfBookedSlotsAsync(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Bookings.AsNoTracking().CountAsync(B => B.SessionId == sessionId);
        }

        public async Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(Expression<Func<Session, bool>>? predicate = null, CancellationToken ct = default)
        {
            IQueryable<Session> query = _dbContext.Sessions
                                                  .AsNoTracking()
                                                  .Include(S => S.Trainer)
                                                  .Include(S => S.Category);
            if(predicate is not null) query = query.Where(predicate);
            return await query.ToListAsync();
        }


        public async Task<IEnumerable<Session>> GetSessionsWithTrainerAndCategory(CancellationToken ct = default)
        {
            var query = _dbContext.Sessions.AsNoTracking().Include(x => x.Trainer).Include(x=>x.Category);
            return await query.ToListAsync();
        }
        public async Task<Session> GetSessionByIdWithTrainerAndCategory(int sessionId, CancellationToken ct = default)
        {
            return await _dbContext.Sessions
                                    .AsNoTracking()
                                    .Include(S => S.Trainer)
                                    .Include(S => S.Category)
                                    .FirstOrDefaultAsync(S => S.Id == sessionId);
        }
    }
}
