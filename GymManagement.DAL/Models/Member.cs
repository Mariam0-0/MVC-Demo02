using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class Member :GymUser
    {
        public string? Photo {  get; set; }

        // join date = createdAt of base entity

        #region Relationships

        public HealthRecord HealthRecord { get; set; } = default;


        public ICollection<Membership> Plans { get; set; }

        public ICollection<Booking> MemberSessions { get; set; }

        #endregion
    }
}
