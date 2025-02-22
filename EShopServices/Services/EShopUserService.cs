using EShopModels;
using EShopModels.Repository;
using EShopModels.Services;

namespace EShopServices.Services
{
    public class EShopUserService : BaseService<EShopUser>,IEShopUserService
    {
        private readonly IEShopUserRepository _EShopUserRepository;
        public EShopUserService(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _EShopUserRepository = unitOfWork.EShopUserRepository;
        }
 
        public async Task<object> SearchAsync(object obj)
        {
            return await _EShopUserRepository.SearchAsync(obj);
        }
 
        public async Task<EShopUser> ValidateUserAsync(EShopUser EShopUser)
        {
            return await _EShopUserRepository.ValidateUserAsync(EShopUser);
        }

        public EShopUser ValidateUser(EShopUser EShopUser)
        {
            return _EShopUserRepository.ValidateUser(EShopUser);
        }

        public async Task<EShopUser> GetUserAsync(string UserName)
        {
            return await _EShopUserRepository.GetUserAsync(UserName);
        }

        public async Task<EShopUser> GetUserNameByEmailAsync(string Email)
        {
            return await _EShopUserRepository.GetUserNameByEmailAsync(Email);
        }

        public async Task<object> InsertLoginDetailsAsync(object loginDetail)
        {
            return await _EShopUserRepository.InsertLoginDetailsAsync(loginDetail);
        }

        public async Task UpdateLoginDetailsAsync(object loginDetail)
        {
             await _EShopUserRepository.UpdateLoginDetailsAsync(loginDetail);
        }

       
    } 
}
