using GymManagement.BLL.ViewModels.MemberViewModels;
using GymManagement.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.Services.Interfaces
{
    public interface IMemberService
    {
        //get all
        Task<IEnumerable<MemberViewModel>> GetAllAsync(CancellationToken ct = default);

        // create member
        Task<bool> CreateMemberAsync(CreateMemberViewModel member, CancellationToken ct = default);

        // get member details
        Task<MemberViewModel> GetMemberDetailsByIdAsync(int memberId, CancellationToken ct = default);

        // get member health record
        Task<HealthRecordViewModel> GetMemberHealthRecordAsync(int memberId, CancellationToken ct = default);

        // get member to update
        Task<MemberToUpdateViewModel> GetMemberToUpdateAsync(int memberId, CancellationToken ct = default);

        // update member info
        Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel model, CancellationToken ct = default);

        // delete member
        Task<bool> DeleteMemberAsync(int memberId, CancellationToken ct = default);
    }


}
