using EShopModels;
using EShopModels.Common;
using EShopModels.Repository;
using EShopModels.Services;
using EShopRepository;
using EShopRepository.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using XUnitTestProject;

namespace EShopApi.Controllers.Tests
{
    public class UnitTypesControllerTests
    {
        private readonly List<UnitType> unitTypes = new(); 
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<HttpContext> mockHttpContext;
        private Mock<ILogger<UnitTypesController>> mocklogger;
        public UnitTypesControllerTests()
        {
            contextAccessor = new Mock<IHttpContextAccessor>();
            unitTypes.Add(new UnitType("","EA",true)
            {
                CreatedDate = DateTime.Now,
                ID = 1,               
                ModifiedDate = DateTime.Now,               
                RowVersion = Array.Empty<byte>() 
            });

            unitTypes.Add(new UnitType("", "Box",true)
            {
                CreatedDate = DateTime.Now,
                ID = 2,               
                ModifiedDate = DateTime.Now,               
                RowVersion = Array.Empty<byte>() 
            }); 
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
            mocklogger = new Mock<ILogger<UnitTypesController>>();
        }

        [Fact()]
        public async Task GetUnitTypesTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<UnitType>>(MockBehavior.Strict);
            mockset.As<IQueryable<UnitType>>().Setup(m => m.Expression).Returns(unitTypes.AsQueryable().Expression);
            mockset.As<IQueryable<UnitType>>().Setup(m => m.ElementType).Returns(unitTypes.AsQueryable().ElementType);
            mockset.As<IQueryable<UnitType>>().Setup(m => m.GetEnumerator()).Returns(unitTypes.GetEnumerator());
            mockset.As<IAsyncEnumerable<UnitType>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitType>(unitTypes.GetEnumerator()));
            mockset.As<IQueryable<UnitType>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitType>(unitTypes.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new();            
            mockDbContext.Setup(u => u.Set<UnitType>()).Returns(mockset.Object);
            mockDbContext.Setup(n => n.UnitTypes).Returns(mockset.Object);

            Mock<EShopModels.Repository.IRepository<UnitType>> repo = new();
            repo.Setup(c => c.GetAllAsync()).Returns(async () =>
            {
                return await mockset.Object.ToListAsync();
            });

            IUnitTypeRepository mockrepo = new UnitTypeRepository(mockDbContext.Object);
            unitofwork.Setup(c => c.UnitTypeRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitType>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));            

