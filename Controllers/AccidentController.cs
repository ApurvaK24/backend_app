using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Net.Mail;
using Newtonsoft.Json;
using backend_app.Models;

namespace backend_app.Controllers
{
    [RoutePrefix("api/Accident")]
    public class AccidentController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
        SqlCommand cmd = null;

        [HttpPost]
        [Route("SubmitReport")]
        public string SubmitReport(AccidentFormData formData)
        {
            string msg = string.Empty;
            try
            {
                // Parse DateOfAccident and TimeOfAccident
                DateTime dateOfAccident = DateTime.Parse(formData.DateOfAccident);
                TimeSpan timeOfAccident = TimeSpan.Parse(formData.TimeOfAccident);

                cmd = new SqlCommand("usp_SubmitReport", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", formData.Name);
                cmd.Parameters.AddWithValue("@Department", formData.Department);
                cmd.Parameters.AddWithValue("@PhoneNumber", formData.PhoneNumber);
                cmd.Parameters.AddWithValue("@DateOfAccident", dateOfAccident);
                cmd.Parameters.AddWithValue("@TimeOfAccident", timeOfAccident);
                cmd.Parameters.AddWithValue("@Location", formData.Location);
                cmd.Parameters.AddWithValue("@PoliceNotified", formData.PoliceNotified);
                cmd.Parameters.AddWithValue("@AccidentDetails", formData.AccidentDetails);
                cmd.Parameters.AddWithValue("@AccidentCauses", formData.AccidentCauses);
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

        [HttpGet]
        [Route("GetAccidentCount")]
        public int GetAccidentCount()
        {
            int AccidentCnt;
            try
            {

                cmd = new SqlCommand("usp_GetAccidentCount", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                conn.Open();
                AccidentCnt = cmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception ex)
            {
               throw ex;
            }

            return AccidentCnt;
        }
    }


}


