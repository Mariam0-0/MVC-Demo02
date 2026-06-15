using GymManagement.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MVC01_Demo.Contexts;
using MVC01_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Classes
{
    public class PlanRepository : IPlanRepository
    {
        // 1. DB connection
        private readonly GymDbContext dbContext;
        public PlanRepository(GymDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> AddAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Plans.Add(plan);
            return await dbContext.SaveChangesAsync(ct);
        }

        public async Task<int> DeleteAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Plans.Remove(plan);
            return await dbContext.SaveChangesAsync(ct);

        }

        public async Task<IEnumerable<Plan>> GetAllAsync(bool tracking = false, CancellationToken ct = default)
        {
            //if (tracking)
            //    return await dbContext.Plans.ToListAsync(ct);
            //else
            //    return await dbContext.Plans.AsNoTracking().ToListAsync(ct);

            IQueryable<Plan> query = tracking? dbContext.Plans : dbContext.Plans.AsNoTracking();

            return await query.ToListAsync(ct);

        }

        public async Task<Plan?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            return await dbContext.Plans.FindAsync(id, ct);
        }

        public Task<int> UpdateAsync(Plan plan, CancellationToken ct = default)
        {
            dbContext.Plans.Update(plan);
            return dbContext.SaveChangesAsync(ct);
        }
    }
}
