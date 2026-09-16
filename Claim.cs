using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaimDataManager
{
    public class Claim
    {
        public int ClaimId { get; set; }

        public string LecturerName { get; set; } = string.Empty;

        public string ModuleCode { get; set; } = string.Empty;

        public decimal HoursWorked { get; set; }

        public decimal HourlyRate { get; set; }

        public string ClaimMonth { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;

        // Calculates the total claim amount.
        public decimal TotalAmount
        {
            get
            {
                return HoursWorked * HourlyRate;
            }
        }
    }
}