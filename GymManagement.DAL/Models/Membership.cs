using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Models
{
    public class Membership :BaseEntity
    {
        public Member Member { get; set; }
        public int MemberId { get; set; } //FK
        public Plan Plan { get; set; }
        public int PlanId { get; set; } // FK

        // startDate ==createdat

        public DateTime EndDate { get; set; }
        // id will be PK so that a member can have many memberships once a current one ends

        //RO props
        // will not be mapped into DB
        public string Status => EndDate > DateTime.Now ? "Active" : "Expired";
        public bool IsActive => EndDate > DateTime.Now;
    }
}
