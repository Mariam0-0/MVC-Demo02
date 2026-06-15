using MVC01_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IPlanRepository
    {
        // getAll
        Task<IEnumerable<Plan>> GetAllAsync(bool tracking = false, CancellationToken ct = default);

        // getById
        Task<Plan?> GetByIdAsync(int id, CancellationToken ct = default);

        // add
        Task<int> AddAsync (Plan plan, CancellationToken ct = default);

        // update
        Task<int> UpdateAsync (Plan plan, CancellationToken ct = default);

        // delete
        Task<int> DeleteAsync(Plan plan, CancellationToken ct = default);
    }
}
