using GymManagement.DAL.Models;
using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MVC01_Demo.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly GymDbContext _dbContext;
        private readonly DbSet<Booking> _set;


        public BookingRepository(GymDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
            _set = dbContext.Set<Booking>();

        }

        public async Task<IEnumerable<Booking>> GetAllAsync(System.Linq.Expressions.Expression<Func<Booking, bool>>? predicate = null, bool tracking = false, CancellationToken ct = default)
        {
            IQueryable<Booking> query = tracking ? _set : _set.AsNoTracking();
            if (predicate is not null) query = query.Where(predicate);
            return await query.ToListAsync(ct);
        }

        public Task<List<Booking>> GetBySessionIdAsync(int sessionId, CancellationToken ct = default)
            => _dbContext.Bookings.AsNoTracking().Include(b => b.Member).Where(b => b.SessionId == sessionId).ToListAsync(ct);


    }
}
