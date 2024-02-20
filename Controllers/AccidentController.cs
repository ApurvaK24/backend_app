﻿using System;
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

        /* [HttpGet]
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
         }*/

        [HttpGet]
        [Route("GetAccidentCount")]
        public int GetAccidentCount()
        {
            int AccidentCnt = 0; // Initialize incidentCnt to 0

            try
            {

                using (SqlCommand cmd = new SqlCommand("usp_GetAccidentCount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    // Use ExecuteScalar to retrieve a single value
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        // If the result is not null or DBNull, convert it to int
                        AccidentCnt = Convert.ToInt32(result);
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exception appropriately (logging, error response, etc.)
                Console.WriteLine("Error fetching incident count: " + ex.Message);
                throw; // Re-throw the exception
            }

            return AccidentCnt;
        }




        [HttpGet]
        [Route("GetAccidentCountsByMonth")]
        public IHttpActionResult GetAccidentCountsByMonth()
        {
            List<AccidentByMonth> accidentCounts = new List<AccidentByMonth>();

            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    conn.Open();

                    string query = @"
                    SELECT
                        MONTH(DateOfAccident) AS Month,
                        DATENAME(MONTH, DateOfAccident) AS MonthName,
                        SUM(CASE WHEN AccidentCauses = 'I' THEN 1 ELSE 0 END) AS I,
                        SUM(CASE WHEN AccidentCauses = 'A' THEN 1 ELSE 0 END) AS A
                    FROM
                        AccidentReports
                    GROUP BY
                        MONTH(DateOfAccident),
                        DATENAME(MONTH, DateOfAccident)
                    ORDER BY
                        MONTH(DateOfAccident);";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AccidentByMonth accident = new AccidentByMonth
                                {
                                    Month = reader.GetInt32(0),
                                    MonthName = reader.GetString(1),
                                    I = reader.GetInt32(2),
                                    A = reader.GetInt32(3)
                                };
                                accidentCounts.Add(accident);
                            }
                        }
                    }
                }

                return Ok(accidentCounts);
            }
            catch (Exception ex)
            {
                // Handle exception appropriately (logging, error response, etc.)
                Console.WriteLine("Error fetching accident counts by month: " + ex.Message);
                return InternalServerError();
            }
        }
    }


}


