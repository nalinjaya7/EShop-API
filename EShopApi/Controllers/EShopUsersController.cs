using EShopApi.Filters;
using EShopModels;
using EShopModels.Common;
using EShopModels.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace EShopApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]  
    public class EShopUsersController : ControllerBase
    { 
        private readonly IEShopUserService _EShopUserService; 
        private readonly ILogger<EShopUsersController> _logger;
        public EShopUsersController(IEShopUserService EShopUserService, IHttpContextAccessor httpContextAccessor, ILogger<EShopUsersController> logger)
        {
            _EShopUserService = EShopUserService;
            _logger = logger;
        }

        //[Produces(typeof(EShopUser))]
        //[HttpPost]
        //[Route("ValidateUser")]
        //public async Task<IActionResult> ValidateUser(EShopUser EShopUser)
        //{
        //    try
        //    {
        //        EncryptDecrypt encrypt = new();
        //        EShopUser.ConfirmPassword = encrypt.Encryptor(EShopUser.Password);
        //        EShopUser.Password = encrypt.Encryptor(EShopUser.Password);
        //        EShopUser pUser = (EShopUser)await _EShopUserService.ValidateUserAsync(EShopUser);
        //        if (pUser == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(pUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        [Produces(typeof(string))]
        [HttpPost]
        [Route("ValidateToken")]
        public async Task<IActionResult> ValidateToken(LoginView login)
        {
            try
            {
                string Token;
                EShopUser EShopUser = new(0, login.Email, "", "", "", login.Email, true, Guid.NewGuid(), login.Password, Roles.User); ;
                EShopUser.ConfirmPassword = login.Password;
                EncryptDecrypt encrypt = new();
                EShopUser.ConfirmPassword = encrypt.Encryptor(EShopUser.Password);
                EShopUser.Password = encrypt.Encryptor(EShopUser.Password);
                EShopUser pUser = (EShopUser)await _EShopUserService.ValidateUserAsync(EShopUser);

                if (pUser == null)
                {
                    return NotFound();
                }
                else
                {
                    var key = Encoding.ASCII.GetBytes("Jagath98989765123");
                    var JWToken = new JwtSecurityToken(
                        issuer: "http://localhost:5167/",
                        audience: "http://localhost:5167/",
                        claims: GetUserClaims(pUser),
                        notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                        expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
                        signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    );
                    Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
                    return Ok(Token);
                }
            }
            catch (Exception ex)
            {
                await ErrorMessages(ex);
                return StatusCode(500,ex);
            }
        }

        private static IEnumerable<Claim> GetUserClaims(EShopUser user)
        {
            IEnumerable<Claim> claims = new Claim[]
            {
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim("USERNAME", user.UserName),
                    new Claim("EMAILID", user.Email),
                    new Claim("EShopUserID",user.ID.ToString()),
                    new Claim("RoleName", user.RoleName.ToString()), 
                    new Claim(ClaimTypes.Role,string.Join(",",user.RoleName))
            };
            return claims;
        }

        //[Produces(typeof(EShopUser))]
        //[HttpPost]
        //[Route("InsertLoginDetails")]
        //public async Task<IActionResult> InsertLoginDetails(LoginDetail loginDetail)
        //{
        //    try { 
        //    LoginDetail login = (EShopModels.LoginDetail)await _EShopUserService.InsertLoginDetailsAsync(loginDetail);            
        //    return Ok(login);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[Produces(typeof(EShopUser))]
        //[HttpPost]
        //[Route("UpdateLoginDetails")]
        //public async Task<IActionResult> UpdateLoginDetails(LoginDetail loginDetail)
        //{
        //    try { 
        //    await _EShopUserService.UpdateLoginDetailsAsync(loginDetail);
        //    return Ok(loginDetail);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpGet]
        //[Route("GetUser/{UserName}")]
        //[Produces(typeof(EShopUser))]
        //public async Task<IActionResult> GetUser(string UserName)
        //{
        //    try
        //    {
        //        EShopUser pUser = (EShopUser)await _EShopUserService.GetUserAsync(UserName);
        //        if (pUser == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(pUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpGet]
        //[Route("GetUserByEmail")]
        //[Produces(typeof(EShopUser))]
        //public async Task<IActionResult> GetUserByEmail(string Email)
        //{
        //    try
        //    {
        //        EShopUser pUser = (EShopUser)await _EShopUserService.GetUserNameByEmailAsync(Email);
        //        if (pUser == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(pUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpGet]
        //[Route("UserNameByEmail")]
        //[Produces(typeof(EShopUser))]
        //public async Task<IActionResult> UserNameByEmail(string Email)
        //{
        //    try
        //    {
        //        EShopUser pUser = (EShopUser)await _EShopUserService.GetUserNameByEmailAsync(Email);
        //        if (pUser == null)
        //        {
        //            return NotFound();
        //        }
        //        return Ok(pUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpGet]
        //[Route("GetEShopUsers/{PageNumber}")]
        //public async Task<IActionResult> GetEShopUsers(int PageNumber)
        //{
        //    try
        //    {
        //        return Ok(await _EShopUserService.SearchAsync(PageNumber));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpGet]
        //[Produces(typeof(EShopUser))]
        //[Route("GetEShopUser/{id}")]
        //public async Task<IActionResult> GetEShopUser(int id)
        //{
        //    try { 
        //    return Ok(await _EShopUserService.GetSingleAsync(filter: g => g.ID == id));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpPut]
        //[Route("PutEShopUser/{id}")]
        //[Produces(typeof(void))]
        //public async Task<IActionResult> PutEShopUser(int id, EShopUser EShopUser)
        //{
        //    try
        //    {
        //        var pusr = await _EShopUserService.GetSingleAsync(f => f.ID == id);
        //        if (ModelState.IsValid)
        //        {
        //            EncryptDecrypt encrypt = new();
        //            EShopUser.ConfirmPassword = encrypt.Encryptor(EShopUser.ConfirmPassword);
        //            EShopUser.Password = encrypt.Encryptor(EShopUser.Password);

        //            pusr.ConfirmPassword = pusr.Password;
        //            pusr.IsActive = EShopUser.IsActive;
        //            await _EShopUserService.UpdateAsync(pusr);
        //        }
        //        return Ok(pusr);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpPost]
        //[Route("PostEShopUser")]
        //[Produces(typeof(EShopUser))]
        //public async Task<IActionResult> PostEShopUser(EShopUser EShopUser)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    try { 
        //    EncryptDecrypt encrypt = new();
        //    EShopUser.ConfirmPassword = encrypt.Encryptor(EShopUser.ConfirmPassword);
        //    EShopUser.Password = encrypt.Encryptor(EShopUser.Password);

        //    if (EShopUser.UserName.Trim() == "Admin")
        //    {
        //        throw new Exception("Admin User Already exists");
        //    }
        //    await _EShopUserService.AddAsync(EShopUser);
        //    return CreatedAtRoute(routeName: "GetToken", routeValues: new { id = EShopUser.ID },value: EShopUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpPost]
        //[Route("InsertAdminUser")]
        //public async Task<IActionResult> InsertAdminUser()
        //{
        //    try
        //    {
        //        EncryptDecrypt encrypt = new();
        //        EShopUser user = new EShopUser();
        //        user.Address = "Admin";
        //        user.UserName = "Admin";
        //        user.Password = encrypt.Encryptor("123456Pp[]");
        //        user.ConfirmPassword = encrypt.Encryptor("123456Pp[]");
        //        user.FirstName = "Test";
        //        user.RoleName = EShopModels.Common.Roles.Admin;
        //        user.LastName = "Test";
        //        user.ActivationCode = Guid.NewGuid();
        //        user.CreatedDate = DateTime.Now;
        //        user.Email = "nalinvsacc@gmail.com";
        //        user.IsActive = true;
        //        user.IsDeleted = false;
        //        user.RowVersion = new byte[0];
        //        return Ok(await _EShopUserService.AddAsync(user));
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpGet]
        //[Route("{id}", Name = "GetToken")]
        //public async Task<IActionResult> GetUserToken(int id)
        //{
        //    try
        //    {
        //        string Token;
        //        EShopUser pUser = (EShopUser)await _EShopUserService.GetSingleAsync(u => u.ID == id);
        //        if (pUser == null)
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            var key = Encoding.ASCII.GetBytes("Jagath98989765123");
        //            var JWToken = new JwtSecurityToken(
        //                issuer: "http://localhost:5167/", audience: "http://localhost:5167/", claims: GetUserClaims(pUser),
        //                notBefore: new DateTimeOffset(DateTime.Now).DateTime,
        //                expires: new DateTimeOffset(DateTime.Now.AddDays(1)).DateTime,
        //                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        //            );
        //            Token = new JwtSecurityTokenHandler().WriteToken(JWToken);
        //            return Ok(Token);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        //[HttpDelete]
        //[Route("DeleteEShopUser/{id}")]
        //[Produces(typeof(EShopUser))]
        //public async Task<IActionResult> DeleteEShopUser(int id)
        //{
        //    try
        //    {
        //        EShopUser EShopUser = await _EShopUserService.GetSingleAsync(filter: m => m.ID == id);
        //        EShopUser.IsDeleted = !EShopUser.IsDeleted;
        //        EShopUser.ConfirmPassword = EShopUser.Password;
        //        await _EShopUserService.UpdateAsync(EShopUser);
        //        return Ok(EShopUser);
        //    }
        //    catch (Exception ex)
        //    {
        //        await ErrorMessages(ex);
        //        return StatusCode(500,ex);
        //    }
        //}

        private async Task ErrorMessages(Exception exception)
        {
            string USERNAME = ""; string EMAILID = ""; string Name = ""; 
            if (HttpContext.User.Identity.IsAuthenticated)
            {

                Claim claim = HttpContext.User.Claims.Where(b => b.Type == "USERNAME").FirstOrDefault();
                USERNAME = claim.Value;

                claim = HttpContext.User.Claims.Where(b => b.Type == "EMAILID").FirstOrDefault();
                EMAILID = claim.Value;

                Name = HttpContext.User.Identity.Name;
            }
            _logger.Log(LogLevel.Error, exception, "{Name},{USERNAME},{EMAILID},{TypeName}", new object[] { Name, USERNAME, EMAILID, this.GetType().Name });
        }
    }
}