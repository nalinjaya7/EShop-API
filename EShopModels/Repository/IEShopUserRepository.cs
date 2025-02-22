namespace EShopModels.Repository
{
    public interface IEShopUserRepository : IRepository<EShopUser>
    {
        Task<EShopModels.EShopUser> ValidateUserAsync(EShopModels.EShopUser EShopUser);
        EShopModels.EShopUser ValidateUser(EShopModels.EShopUser EShopUser);
        Task<EShopModels.EShopUser> GetUserAsync(string UserName);
        Task<EShopModels.EShopUser> GetUserNameByEmailAsync(string Email);
        Task<object> InsertLoginDetailsAsync(object loginDetail);
        Task UpdateLoginDetailsAsync(object loginDetail);
        Task<object> SearchAsync(object obj);       
        Task<object> InsertAdminUser(object obj);
    }
}
