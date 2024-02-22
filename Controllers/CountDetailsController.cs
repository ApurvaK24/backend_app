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
    public class CountDetailsController : ApiController
    {
        [HttpGet]
        [Route("api/CountDetails")]
        public IHttpActionResult GetCountDetails()
        {
            List<CountDetail> countDetails = new List<CountDetail>();

            try
            {
                 // Replace with your SQL Server connection string
                using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand("sp_getCountDetails", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CountDetail detail = new CountDetail();
                                detail.MonthName = reader["MonthName"].ToString();
                                detail.IncidentCount = DBNull.Value.Equals(reader["IncidentCount"]) ? 0 : Convert.ToInt32(reader["IncidentCount"]);
                                detail.AccidentCount = DBNull.Value.Equals(reader["AccidentCount"]) ? 0 : Convert.ToInt32(reader["AccidentCount"]);
                                countDetails.Add(detail);
                            }
                        }
                    }
                }

                return Ok(countDetails);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
