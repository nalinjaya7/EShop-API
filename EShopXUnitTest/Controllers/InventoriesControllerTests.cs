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
    public class InventoriesControllerTests
    {
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpContext> mockHttpContext;
        private Mock<ILogger<InventoriesController>> mocklogger;
        private readonly List<Inventory> inventories = new(); 

        public InventoriesControllerTests()
        {
            Inventory inventory1 = new("INV002", 1, 1, 10, 1, 120, 8, 1)
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now,
                RowVersion = Array.Empty<byte>(), 
                Product = new Product(1, 1, "Ped01", "ITM001", "ITEM001", 6, 1, false, 0.6m)
                {
                    CreatedDate = DateTime.Now,
                    ID = 5, 
                    Inventories = new List<Inventory>()
                },
                UnitChart = new UnitChart(8, 76, 90, "EA")
                {
                    CreatedDate = DateTime.Now,
                    ID = 9,
                    ModifiedDate = DateTime.Now, 
                    RowVersion = Array.Empty<byte>()
                }
            };

            Inventory inventory2 = new("INV011", 1, 2, 10, 1, 43.50m, 40.00m, 3)
            {
                CreatedDate = DateTime.Now,
                ID = 2,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>(), 
                Product = new Product(1, 1, "Product 01", "PRD001", "PRD001", 3, 1, false, 0.4m) { },
                UnitChart = new UnitChart(1, 1, 4, "EA") { }
            }; 
            inventories.Add(inventory1);
            inventories.Add(inventory2);
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
            mocklogger = new Mock<ILogger<InventoriesController>>();
        }

        [Fact()]
        public async Task GetInventoriesTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Inventory>>(MockBehavior.Strict);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mockset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
 
            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Inventories).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Inventory>> repo = new();
            IInventoryRepository mockrepo = new InventoryRepository(mockDbContext.Object);
            repo.Setup(c => c.GetAllAsync()).Returns(async () =>
            {
                return await mockset.Object.ToListAsync();
            });

            unitofwork.Setup(c => c.InventoryRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IInventoryService service = new InventoryService(work);
            InventoriesController inventoriesController = new(service, contextAccessor.Object, mocklogger.Object);

            var result = await inventoriesController.GetInventories(1);
            mockRepository.Verify();
            Assert.IsType<List<Inventory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetInventoriesTest1()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Inventory>>(MockBehavior.Strict);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mockset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
 
            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Inventories).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Inventory>> repo = new();
            IInventoryRepository mockrepo = new InventoryRepository(mockDbContext.Object);
            repo.Setup(c => c.GetAllAsync()).Returns(async () =>
            {
                return await mockset.Object.ToListAsync();
            });

            unitofwork.Setup(c => c.InventoryRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IInventoryService service = new InventoryService(work);
            InventoriesController inventoriesController = new(service,contextAccessor.Object, mocklogger.Object);

            var result = await inventoriesController.GetInventories(1);
            mockRepository.Verify();
            Assert.IsType<List<Inventory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetInventoriesForSearchTest()
        {
            Inventory inventory1 = new("PRD001",1,1,10,1,42,40,4)
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now,
                RowVersion = Array.Empty<byte>(), 
                Product = new Product(1,1,"Product 1","PRD001","PRD001",5,3,false,0.3m)
                { 
                    CreatedDate = DateTime.Now,
                    ID = 5, 
                    Inventories = new List<Inventory>() 
                },
                UnitChart = new UnitChart(8,76,90,"EA")
                {
                    CreatedDate = DateTime.Now,
                    ID = 9,
                    ModifiedDate = DateTime.Now, 
                    RowVersion = Array.Empty<byte>()
                }
            };
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Inventory>>(MockBehavior.Strict);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mockset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
 
            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Inventories).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Inventory>> repo = new();
            IInventoryRepository mockrepo = new InventoryRepository(mockDbContext.Object);
            repo.Setup(c => c.GetAllAsync()).Returns(async () =>
            {
                return await mockset.Object.ToListAsync();
            });

            unitofwork.Setup(c => c.InventoryRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IInventoryService service = new InventoryService(work);
            InventoriesController inventoriesController = new(service, contextAccessor.Object, mocklogger.Object);

            var result = await inventoriesController.GetInventoriesForSearch(inventory1);
            mockRepository.Verify();
            Assert.IsType<List<Inventory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetInventoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Inventory>>(MockBehavior.Strict);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mockset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
 
            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Inventories).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Inventory>> repo = new();
            IInventoryRepository mockrepo = new InventoryRepository(mockDbContext.Object);
            repo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Inventory, bool>>>()
              , It.IsAny<Func<IQueryable<Inventory>, IOrderedQueryable<Inventory>>>()
              , It.IsAny<string>())).Returns(async
              (System.Linq.Expressions.Expression<Func<Inventory, bool>> ex, Func<IQueryable<Inventory>> value, string vvc) =>
              {
                  return await mockset.Object.SingleOrDefaultAsync(ex);
              });

            unitofwork.Setup(c => c.InventoryRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IInventoryService service = new InventoryService(work);
            InventoriesController inventoriesController = new(service, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await inventoriesController.GetInventory(1);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            // Assert
            Assert.NotNull(actionresult.Value);
            Assert.NotNull(actionresult.Value);
            Assert.Equal(1, ((Inventory)actionresult.Value).ID);
        }

        [Fact()]
        public async Task PutInventoryTest()
        {
            Inventory inventory2 = new("INV002",1,3,10,1,78,70,9)
            {
                CreatedDate = DateTime.Now,
                ID = 3,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>(),  
                Product = new Product(1,1,"Product 001","PRD001","PRD001",5,1,false,0.4m) { },
                UnitChart = new UnitChart(1,1,56,"BOX") { } 
            };

            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockdbset = new Mock<DbSet<Inventory>>() { CallBase = false };
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(g => g.Attach(It.IsAny<Inventory>())).Callback((Inventory cg) => mockdbset.Object.Attach(cg));//.Returns(inventory2);

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<Inventory>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.Inventories).Returns(mockdbset.Object);
            mockdbcontext.Setup(r => r.MarkAsModified(It.IsAny<Inventory>())).Verifiable();
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<Inventory>> repo = new();
            IInventoryRepository mockrepo = new InventoryRepository(mockdbcontext.Object);

            unitofwork.Setup(c => c.InventoryRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IInventoryService service = new InventoryService(unitofwork.Object);
            InventoriesController inventoriesController = new(service, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await inventoriesController.PutInventory(1, inventory2);
            var actionresult = httpActionResult as ObjectResult;

            mockRepository.Verify();

            Assert.NotNull(actionresult); 
            Assert.IsType<Inventory>(actionresult.Value);
        }

        [Fact()]
        public async Task PostInventoryTest()
        {
            Inventory inventory2 = new("Inv1",1,3,10,1,450,400,10)
            {
                CreatedDate = DateTime.Now,
                ID = 3,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
            };

            var mockset = new Mock<DbSet<Inventory>>();
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mockset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mockset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mockset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Inventories).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Inventory>> repo = new();
            repo.Setup(g => g.Add(It.IsAny<Inventory>())).Returns(inventory2);
            IInventoryRepository mockrepo = new InventoryRepository(mockDbContext.Object);

            MockRepository mockRepository = new(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
            Mock<EShopModels.IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            unitofwork.Setup(c => c.InventoryRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            unitofwork.Setup(c => c.Repository<Inventory>().Add(It.IsAny<Inventory>())).Callback<Inventory>((s) => inventories.Add(s)).Returns(inventory2);

            IInventoryService service = new InventoryService(unitofwork.Object);
            InventoriesController inventoriesController = new(service, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await inventoriesController.PostInventory(inventory2);
            var actionresult = httpActionResult as CreatedAtRouteResult;

            mockRepository.Verify();

            Assert.NotNull(actionresult);
            Assert.Equal("DefaultApi", actionresult.RouteName);
            Assert.Equal(inventory2.ID, actionresult.RouteValues["id"]);
            Assert.Equal(3, inventories.Count);
        }

        [Fact()]
        public async Task DeleteInventoryTest()
        {
            int invID = 1;
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockdbset = new Mock<DbSet<Inventory>>();
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mockdbset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(u => u.Attach(It.IsAny<Inventory>())).Verifiable();
            mockdbset.Setup(m => m.Remove(It.IsAny<Inventory>())).Callback<Inventory>((entity) => inventories.Remove(entity));

            Mock<ApplicationDbContext> dbmock = new() { CallBase = true };
            dbmock.Setup(u => u.Set<Inventory>()).Returns(mockdbset.Object);
            dbmock.Setup(n => n.Inventories).Returns(mockdbset.Object);
            dbmock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<Inventory>> repo = new();
            repo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Inventory, bool>>>()
             , It.IsAny<Func<IQueryable<Inventory>, IOrderedQueryable<Inventory>>>()
             , It.IsAny<string>())).Returns(async
             (System.Linq.Expressions.Expression<Func<Inventory, bool>> ex, Func<IQueryable<Inventory>> value, string vvc) =>
             {
                 return await mockdbset.Object.SingleOrDefaultAsync(ex);
             });

            IInventoryRepository mockrepo = new InventoryRepository(dbmock.Object);

            unitofwork.Setup(c => c.InventoryRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(repo.Object);
            unitofwork.Setup(x => x.CompleteAsync()).ReturnsAsync(1);
            IUnitOfWork work = unitofwork.Object;

            IInventoryService service = new InventoryService(work);
            InventoriesController inventoriesController = new(service, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await inventoriesController.DeleteInventory(invID);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.True(((Inventory)actionresult.Value).IsDeleted);
        }
    }
}