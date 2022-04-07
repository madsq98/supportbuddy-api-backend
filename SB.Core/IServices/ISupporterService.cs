using Core.Models;

namespace Core.IServices
{
    public interface ISupporterService : I_RW_Service<Supporter>
    {
        public int CheckLogin(string username, string password);
    }
}