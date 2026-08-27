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
        // READ - SHOW ALL USERS
        // =========================================
        public async Task<IActionResult> Index()
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
                        SELECT Id,
                               FullName,
                               Email,
                               Username,
                               Password
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


                                users.Add(user);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }


            return View(users);
        }


        // =========================================
        // LOGIN PAGE
        // Views/Home/login.cshtml
        // =========================================
        [HttpGet]
        public IActionResult login()
        {
            return View();
        }


        // =========================================
        // REGISTER PAGE
        // Views/Home/register.cshtml
        // =========================================
        [HttpGet]
        public IActionResult register()
        {
            return View();
        }


        // =========================================
        // CREATE - REGISTER
        // =========================================
        [HttpPost]
        public async Task<IActionResult> register(
            User user,
            string confirmPassword)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(user.FullName) ||
                    string.IsNullOrWhiteSpace(user.Email) ||
                    string.IsNullOrWhiteSpace(user.Username) ||
                    string.IsNullOrWhiteSpace(user.Password) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    return Json(new
                    {
                        success = false,
                        message = "Please complete all fields."
                    });
                }


                if (user.Password != confirmPassword)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Passwords do not match."
                    });
                }


                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    // CHECK DUPLICATE
                    string checkQuery = @"
                        SELECT COUNT(*)
                        FROM Users
                        WHERE Username = @Username
                        OR Email = @Email";


                    using (SqlCommand checkCommand =
                        new SqlCommand(checkQuery, connection))
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


                    // INSERT
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
                                    "Registration successful!",

                                fullName =
                                    user.FullName,

                                email =
                                    user.Email,

                                username =
                                    user.Username
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
        // LOGIN - CHECK LOCALDB
        // =========================================
        [HttpPost]
        public async Task<IActionResult> login(
            User user)
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
                               Email,
                               Username,
                               Password
                        FROM Users
                        WHERE Username = @Username
                        AND Password = @Password";


                    using (SqlCommand command =
                        new SqlCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Username",
                            user.Username);

                        command.Parameters.AddWithValue(
                            "@Password",
                            user.Password);


                        using (SqlDataReader reader =
                            await command
                            .ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return Json(new
                                {
                                    success = true,

                                    message =
                                        "Login successful!",

                                    id =
                                        Convert.ToInt32(
                                            reader["Id"]),

                                    fullName =
                                        reader["FullName"]
                                        .ToString(),

                                    username =
                                        reader["Username"]
                                        .ToString()
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


        // =========================================
        // EDIT - GET USER BY ID
        // =========================================
        [HttpGet]
        public async Task<IActionResult> Edit(
            int id)
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
                        SELECT Id,
                               FullName,
                               Email,
                               Username,
                               Password
                        FROM Users
                        WHERE Id = @Id";


                    using (SqlCommand command =
                        new SqlCommand(
                            query,
                            connection))
                    {
                        command.Parameters.AddWithValue(
                            "@Id",
                            id);


                        using (SqlDataReader reader =
                            await command
                            .ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                User user =
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
                                            .ToString() ?? "",

                                        Password =
                                            reader["Password"]
                                            .ToString() ?? ""
                                    };


                                return View(user);
                            }
                        }
                    }
                }


                return NotFound();
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }


        // =========================================
        // UPDATE USER
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Update(
            User user)
        {
            try
            {
                if (user.Id <= 0 ||
                    string.IsNullOrWhiteSpace(
                        user.FullName) ||
                    string.IsNullOrWhiteSpace(
                        user.Email) ||
                    string.IsNullOrWhiteSpace(
                        user.Username) ||
                    string.IsNullOrWhiteSpace(
                        user.Password))
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


                    // CHECK DUPLICATE
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


                    // UPDATE
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
        // DELETE USER
        // =========================================
        [HttpPost]
        public async Task<IActionResult> Delete(
            int id)
        {
            try
            {
                if (id <= 0)
                {
                    return Json(new
                    {
                        success = false,
                        message =
                            "Invalid user ID."
                    });
                }


                string connectionString =
                    _configuration.GetConnectionString(
                        "DefaultConnection")!;


                using (SqlConnection connection =
                    new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();


                    User user =
                        new User();


                    string deleteQuery =
                        user.DeleteQuery();


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
    }
}