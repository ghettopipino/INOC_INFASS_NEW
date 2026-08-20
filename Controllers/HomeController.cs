using Inoc_laboratory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Diagnostics;

namespace Inoc_laboratory.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(
    User user,
    string confirmPassword)
        {
            try
            {
                if (user.Password != confirmPassword)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Passwords do not match."
                    });
                }

                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection")!;

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string insertQuery = user.InsertQuery();

                    using (SqlCommand command =
                        new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@FullName", user.FullName);

                        command.Parameters.AddWithValue(
                            "@Email", user.Email);

                        command.Parameters.AddWithValue(
                            "@Username", user.Username);

                        command.Parameters.AddWithValue(
                            "@Password", user.Password);

                        int rowsAffected =
                            await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Json(new
                            {
                                success = true,
                                fullName = user.FullName,
                                email = user.Email,
                                username = user.Username
                            });
                        }
                    }
                }

                return Json(new
                {
                    success = false,
                    message = "Insert failed."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(User user)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString("DefaultConnection")!;

                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    string query = @"
                SELECT COUNT(*)
                FROM Users
                WHERE Username = @Username
                AND Password = @Password";

                    using (SqlCommand command =
                        new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Username", user.Username);

                        command.Parameters.AddWithValue(
                            "@Password", user.Password);

                        int count = Convert.ToInt32(
                            await command.ExecuteScalarAsync());

                        if (count > 0)
                        {
                            return Json(new
                            {
                                success = true,
                                message = "Login successful!"
                            });
                        }
                    }

                    return Json(new
                    {
                        success = false,
                        message = "Invalid username or password."
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}