using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northstar.Models
{
    public class Company : NamedModel
    {
        public string? StreetAddress { get; set; }
        public string? PhoneNumber { get; set; }
        [NotMapped] public int EmployeeCount { get; set; }
    }
}