            IUnitTypeService mockservice = new EShopServices.Services.UnitTypeService(unitofwork.Object);
            UnitTypesController unitTypesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var result = await unitTypesController.GetUnitTypes(1);
            mockRepository.Verify();
            var contentresult = result as OkObjectResult;
            Assert.IsType<List<UnitType>>(contentresult.Value);
        }

        [Fact()]
        public async Task GetUnitTypeTest()
        {
            int UnitTypeID = 1;
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockdbset = new Mock<DbSet<UnitType>>();
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Expression).Returns(unitTypes.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.ElementType).Returns(unitTypes.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.GetEnumerator()).Returns(unitTypes.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitType>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitType>(unitTypes.GetEnumerator()));
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitType>(unitTypes.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mock = new();
            mock.Setup(h => h.UnitTypes).Returns(mockdbset.Object);
            mock.Setup(f => f.Set<UnitType>()).Returns(mockdbset.Object);

            Mock<EShopModels.Repository.IRepository<UnitType>> repo = new();
            repo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UnitType, bool>>>()
            , It.IsAny<Func<IQueryable<UnitType>, IOrderedQueryable<UnitType>>>()
            , It.IsAny<string>())).Returns(async
            (System.Linq.Expressions.Expression<Func<UnitType, bool>> ex, Func<IQueryable<UnitType>, IOrderedQueryable<UnitType>> value, string vvc) =>
            {
                return await mockdbset.Object.SingleOrDefaultAsync(ex);
            });

            IUnitTypeRepository mockrepo = new UnitTypeRepository(mock.Object);
            unitofwork.Setup(c => c.UnitTypeRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitType>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitTypeService mockservice = new EShopServices.Services.UnitTypeService(unitofwork.Object);
            UnitTypesController unitTypesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            // Act
            var httpActionResult = await unitTypesController.GetUnitType(UnitTypeID);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            // Assert
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.Equal(((UnitType)actionresult.Value).ID, UnitTypeID);
        }

        [Fact()]
        public async Task GetUnitTypesTest1()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<UnitType>>(MockBehavior.Strict);
            mockset.As<IQueryable<UnitType>>().Setup(m => m.Expression).Returns(unitTypes.AsQueryable().Expression);
            mockset.As<IQueryable<UnitType>>().Setup(m => m.ElementType).Returns(unitTypes.AsQueryable().ElementType);
            mockset.As<IQueryable<UnitType>>().Setup(m => m.GetEnumerator()).Returns(unitTypes.GetEnumerator());
            mockset.As<IAsyncEnumerable<UnitType>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitType>(unitTypes.GetEnumerator()));
            mockset.As<IQueryable<UnitType>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitType>(unitTypes.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Set<UnitType>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.UnitTypes).Returns(mockset.Object);

            Mock<EShopModels.Repository.IRepository<UnitType>> repo = new();
            repo.Setup(c => c.GetAllAsync()).Returns(async () =>
            {
                return await mockset.Object.ToListAsync();
            });

            IUnitTypeRepository mockrepo = new UnitTypeRepository(mockDbContext.Object);
            unitofwork.Setup(c => c.UnitTypeRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitType>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitTypeService mockservice = new EShopServices.Services.UnitTypeService(unitofwork.Object);
            UnitTypesController unitTypesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var result = await unitTypesController.GetUnitTypes(1);
            var actionresult = result as ObjectResult;
            mockRepository.Verify();
            Assert.NotNull(result);
            Assert.NotNull(actionresult);
            Assert.IsType<List<EShopModels.UnitType>>(actionresult.Value);
        }

        [Fact()]
        public async Task PutUnitTypeTest()
        {
            UnitType unitType = new("cr12", "EA", true)
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now,
                RowVersion = Array.Empty<byte>()
            };
            
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockdbset = new Mock<DbSet<UnitType>>() { CallBase = false };
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Expression).Returns(unitTypes.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.ElementType).Returns(unitTypes.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.GetEnumerator()).Returns(unitTypes.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitType>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitType>(unitTypes.GetEnumerator()));
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitType>(unitTypes.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(g => g.Attach(It.IsAny<UnitType>())).Callback((UnitType cg) => mockdbset.Object.Attach(cg));//.Returns(unitType);

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<UnitType>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.UnitTypes).Returns(mockdbset.Object);
            mockdbcontext.Setup(r => r.MarkAsModified(It.IsAny<UnitType>())).Verifiable();
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<UnitType>> repo = new();
            IUnitTypeRepository mockrepo = new UnitTypeRepository(mockdbcontext.Object);
            unitofwork.Setup(c => c.UnitTypeRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitType>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitTypeService mockservice = new EShopServices.Services.UnitTypeService(unitofwork.Object);
            UnitTypesController unitTypesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await unitTypesController.PutUnitType(1, unitType);
            var actionresult = httpActionResult as ObjectResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.IsType<UnitType>(actionresult.Value);
        }

        [Fact()]
        public async Task PostUnitTypeTest()
        {
            UnitType unitType = new("", "TEST",true)
            {
                CreatedDate = DateTime.Now,
                ID = 3,               
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>()
            };
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockdbset = new Mock<DbSet<UnitType>>() { CallBase = false };
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Expression).Returns(unitTypes.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.ElementType).Returns(unitTypes.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.GetEnumerator()).Returns(unitTypes.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitType>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitType>(unitTypes.GetEnumerator()));
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitType>(unitTypes.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(f => f.Add(It.IsAny<UnitType>())).Callback<UnitType>((entity) => unitTypes.Add(entity));

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<UnitType>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.UnitTypes).Returns(mockdbset.Object);
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IUnitTypeRepository mockrepo = new UnitTypeRepository(mockdbcontext.Object);
            Mock<EShopModels.Repository.IRepository<UnitType>> repo = new();
            repo.Setup(g => g.Add(It.IsAny<UnitType>())).Callback<UnitType>((loc) => unitTypes.Add(loc)).Returns(unitType);

            unitofwork.Setup(c => c.UnitTypeRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitType>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitTypeService mockservice = new EShopServices.Services.UnitTypeService(unitofwork.Object);
            UnitTypesController unitTypesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await unitTypesController.PostUnitType(unitType);
            var actionresult = httpActionResult as CreatedAtRouteResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.Equal("DefaultApi", actionresult.RouteName);
            Assert.Equal(unitType.ID, actionresult.RouteValues["id"]);
            Assert.Equal(3, unitTypes.Count);
        }

        [Fact()]
        public async Task DeleteUnitTypeTest()
        {
            int UnitTypeID = 1;
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockdbset = new Mock<DbSet<UnitType>>();
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Expression).Returns(unitTypes.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.ElementType).Returns(unitTypes.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.GetEnumerator()).Returns(unitTypes.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitType>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitType>(unitTypes.GetEnumerator()));
            mockdbset.As<IQueryable<UnitType>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitType>(unitTypes.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(u => u.Attach(It.IsAny<UnitType>())).Verifiable();
            mockdbset.Setup(m => m.Remove(It.IsAny<UnitType>())).Callback<UnitType>((entity) => unitTypes.Remove(entity));

            Mock<ApplicationDbContext> dbmock = new() { CallBase = true };
            dbmock.Setup(u => u.Set<UnitType>()).Returns(mockdbset.Object);
            dbmock.Setup(n => n.UnitTypes).Returns(mockdbset.Object);
            dbmock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IUnitTypeRepository mockrepo = new UnitTypeRepository(dbmock.Object);
            Mock<EShopModels.Repository.IRepository<UnitType>> repo = new();
            repo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UnitType, bool>>>()
            , It.IsAny<Func<IQueryable<UnitType>, IOrderedQueryable<UnitType>>>()
            , It.IsAny<string>())).Returns(async
            (System.Linq.Expressions.Expression<Func<UnitType, bool>> ex, Func<IQueryable<UnitType>, IOrderedQueryable<UnitType>> value, string vvc) =>
            {
                return await mockdbset.Object.SingleOrDefaultAsync(ex);
            });

            unitofwork.Setup(c => c.UnitTypeRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitType>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitTypeService mockservice = new EShopServices.Services.UnitTypeService(unitofwork.Object);
            UnitTypesController unitTypesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await unitTypesController.DeleteUnitType(UnitTypeID);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.True(((UnitType)actionresult.Value).IsDeleted);
        }
    }
}