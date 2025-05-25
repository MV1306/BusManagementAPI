using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BusManagementAPI.Models
{
    public class FareMaster_New
    {
        [Key]
        public Guid FareID { get; set; }
        public string BusType { get; set; } = string.Empty;
        public int Stage { get; set; }
        [Precision(10, 2)]
        public decimal StageFare { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
    }
}