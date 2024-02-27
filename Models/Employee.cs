
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_app.Models
{
    public class Employee
    {
        /*public int Id { get; set; }
        public string Name { get; set; }
        public string EmployeeId { get; set; } // Updated from PhoneNo
        public string Password { get; set; } // Updated from Address
        public string ReenteredPassword { get; set; }
        public string SelectedOption { get; set; }*/

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; } // Updated from PhoneNo
        public string Password { get; set; } // Updated from Address
        public string ReenteredPassword { get; set; }
        public string SelectedOption { get; set; }

    }
}