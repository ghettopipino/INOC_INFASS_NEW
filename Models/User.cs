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
            return @"
                INSERT INTO Users
                (FullName, Email, Username, Password)
                VALUES
                (@FullName, @Email, @Username, @Password)";
        }


        public string UpdateQuery()
        {
            return @"
                UPDATE Users
                SET FullName = @FullName,
                    Email = @Email,
                    Username = @Username,
                    Password = @Password
                WHERE Id = @Id";
        }


        public string DeleteQuery()
        {
            return @"
                DELETE FROM Users
                WHERE Id = @Id";
        }
    }
}