using Inoc_laboratory.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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


        // =========================================
        // INDEX PAGE
        // =========================================
        public IActionResult Index()
        {
            return View();
        }


        // =========================================
        // LOGIN PAGE
        // =========================================
        [HttpGet]
        public IActionResult login()
        {
            return View();
        }


        // =========================================
        // REGISTER PAGE
        // =========================================
        [HttpGet]
        public IActionResult register()
        {
            return View();
        }


        // =========================================
        // GET ALL USERS
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            List<User> users = new List<User>();

            try
            {
                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    string query = @"
                        SELECT
                            Id,
                            FullName,
                            Email,
                            Username
                        FROM Users
                        ORDER BY Id DESC";


                    using (SqlCommand command =
                        new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader =
                            await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                users.Add(
                                    new User
                                    {
                                        Id =
                                            Convert.ToInt32(
                                                reader["Id"]),

                                        FullName =
                                            reader["FullName"]
                                            .ToString() ?? "",

                                        Email =
                                            reader["Email"]
                                            .ToString() ?? "",

                                        Username =
                                            reader["Username"]
                                            .ToString() ?? ""
                                    }
                                );
                            }
                        }
                    }
                }


                return Json(new
                {
                    success = true,
                    data = users
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


        // =========================================
        // GET SINGLE USER
        // =========================================
        [HttpGet]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    string query = @"
                        SELECT
                            Id,
                            FullName,
                            Email,
                            Username,
                            Password
                        FROM Users
                        WHERE Id = @Id";


                    using (SqlCommand command =
                        new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Id",
                            id);


                        using (SqlDataReader reader =
                            await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                User user = new User
                                {
                                    Id =
                                        Convert.ToInt32(
                                            reader["Id"]),

                                    FullName =
                                        reader["FullName"]
                                        .ToString() ?? "",

                                    Email =
                                        reader["Email"]
                                        .ToString() ?? "",

                                    Username =
                                        reader["Username"]
                                        .ToString() ?? "",

                                    Password =
                                        reader["Password"]
                                        .ToString() ?? ""
                                };


                                return Json(new
                                {
                                    success = true,
                                    data = user
                                });
                            }
                        }
                    }
                }


                return Json(new
                {
                    success = false,
                    message = "User not found."
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


        // =========================================
        // DYNAMIC INSERT DATA
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Insert(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.FullName) ||
                    string.IsNullOrWhiteSpace(user.Email) ||
                    string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.Password))
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Please complete all fields."
                    });
                }


                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    // CHECK DUPLICATE USERNAME / EMAIL
                    string checkQuery = @"
                        SELECT COUNT(*)
                        FROM Users
                        WHERE Username = @Username
                        OR Email = @Email";


                    using (SqlCommand checkCommand =
                        new SqlCommand(
                            checkQuery,
                            connection))
                    {
                        checkCommand.Parameters.AddWithValue(
                            "@Username",
                            user.Username);

                        checkCommand.Parameters.AddWithValue(
                            "@Email",
                            user.Email);


                        int count = Convert.ToInt32(
                            await checkCommand
                            .ExecuteScalarAsync());


                        if (count > 0)
                        {
                            return Json(new
                            {
                                success = false,
                                message =
                                    "Username or email already exists."
                            });
                        }
                    }


                    string insertQuery =
                        user.InsertQuery();


                    using (SqlCommand command =
                        new SqlCommand(
                            insertQuery,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@FullName",
                            user.FullName);

                        command.Parameters.AddWithValue(
                            "@Email",
                            user.Email);

                        command.Parameters.AddWithValue(
                            "@Username",
                            user.Username);

                        command.Parameters.AddWithValue(
                            "@Password",
                            user.Password);


                        int rowsAffected =
                            await command
                            .ExecuteNonQueryAsync();


                        if (rowsAffected > 0)
                        {
                            return Json(new
                            {
                                success = true,
                                message =
                                    "User inserted successfully!"
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


        // =========================================
        // DYNAMIC UPDATE DATA
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Update(User user)
        {
            try
            {
                if (user.Id <= 0 ||
                    string.IsNullOrWhiteSpace(user.FullName) ||
                    string.IsNullOrWhiteSpace(user.Email) ||
                    string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.Password))
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Please complete all fields."
                    });
                }


                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    string checkQuery = @"
                        SELECT COUNT(*)
                        FROM Users
                        WHERE
                        (Username = @Username
                        OR Email = @Email)
                        AND Id != @Id";


                    using (SqlCommand checkCommand =
                        new SqlCommand(
                            checkQuery,
                            connection))
                    {
                        checkCommand.Parameters.AddWithValue(
                            "@Username",
                            user.Username);

                        checkCommand.Parameters.AddWithValue(
                            "@Email",
                            user.Email);

                        checkCommand.Parameters.AddWithValue(
                            "@Id",
                            user.Id);


                        int count = Convert.ToInt32(
                            await checkCommand
                            .ExecuteScalarAsync());


                        if (count > 0)
                        {
                            return Json(new
                            {
                                success = false,
                                message =
                                    "Username or email already exists."
                            });
                        }
                    }


                    string updateQuery =
                        user.UpdateQuery();


                    using (SqlCommand command =
                        new SqlCommand(
                            updateQuery,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Id",
                            user.Id);

                        command.Parameters.AddWithValue(
                            "@FullName",
                            user.FullName);

                        command.Parameters.AddWithValue(
                            "@Email",
                            user.Email);

                        command.Parameters.AddWithValue(
                            "@Username",
                            user.Username);

                        command.Parameters.AddWithValue(
                            "@Password",
                            user.Password);


                        int rowsAffected =
                            await command
                            .ExecuteNonQueryAsync();


                        if (rowsAffected > 0)
                        {
                            return Json(new
                            {
                                success = true,
                                message =
                                    "User updated successfully!"
                            });
                        }
                    }
                }


                return Json(new
                {
                    success = false,
                    message = "Update failed."
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


        // =========================================
        // DYNAMIC DELETE DATA
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Invalid user ID."
                    });
                }


                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    string deleteQuery = @"
                        DELETE FROM Users
                        WHERE Id = @Id";


                    using (SqlCommand command =
                        new SqlCommand(
                            deleteQuery,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Id",
                            id);


                        int rowsAffected =
                            await command
                            .ExecuteNonQueryAsync();


                        if (rowsAffected > 0)
                        {
                            return Json(new
                            {
                                success = true,
                                message =
                                    "User deleted successfully!"
                            });
                        }
                    }
                }


                return Json(new
                {
                    success = false,
                    message = "User not found."
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


        // =========================================
        // KEEP YOUR EXISTING REGISTER PAGE WORKING
        // =========================================
        [HttpPost]
        public async Task<IActionResult> register(
            User user,
            string confirmPassword)
        {
            if (user.Password != confirmPassword)
            {
                return Json(new
                {
                    success = false,
                    message = "Passwords do not match."
                });
            }


            return await Insert(user);
        }


        // =========================================
        // LOGIN USING LOCALDB
        // =========================================
        [HttpPost]
        public async Task<IActionResult> login(User user)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.Password))
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Please enter username and password."
                    });
                }


                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    string query = @"
                        SELECT Id,
                               FullName,
                               Username
                        FROM Users
                        WHERE Username = @Username
                        AND Password = @Password";


                    using (SqlCommand command =
                        new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Username",
                            user.Username);

                        command.Parameters.AddWithValue(
                            "@Password",
                            user.Password);


                        using (SqlDataReader reader =
                            await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return Json(new
                                {
                                    success = true,
                                    message =
                                        "Login successful!"
                                });
                            }
                        }
                    }
                }


                return Json(new
                {
                    success = false,
                    message =
                        "Invalid username or password."
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
    }
}