using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : BaseEntity, new()
    {
        
        // getAll
        Task<IEnumerable<TEntity>> GetAllAsync(bool tracking = false, CancellationToken ct = default);

        // getById
        Task<TEntity?> GetByIdAsync(int id, CancellationToken ct = default);

        // add
        void AddAsync(TEntity entity);

        // update
        void UpdateAsync(TEntity entity);

        // delete
        void DeleteAsync(TEntity entity);



        // check
        Task<bool> AnyAsync(Expression<Func<TEntity,bool>> predicate,CancellationToken ct = default);


        // check if user has active membership
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool tracking = false, CancellationToken ct = default);

        Task<IEnumerable<TEntity>> GetAllExpAsync(Expression<Func<TEntity, bool>>? predicate = null, bool tracking = false, CancellationToken ct = default);

        //
        Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken ct = default);

    }
}
