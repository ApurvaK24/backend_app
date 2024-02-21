using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using backend_app.Models;



namespace backend_app.Controllers
{
    [RoutePrefix("api/Incident")]
    public class IncidentController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
        SqlCommand cmd = null;

        [HttpPost]
        [Route("SubmitReport")]
        public string SubmitReport(IncidentFormData formData)
        {
            string msg = string.Empty;
            try
            {
                // Parse DateOfAccident and TimeOfAccident
                DateTime dateOfIncident = DateTime.Parse(formData.DateOfIncident);
                TimeSpan timeOfIncident = TimeSpan.Parse(formData.TimeOfIncident);

                cmd = new SqlCommand("uspic_SubmitReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", formData.Name);
                cmd.Parameters.AddWithValue("@Department", formData.Department);
                cmd.Parameters.AddWithValue("@PhoneNumber", formData.PhoneNumber);
                cmd.Parameters.AddWithValue("@DateOfIncident", dateOfIncident);
                cmd.Parameters.AddWithValue("@TimeOfIncident", timeOfIncident);
                cmd.Parameters.AddWithValue("@Location", formData.Location);
                cmd.Parameters.AddWithValue("@PoliceNotified", formData.PoliceNotified);
                cmd.Parameters.AddWithValue("@IncidentDetails", formData.IncidentDetails);
                cmd.Parameters.AddWithValue("@IncidentCauses", formData.IncidentCauses);
                cmd.Parameters.AddWithValue("@FollowUpRecommendations", formData.FollowUpRecommendations);
                cmd.Parameters.AddWithValue("@AdditionalNotes", formData.AdditionalNotes);

                conn.Open();
                int i = cmd.ExecuteNonQuery();
                conn.Close();

                if (i > 0)
                {
                    msg = "Report submitted successfully.";
                }
                else
                {
                    msg = "Error submitting report.";
                }
            }
            catch (Exception ex)
            {
                msg = $"Error: {ex.Message}";
            }

            return msg;
        }

        /*[HttpGet]
        [Route("GetIncidentCount")]
        public int GetIncidentCount()
        {
            int IncidentCnt;
            try
            {

                cmd = new SqlCommand("usp_GetIncidentCount", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                IncidentCnt = cmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return IncidentCnt;
        }*/

        [HttpGet]
        [Route("GetIncidentCount")]
        public int GetIncidentCount()
        {
            int incidentCnt = 0; // Initialize incidentCnt to 0

            try
            {
                
                    using (SqlCommand cmd = new SqlCommand("usp_GetIncidentCount", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        conn.Open();
                        // Use ExecuteScalar to retrieve a single value
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                        {
                            // If the result is not null or DBNull, convert it to int
                            incidentCnt = Convert.ToInt32(result);
                        }
                    }
                
            }
            catch (Exception ex)
            {
                // Handle exception appropriately (logging, error response, etc.)
                Console.WriteLine("Error fetching incident count: " + ex.Message);
                throw; // Re-throw the exception
            }

            return incidentCnt;
        }



        [HttpGet]
        [Route("GetMonthlyIncidentCounts")]
        public IHttpActionResult GetMonthlyAccidentCounts()
        {
            List<MonthlyIncidentCount> monthlyCounts = new List<MonthlyIncidentCount>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    conn.Open();

                    using (SqlCommand cmd = new SqlCommand("usp_IncMonthCount", conn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MonthlyIncidentCount monthlyCount = new MonthlyIncidentCount
                                {
                                    Month = reader.GetInt32(0),
                                    MonthName = reader.GetString(1),
                                    IncidentCount = reader.GetInt32(2)
                                };
                                monthlyCounts.Add(monthlyCount);
                            }
                        }
                    }
                }

                return Ok(monthlyCounts);
            }
            catch (Exception ex)
            {
                // Handle exception appropriately (logging, error response, etc.)
                Console.WriteLine("Error fetching monthly accident counts: " + ex.Message);
                return InternalServerError();
            }
        }
    }
}
