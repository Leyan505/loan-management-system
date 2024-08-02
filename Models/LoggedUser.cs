namespace PrestamosCreciendo.Models
{
    public class LoggedUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Level { get; set; }
        public int Id{ get; set; }
        public int TimeOffset { get; set; }

        public LoggedUser(HttpContext userContext) 
        {
            Email = userContext.User.Claims.ElementAt(0).Value;
            Name = userContext.User.Claims.ElementAt(1).Value;
            Level = userContext.User.Claims.ElementAt(2).Value;
            Id = Int32.Parse(userContext.User.Claims.ElementAt(3).Value);
            TimeOffset = Int32.Parse(userContext.User.Claims.ElementAt(4).Value);
        }
    }
}
