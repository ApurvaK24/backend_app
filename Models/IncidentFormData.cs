using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace backend_app.Models
{
    public class IncidentFormData
    {
        public string Name { get; set; }
        public string Department { get; set; }
        public string PhoneNumber { get; set; }
        public string DateOfIncident { get; set; }
        public string TimeOfIncident { get; set; }
        public string Location { get; set; }
        public bool PoliceNotified { get; set; }
        public string IncidentDetails { get; set; }
        public string IncidentCauses { get; set; }
        public string FollowUpRecommendations { get; set; }
        public string AdditionalNotes { get; set; }

    }
}