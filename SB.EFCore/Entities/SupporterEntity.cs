namespace SB.EFCore.Entities
{
    public class SupporterEntity
    {
        public int Id { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public UserInfoEntity UserInfo { get; set; }
        
        public int UserInfoId { get; set; }
    }
}