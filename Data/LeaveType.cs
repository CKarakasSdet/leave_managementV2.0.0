using System;
using System.ComponentModel.DataAnnotations;

namespace leave_management.Data
{
    public class LeaveType
    {
        public LeaveType()
        { }


        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int DefaultDays { get; set; }

        public DateTime DateCreated { get; set; }


    }

}
