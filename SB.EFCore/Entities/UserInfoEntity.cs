namespace SB.EFCore.Entities
{
    public class UserInfoEntity
    {
        public int Id { get; set; }
        
        public string Email { get; set; }
        
        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int PhoneNumber { get; set; }
    }
}