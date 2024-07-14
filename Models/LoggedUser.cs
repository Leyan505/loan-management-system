namespace PrestamosCreciendo.Models
{
    public class LoggedUser
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Level { get; set; }

        public LoggedUser(HttpContext userContext) 
        {
            Email = userContext.User.Claims.ElementAt(0).Value;
            Name = userContext.User.Claims.ElementAt(1).Value;
            Level = userContext.User.Claims.ElementAt(2).Value;
        }
    }
}
