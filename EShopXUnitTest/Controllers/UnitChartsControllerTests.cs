using Castle.Core.Logging;
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
    public class UnitChartsControllerTests
    {
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpContext> mockHttpContext;
        private readonly List<UnitChart> unitCharts=new();
        private readonly Mock<ILogger<UnitChartsController>> mocklogger;
        public UnitChartsControllerTests()
        { 
            contextAccessor = new Mock<IHttpContextAccessor>();
           
            unitCharts.Add(new UnitChart(1,1,10,"EA")
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now,               
                RowVersion = Array.Empty<byte>(),  
                UnitType = new UnitType("cc","nn",true)
                { 
                    CreatedDate = DateTime.Now,
                    ID = 1,                   
                    IsDeleted = false,
                    ModifiedDate = DateTime.Now, 
                    RowVersion = Array.Empty<byte>()
                }
            });

            unitCharts.Add(new UnitChart(2,1,100,"")
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
            mocklogger = new Mock<ILogger<UnitChartsController>>();
        }

        [Fact()]
        public async Task GetUnitChartsTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<UnitChart>>(MockBehavior.Strict);
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mockset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Set<UnitChart>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.UnitCharts).Returns(mockset.Object);

            Mock<EShopModels.Repository.IRepository<UnitChart>> repo = new();
            IUnitChartRepository mockrepo = new UnitChartRepository(mockDbContext.Object);
            unitofwork.Setup(c => c.UnitChartRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitChart>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitChartService mockservice = new EShopServices.Services.UnitChartService(work);
            UnitChartsController unitChartsController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var result = await unitChartsController.GetUnitCharts(1);
            mockRepository.Verify();
            Assert.IsType<List<EShopModels.UnitChart>>(((ObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetUnitChartsByProductTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<UnitChart>>(MockBehavior.Strict);
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mockset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mockset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.UnitCharts).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<UnitChart>()).Returns(mockset.Object);

            Mock<EShopModels.Repository.IRepository<UnitChart>> repo = new();
            IUnitChartRepository mockrepo = new UnitChartRepository(mockDbContext.Object);
            unitofwork.Setup(c => c.UnitChartRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitChart>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitChartService mockservice = new EShopServices.Services.UnitChartService(work);
            UnitChartsController unitChartsController = new(mockservice, contextAccessor.Object,mocklogger.Object);
            var result = await unitChartsController.GetUnitChartsByProduct(1);
            mockRepository.Verify();
            Assert.IsType<List<UnitChart>>(((ObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetUnitChartTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockdbset = new Mock<DbSet<UnitChart>>();
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mock = new();
            mock.Setup(h => h.UnitCharts).Returns(mockdbset.Object);
            mock.Setup(f => f.Set<UnitChart>()).Returns(mockdbset.Object);

            Mock<EShopModels.Repository.IRepository<UnitChart>> repo = new();
            repo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UnitChart, bool>>>()
            , It.IsAny<Func<IQueryable<UnitChart>, IOrderedQueryable<UnitChart>>>()
            , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<UnitChart, bool>> ex, Func<IQueryable<UnitChart>, IOrderedQueryable<UnitChart>> value, string vvc) =>
            {
                return await mockdbset.Object.SingleOrDefaultAsync(ex);
            });
            IUnitChartRepository mockrepo = new UnitChartRepository(mock.Object);
            unitofwork.Setup(c => c.UnitChartRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitChart>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitChartService mockservice = new EShopServices.Services.UnitChartService(work);
            UnitChartsController unitChartsController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            // Act
            var httpActionResult = await unitChartsController.GetUnitChart(1);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            // Assert
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.Equal(1, ((UnitChart)actionresult.Value).ID);
        }

        [Fact()]
        public async Task PutUnitChartTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            UnitChart unitChart = new(3,1,100,"")
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now,                
                RowVersion = Array.Empty<byte>() 
            };

            var mockdbset = new Mock<DbSet<UnitChart>>() { CallBase = false };
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(g => g.Attach(It.IsAny<UnitChart>())).Callback((UnitChart cg) => mockdbset.Object.Attach(cg));//.Returns(unitChart);

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<UnitChart>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.UnitCharts).Returns(mockdbset.Object);
            mockdbcontext.Setup(r => r.MarkAsModified(It.IsAny<UnitChart>())).Verifiable();
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<UnitChart>> repo = new();
            IUnitChartRepository mockrepo = new UnitChartRepository(mockdbcontext.Object);
            unitofwork.Setup(c => c.UnitChartRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitChart>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitChartService mockservice = new EShopServices.Services.UnitChartService(work);
            UnitChartsController unitChartsController = new(mockservice, contextAccessor.Object,mocklogger.Object);

            var httpActionResult = await unitChartsController.PutUnitChart(1, unitChart);
            var actionresult = httpActionResult as ObjectResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult); 
            Assert.IsType<UnitChart>(actionresult.Value);
        }

        [Fact()]
        public async Task PostUnitChartTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            UnitChart unitChart = new(1,1,100,"")
            {
                CreatedDate = DateTime.Now,
                ID = 4,
                ModifiedDate = DateTime.Now,                
                RowVersion = Array.Empty<byte>() 
            };

            var mockdbset = new Mock<DbSet<UnitChart>>() { CallBase = false };
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(f => f.Add(It.IsAny<UnitChart>())).Callback<UnitChart>((entity) => unitCharts.Add(entity));

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<UnitChart>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.UnitCharts).Returns(mockdbset.Object);
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<UnitChart>> repo = new();
            repo.Setup(g => g.Add(It.IsAny<UnitChart>())).Callback<UnitChart>((loc) => unitCharts.Add(loc)).Returns(unitChart);

            IUnitChartRepository mockrepo = new UnitChartRepository(mockdbcontext.Object);
            unitofwork.Setup(c => c.UnitChartRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitChart>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitChartService mockservice = new EShopServices.Services.UnitChartService(work);
            UnitChartsController unitChartsController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await unitChartsController.PostUnitChart(unitChart);
            var actionresult = httpActionResult as CreatedAtRouteResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.Equal("DefaultApi", actionresult.RouteName);
            Assert.Equal(unitChart.ID, actionresult.RouteValues["id"]);
            Assert.Equal(3, unitCharts.Count);
        }

        [Fact()]
        public async Task UpdateUnitChartTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            UnitChart unitChart = new(1,1,100,"")
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now,               
                RowVersion = Array.Empty<byte>() 
            };

            var mockdbset = new Mock<DbSet<UnitChart>>() { CallBase = false };
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            mockdbset.Setup(g => g.Attach(It.IsAny<UnitChart>())).Callback((UnitChart cg) => mockdbset.Object.Attach(cg));//.Returns(unitChart);

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<UnitChart>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.UnitCharts).Returns(mockdbset.Object);
            mockdbcontext.Setup(r => r.MarkAsModified(It.IsAny<UnitChart>())).Verifiable();
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<UnitChart>> repo = new();
            IUnitChartRepository mockrepo = new UnitChartRepository(mockdbcontext.Object);
            unitofwork.Setup(c => c.UnitChartRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitChart>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitChartService mockservice = new EShopServices.Services.UnitChartService(work);
            UnitChartsController unitChartsController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await unitChartsController.PutUnitChart(1, unitChart);
            var actionresult = httpActionResult as ObjectResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult);            
            Assert.IsType<UnitChart>(actionresult.Value);
        }

        [Fact()]
        public async Task DeleteUnitChartTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockdbset = new Mock<DbSet<UnitChart>>();
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mockdbset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            mockdbset.Setup(u => u.Attach(It.IsAny<UnitChart>())).Verifiable();
            mockdbset.Setup(m => m.Remove(It.IsAny<UnitChart>())).Callback<UnitChart>((entity) => unitCharts.Remove(entity));
            //mockdbset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockdbset.Object);

            Mock<ApplicationDbContext> dbmock = new() { CallBase = true };
            dbmock.Setup(u => u.Set<UnitChart>()).Returns(mockdbset.Object);
            dbmock.Setup(n => n.UnitCharts).Returns(mockdbset.Object);
            dbmock.Setup(r => r.MarkAsModified(It.IsAny<Inventory>())).Verifiable();
            dbmock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<UnitChart>> repo = new();
            repo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<UnitChart, bool>>>()
            , It.IsAny<Func<IQueryable<UnitChart>, IOrderedQueryable<UnitChart>>>()
            , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<UnitChart, bool>> ex, Func<IQueryable<UnitChart>, IOrderedQueryable<UnitChart>> value, string vvc) =>
            {
                return await mockdbset.Object.SingleOrDefaultAsync(ex);
            });

            IUnitChartRepository mockrepo = new UnitChartRepository(dbmock.Object);
            unitofwork.Setup(c => c.UnitChartRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<UnitChart>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IUnitChartService mockservice = new EShopServices.Services.UnitChartService(work);
            UnitChartsController unitChartsController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await unitChartsController.DeleteUnitChart(1, 1);
            var actionresult = httpActionResult as ObjectResult;
            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);

            Assert.NotEmpty(((List<UnitChart>)actionresult.Value));
        }
    }
}