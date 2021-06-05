using System;
using System.ComponentModel.DataAnnotations;

namespace leave_management.Models
{
    public class LeaveTypeVM
    {

        
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Default Number of Days")]
        [Range(1 , 31 , ErrorMessage = "Please enter a number less than 31.")]
        public int DefaultDays { get; set; }

        [Display(Name="Date Created")]

        public DateTime? DateCreated { get; set; }


    }


    
}
