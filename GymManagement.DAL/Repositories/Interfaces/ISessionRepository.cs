using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface ISessionRepository :IGenericRepository<Session>
    {
        Task<IEnumerable<Session>> GetSessionsWithTrainerAndCategory(CancellationToken ct = default);

        Task<int> CountOfBookedSlotsAsync (int sessionId,CancellationToken ct = default);

        Task<IEnumerable<Session>> GetAllSessionsWithTrainerAndCategoryAsync(
            Expression<Func<Session, bool>>? predicate = null,
            CancellationToken ct = default);

        Task<Session> GetSessionByIdWithTrainerAndCategory(int sessionId,CancellationToken ct = default);

    }
}
