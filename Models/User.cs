namespace Inoc_laboratory.Models
{
    public class User
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public string InsertQuery()
        {
            return @"INSERT INTO Users
                     (FullName, Email, Username, Password)
                     VALUES
                     (@FullName, @Email, @Username, @Password)";
        }
    }
}