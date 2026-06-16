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
    }
}
