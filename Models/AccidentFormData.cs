using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_app.Models
{
    public class AccidentFormData
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfAccident { get; set; }
        public string TimeOfAccident { get; set; }
        public string Location { get; set; }
        public bool PoliceNotified { get; set; }
        public string AccidentDetails { get; set; }
        public string AccidentCauses { get; set; }
        public string FollowUpRecommendations { get; set; }
        public string AdditionalNotes { get; set; }

    }
}