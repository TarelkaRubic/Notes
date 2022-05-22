using PS_223020_Server.BusinessLogic.Core.Models;
using System.Threading.Tasks;

namespace PS_223020_Server.BusinessLogic.Core.Interfaces
{
    public interface IUserService
    {
        Task<UserInformationBlo> Registration(int numberPrefix, int number, string password);
        Task<UserInformationBlo> Auth(int numberPrefix, int number, string password);
        Task<UserInformationBlo> Get(int userId);
        Task<UserInformationBlo> Update(int numberPrefix, int number, string password, UserUpdateBlo userUpdateBlo);
        Task<bool> DoesExist(int numberPrefix, int number);
    }
}