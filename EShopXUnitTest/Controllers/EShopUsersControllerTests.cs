using Castle.Core.Logging;
using EShopModels;
using EShopModels.Common;
using EShopModels.Repository;
using EShopModels.Services;
using EShopRepository;
using EShopRepository.Repositories;
using EShopServices.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using XUnitTestProject;

namespace EShopApi.Controllers.Tests
{
    public class EShopUsersControllerTests
    {
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpContext> mockHttpContext; 
        private readonly List<EShopUser> EShopUsers=new();
        private readonly List<LoginDetail> loginDetails=new();
        private readonly Mock<ILogger<EShopUsersController>> mocklogger;

        public EShopUsersControllerTests()
        { 
            EShopUsers.Add(new EShopModels.EShopUser(1,"test123", "nalin","Jagath","","nalinmyid@gmail.com",true,Guid.NewGuid(),"",Roles.User)
            {
                ID = 1, 
                ConfirmPassword = "123",
                CreatedDate = DateTime.Now,  
                IsActive = true, 
                ModifiedDate = DateTime.Now,               
                RowVersion = Array.Empty<byte>() 
            });

            EShopUsers.Add(new EShopModels.EShopUser(2, "semyid@gmail.com","jagath","tertert","", "semyid@gmail.com", true, Guid.NewGuid(),"123",Roles.User)
            {  
                ConfirmPassword = "321",
                CreatedDate = DateTime.Now, 
                IsActive = true,
                LastName = "nalin",
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
            });

            loginDetails.Add(new LoginDetail(2,DateTime.Now, DateTime.Now)
            {
                ID = 2 
            });
             
            contextAccessor = new Mock<IHttpContextAccessor>();
            string value = "1";
            byte[] val = new byte[] { 1 };
            mockHttpContext = new Mock<Microsoft.AspNetCore.Http.HttpContext>();
            Mock<ISession> mocks = new();
            mocks.Setup(c => c.Set("JWToken", It.IsAny<byte[]>())).Callback<string, byte[]>((k, v) => value = v.ToString());
            mocks.Setup(v => v.TryGetValue("JWToken", out val)).Returns(true);
            mockHttpContext.Setup(v => v.Session).Returns(mocks.Object);

            IList<Claim> claimCollection = new List<Claim> { new Claim("name", "John Doe") };
            var identityMock = new Mock<ClaimsIdentity>();
            identityMock.Setup(x => x.Claims).Returns(claimCollection);
            identityMock.Setup(x => x.IsAuthenticated).Returns(true);
            var user = new Mock<ClaimsPrincipal>();
            user.Setup(m => m.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            user.Setup(m => m.Identity).Returns(identityMock.Object);
            user.Setup(m => m.Claims).Returns(claimCollection);
            mockHttpContext.Setup(g => g.User).Returns(user.Object);
            contextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext.Object);
            mocklogger = new Mock<ILogger<EShopUsersController>>();
        }
 
        [Fact()]
        public async Task ValidateTokenTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            EShopUser EShopUser = new(2,"jagath","jagath", "nalin","tertert","semyid@gmail.com",true,Guid.NewGuid(),"",Roles.User)
            {
                ActivationCode = Guid.NewGuid(),
                ConfirmPassword = "321",
                CreatedDate = DateTime.Now,  
                IsActive = true, 
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>(),
                ID = 3
            };

            var mockdbset = new Mock<DbSet<EShopUser>>();
            mockdbset.As<IQueryable<EShopUser>>().Setup(m => m.Expression).Returns(EShopUsers.AsQueryable().Expression);
            mockdbset.As<IQueryable<EShopUser>>().Setup(m => m.ElementType).Returns(EShopUsers.AsQueryable().ElementType);
            mockdbset.As<IQueryable<EShopUser>>().Setup(m => m.GetEnumerator()).Returns(EShopUsers.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<EShopUser>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<EShopUser>(EShopUsers.GetEnumerator()));
            mockdbset.As<IQueryable<EShopUser>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<EShopUser>(EShopUsers.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mock = new();
            mock.Setup(h => h.Users).Returns(mockdbset.Object);
            mock.Setup(f => f.Set<EShopUser>()).Returns(mockdbset.Object);
            IUnitOfWork unitOfWork = new UnitOfWork(mock.Object);

            IEShopUserService mockservice = new EShopUserService(unitOfWork); 
            EShopUsersController EShopUsersController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            LoginView loginView = new("jkpjnjaya@gmail.com","123456",true);
 
            // Act
            var httpActionResult = await EShopUsersController.ValidateToken(loginView);
            mockRepository.Verify();
            Assert.NotNull(httpActionResult);
            if (httpActionResult is OkObjectResult result)
            {
                Assert.IsType<EShopUser>(result.Value);
            }
            else if (httpActionResult is NotFoundResult)
            {
                Assert.IsType<NotFoundResult>(httpActionResult);
            }
        }  
    }
}