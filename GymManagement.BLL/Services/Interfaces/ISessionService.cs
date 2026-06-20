using GymManagement.BLL.Common;
using GymManagement.BLL.ViewModels.SessionViewModel;
using GymManagement.DAL.Models;
using GymManagmemnt.BLL.ViewModels.SessionViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface ISessionService
    {
        // get all sessions
        Task<IEnumerable<SessionViewModel>> GetAllSessionsAsync(CancellationToken ct);


        // create session
        Task<Result> CreateSessionAsync(CreateSessionViewModel model, CancellationToken ct = default);


        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDown(CancellationToken ct = default);
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDown(CancellationToken ct = default);

        // get session detaild
        Task<Result<SessionViewModel>> GetSessionByIdAsync (int sessionId,CancellationToken ct = default);

        // update session
        Task<Result<UpdateSessionViewModel>> GetSessionToUpdate(int sessionId, CancellationToken ct = default);

        Task<Result> UpdateSessionAsync(int id, UpdateSessionViewModel model, CancellationToken ct = default);

        // delete
        Task<Result> DeleteSessionAsync(int sessionId, CancellationToken ct = default);

    }
}
