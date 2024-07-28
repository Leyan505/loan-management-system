namespace PrestamosCreciendo.Models
{
    public class UsersDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Level { get; set; } = "user";
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Province { get; set; }
        public string? Country { get; set; }
        public int? Phone { get; set; }
        public string? Nit { get; set; }
        public string? lng { get; set; }
        public string? lat { get; set; }
        public string Status { get; set; } = "good";
        public ErrorViewModel? Error { get; set; }
    }
}
