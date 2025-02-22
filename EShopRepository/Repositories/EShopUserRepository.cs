using EShopModels;
using EShopModels.Repository;
using Microsoft.EntityFrameworkCore;

namespace EShopRepository.Repositories
{
    public class EShopUserRepository : BaseRepository<EShopUser>, IEShopUserRepository
    {
        public EShopUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

        public async Task<object> DeleteAsync(int ID)
        {
            EShopUser EShopUser = await Set.Where(g => g.ID == ID).FirstOrDefaultAsync();
            Set.Remove(EShopUser);
            await _context.SaveChangesAsync();
            return await Task.FromResult<object>(EShopUser);
        }

        public async Task<object> GetByIDAsync(int ID)
        {
            EShopUser EShopUser = await Set.Where(i => i.ID == ID).SingleAsync();
            return await Task.FromResult<object>(EShopUser);
        }

        public async Task<object> InsertAsync(object obj)
        {
            if (obj is EShopUser user1 && user1.UserName != null)
            {
                EShopUser user = user1;               
                Set.Add(user);
                await _context.SaveChangesAsync();                 
                return await Task.FromResult<object>(user);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public async Task<object> SearchAsync(object obj)
        {
            List<EShopUser> EShopUsers = new List<EShopUser>();
            EShopUsers = (List<EShopUser>)await Set.OrderBy(j => j.UserName).ToListAsync();
 
            return await Task.FromResult<object>(EShopUsers);
        }

        public async Task<int> UpdateAsync(int ID, object obj)
        {
           _context.Entry((EShopUser)obj).State = EntityState.Modified;
            return await Task.FromResult<int>(await _context.SaveChangesAsync());
        }

        public async Task<EShopUser> GetUserAsync(string UserName)
        {
            EShopModels.EShopUser pUser = await Set.Where(e => e.UserName == UserName).SingleOrDefaultAsync();
            return await Task.FromResult<EShopUser>(pUser);
        }

        public async Task<EShopUser> GetUserNameByEmailAsync(string Email)
        {
            return await Task.FromResult<EShopUser>(await Set.Where(e => e.Email == Email).SingleOrDefaultAsync());
        }

        public async Task<object> InsertLoginDetailsAsync(object loginDetail)
        {
            if (loginDetail != null)
            {
                LoginDetail detail = (LoginDetail)loginDetail;
                _context.LoginDetails.Add(detail);
                await _context.SaveChangesAsync();
                return await Task.FromResult<LoginDetail>(detail);
            }
            else
            {
                return await Task.FromResult<object>(loginDetail);
            }
        }

        public async Task UpdateLoginDetailsAsync(object loginDetail)
        {
            if (loginDetail != null)
            {
                LoginDetail detail = (LoginDetail)loginDetail;
                LoginDetail logind = _context.LoginDetails.Find(detail.ID);
                logind.LogOutTime = DateTime.Now;
                _context.MarkAsModified(logind);
                await _context.SaveChangesAsync();
            }
            return; 
        }

        public async Task<EShopUser> ValidateUserAsync(EShopUser EShopUser)
        { 
             return await Task.FromResult<EShopUser>(await Set.Where(e => e.IsActive == true && e.Email == EShopUser.Email && e.Password == EShopUser.Password).SingleOrDefaultAsync());
        }

        EShopUser IEShopUserRepository.ValidateUser(EShopUser EShopUser)
        {
            throw new NotImplementedException();
        }

        public async Task<object> InsertAdminUser(object obj)
        {
            if (obj != null)
            {
                EShopUser EShopUser = (EShopUser)obj;
                var userExists = _context.Users.SingleOrDefaultAsync(u => u.UserName == EShopUser.UserName);
                if (userExists != null)
                {
                    _context.Users.Add(EShopUser);
                    await _context.SaveChangesAsync();
                }
                return await Task.FromResult<EShopUser>(EShopUser);
            }
            else
            {
                return Task.FromResult<object>(obj);
            }
        }
    }
}
