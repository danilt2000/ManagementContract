using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementContract.Models
{
    public class Contract
    {
        public int Id { get; set; }

        public int RegistrationNumber { get; set; }

        public string Institution { get; set; } = string.Empty;

        public int ClientId { get; set; }

        public DateTime? ClosingDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? TerminationDate { get; set; }

        public List<ConsultantContract> Consultants { get; set; } = new();



    }
}
