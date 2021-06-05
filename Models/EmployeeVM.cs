using System;
namespace leave_management.Models
{
    public class EmployeeVM
    {
        public EmployeeVM()
        {
        }

        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string PhoneNumber { get; set; }

        public string Firstname { get; set; }

        public string Lastname { get; set; }

        public string TaxId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime DateJoined { get; set; }
    }
}
