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
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
        SqlCommand cmd = null;
        SqlDataAdapter da = null;

        [HttpPost]
        [Route("Registration")]
        public string Registration(Employee employee)
        {
            string msg = string.Empty;
            try
            {
                cmd = new SqlCommand("usp_Registration", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@Email", employee.Email); // Updated from PhoneNo
                cmd.Parameters.AddWithValue("@Password", employee.Password); // Updated from Address
                cmd.Parameters.AddWithValue("@ReenteredPassword", employee.ReenteredPassword);
                cmd.Parameters.AddWithValue("@SelectedOption", employee.SelectedOption);
                /*cmd = new SqlCommand("usp_Registration", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId); // Updated from PhoneNo
                cmd.Parameters.AddWithValue("@Password", employee.Password); // Updated from Address
                cmd.Parameters.AddWithValue("@ReenteredPassword", employee.ReenteredPassword);
                cmd.Parameters.AddWithValue("@SelectedOption", employee.SelectedOption);*/
                // cmd.Parameters.AddWithValue("@IsActive", employee.IsActive);

                conn.Open();
                int i = cmd.ExecuteNonQuery();
                conn.Close();

                /*if (i > 0)
                {
                    // Send email after successful registration
                    SendEmail(employee.Email, employee);
                   
                    msg = "Data Inserted.";
                }*/
                if (i > 0)
                {
                    // Send email to the registered user
                    SendEmail(employee.Email, employee);

                    // Send email to ishakandhare2003@gmail.com
                    SendEmail("ishakandhare2003@gmail.com", employee);

                    msg = "Data Inserted.";
                }
                else
                {
                    msg = "Error inserting data.";
                }
            }
            catch (Exception ex)
            {
                msg = $"Error: {ex.Message}";
            }

            return msg;
        }

        private void SendEmail(string recipientEmail, Employee employee)
        {
            string subject = "New Registration";
            string body = $"Name: {employee.Name}\nEmail: {employee.Email}\nPassword: {employee.Password}\nSelected Option: {employee.SelectedOption}";

            try
            {
                using (var mailMessage = new MailMessage("ishakandhare2003@gmail.com", recipientEmail)) // Send email to both ishakandhare2003@gmail.com and the registered user's email
                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new System.Net.NetworkCredential("ishakandhare2003@gmail.com", "nfne uroi vonq qczw");
                    smtpClient.EnableSsl = true;

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                // Log the exception or handle it appropriately
            }
        }


        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(Employee employee)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("usp_Login", conn))
                {
                    /*cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);
                    cmd.Parameters.AddWithValue("@Password", employee.Password);
                    cmd.Parameters.AddWithValue("@SelectedOption", employee.SelectedOption);*/
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@Email", employee.Email); // Updated from EmployeeId
                    cmd.Parameters.AddWithValue("@Password", employee.Password);
                    cmd.Parameters.AddWithValue("@SelectedOption", employee.SelectedOption);

                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            // User is valid, return user data
                            reader.Read();
                            /* Employee loggedInUser = new Employee
                             {
                                 Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                 Name = reader.GetString(reader.GetOrdinal("NAME")),
                                 EmployeeId = reader.GetString(reader.GetOrdinal("EmployeeId")),
                                 // Include other fields as needed
                             };*/
                            Employee loggedInUser = new Employee
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                Name = reader.GetString(reader.GetOrdinal("NAME")),
                                Email = reader.GetString(reader.GetOrdinal("Email")), // Updated from EmployeeId
                                // Include other fields as needed
                            };

                            return Ok(loggedInUser);
                        }
                        else
                        {
                            // User is invalid
                            return NotFound();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(new Exception($"Error: {ex.Message}"));
            }
            finally
            {
                conn.Close();
            }
        }






        [HttpGet]
        [Route("GetHODCount")]
        public int GetHODCount()
        {
            int hodCnt = 0; // Initialize incidentCnt to 0

            try
            {

                using (SqlCommand cmd = new SqlCommand("usp_HodCount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    // Use ExecuteScalar to retrieve a single value
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        // If the result is not null or DBNull, convert it to int
                        hodCnt = Convert.ToInt32(result);
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exception appropriately (logging, error response, etc.)
                Console.WriteLine("Error fetching incident count: " + ex.Message);
                throw; // Re-throw the exception
            }

            return hodCnt;
        }

        



        [HttpGet]
        [Route("GetEmpCount")]
        public int GetEmpCount()
        {
            int empCnt = 0; // Initialize incidentCnt to 0

            try
            {

                using (SqlCommand cmd = new SqlCommand("usp_EmpCount", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    conn.Open();
                    // Use ExecuteScalar to retrieve a single value
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        // If the result is not null or DBNull, convert it to int
                        empCnt = Convert.ToInt32(result);
                    }
                }

            }
            catch (Exception ex)
            {
                // Handle exception appropriately (logging, error response, etc.)
                Console.WriteLine("Error fetching incident count: " + ex.Message);
                throw; // Re-throw the exception
            }

            return empCnt;
        }


    }




}
/*namespace backend_app.Controllers
{
    [RoutePrefix("api/Test")]
    public class TestController : ApiController
    {
        SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
        SqlCommand cmd = null;
        SqlDataAdapter da = null;

        [HttpPost]
        [Route("Registration")]
        public string Registration(Employee employee)
        {
            string msg = string.Empty;
            try
            {
                cmd = new SqlCommand("usp_Registration", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Name", employee.Name);
                cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId); // Updated from PhoneNo
                cmd.Parameters.AddWithValue("@Password", employee.Password); // Updated from Address
                cmd.Parameters.AddWithValue("@ReenteredPassword", employee.ReenteredPassword);
                cmd.Parameters.AddWithValue("@SelectedOption", employee.SelectedOption);
                // cmd.Parameters.AddWithValue("@IsActive", employee.IsActive);

                conn.Open();
                int i = cmd.ExecuteNonQuery();
                conn.Close();

                if (i > 0)
                {
                    // Send email after successful registration
                    SendEmail(employee);

                    msg = "Data Inserted.";
                }
                else
                {
                    msg = "Error inserting data.";
                }
            }
            catch (Exception ex)
            {
                msg = $"Error: {ex.Message}";
            }

            return msg;
        }

        private void SendEmail(Employee employee)
        {
            string toEmail = "ishakandhare2003@gmail.com";
            string subject = "New Registration";
            string body = $"Name: {employee.Name}\nEmployee Id: {employee.EmployeeId}\nPassword: {employee.Password}\nRe-entered Password: {employee.ReenteredPassword}\nSelected Option: {employee.SelectedOption}";

            try
            {
                using (var mailMessage = new MailMessage("ishakandhare2003@gmail.com", toEmail))
                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new System.Net.NetworkCredential("ishakandhare2003@gmail.com", "acpz sxau yhwa gnae");
                    smtpClient.EnableSsl = true;

                    mailMessage.Subject = subject;
                    mailMessage.Body = body;

                    smtpClient.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                // Log the exception or handle it appropriately
            }
        }

        /*[HttpPost]
        [Route("Login")]
        public string Login(Employee employee)
        {
            string msg = string.Empty;
            try
            {
                da = new SqlDataAdapter("usp_Login", conn);
                da.SelectCommand.CommandType = CommandType.StoredProcedure;
                da.SelectCommand.Parameters.AddWithValue("@Name", employee.Name);
                da.SelectCommand.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId); // Updated from PhoneNo
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    msg = "User is Valid";
                }
                else
                {
                    msg = "User is Invalid";
                }
            }
            catch (Exception ex)
            {
                msg = $"Error: {ex.Message}";
            }

            return msg;
        }*/
/*        [HttpPost]
        [Route("Login")]
        public string Login(Employee employee)
        {
            string msg = string.Empty;
            //string role = string.Empty; // New variable to hold the role
            try
            {
                cmd = new SqlCommand("usp_Log", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId); // Assuming 'EmployeeId' is the appropriate field for username
                cmd.Parameters.AddWithValue("@Password", employee.Password); // Updated field

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    // Retrieve the role from the result set
                    //role = reader["Role"].ToString(); // Assuming 'Role' is the column name for role in the database

                    msg = "User is Valid";
                }
                else
                {
                    msg = "User is Invalid";
                }

                conn.Close();
            }
            catch (Exception ex)
            {
                msg = $"Error: {ex.Message}";
            }

            return msg;
            // Create a JSON object with message and role
            //var responseObject = new { Message = msg, Role = role };

            // Serialize the JSON object to a string
            //var responseJson = JsonConvert.SerializeObject(responseObject);

            // Create the response with status code OK and the JSON content
            //var response = new HttpResponseMessage(HttpStatusCode.OK);
            //response.Content = new StringContent(responseJson, System.Text.Encoding.UTF8, "application/json");

           // return response;
        }


    }
}
*/