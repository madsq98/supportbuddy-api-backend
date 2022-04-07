namespace Core.Models
{
    public class Supporter
    {
        public int Id { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public UserInfo UserInfo { get; set; }
    }
}