namespace ProiectPSSC2025.DTOs
{
    public class UserDTO
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }

        public void SetEmail(string email)
        {
            if (!email.Contains("@"))
                throw new ArgumentException("Invalid email format");
            Email = email;
        }
    }
}
