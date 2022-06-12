using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ManagementContract.Models
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Surname { get; set; } = string.Empty;

        [EmailAddress]
        public string Mail { get; set; } = string.Empty; 
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public string RodneCislo { get; set; } = string.Empty;

        public int Age { get; set; }


        
    }
}