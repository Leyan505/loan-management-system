namespace PrestamosCreciendo.Models
{
    public class Users
    {
        public int Id {  get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; } 
        public required string Level { get; set; }
        public string? Address { get; set; }
        public int? City { get; set; }
        public int? Phone { get; set; }

    }
}
