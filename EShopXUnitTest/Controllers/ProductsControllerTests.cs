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
    public class ProductsControllerTests
    {
        private readonly List<Inventory> inventories=new();
        private readonly List<Product> products=new(); 
        private readonly List<UnitChart> unitCharts=new();
        private readonly Mock<ILogger<ProductsController>> mocklogger;
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<HttpContext> mockHttpContext;

        public ProductsControllerTests()
        {  
            products.Add(new Product(1,1,"Pname","B001","I001",1,1,false,5)
            { 
                CreatedDate = DateTime.Now,
                ID = 1,
                IsDeleted = false,
                ModifiedDate = DateTime.Now,
                RowVersion = Array.Empty<byte>(),
                Inventories = new List<Inventory>()
                {
                    new Inventory("INV03",1,1,1,1,321,123,12)
                    {
                        ID=1, CreatedDate=DateTime.Now, IsDeleted=false, ModifiedDate=DateTime.Now,
                         RowVersion=Array.Empty<byte>(), 
                         Product = new Product(1,1,"Pname","B001","I001",1,1,false,5){ID=1}
                    }
                }
            });

            products.Add(new Product(1, 1, "Pname", "B002", "I002", 1, 1, false, 5)
            { 
                CreatedDate = DateTime.Now,
                ID = 2,
                IsDeleted = false,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>(),
                Inventories = new List<Inventory>()
                {
                    new Inventory("INV001",2,1,1,1,921,876,12)
                    {
                        ID=1, BatchID=1, CreatedDate=DateTime.Now, IsDeleted=false,ModifiedDate=DateTime.Now,
                        RowVersion=Array.Empty<byte>(),Product = new Product(1, 1, "Pname", "B002", "I002", 1, 1, false, 5){ID=2}
                    }
                }
            });
 
            inventories.Add(new Inventory("Inv001",1,1,300,1,17,12,0)
            { 
                CreatedDate = DateTime.Now,
                ID = 1,
                IsDeleted = false,
                ModifiedDate = DateTime.Now, 
                ReservedQuantity = 0,
                RowVersion = Array.Empty<byte>(),
                Product = new Product(1,1,"Pname","B001","I001",1,1,false,4)
                { 
                    CreatedDate = DateTime.Now,
                    ID = 1,
                    IsDeleted = false,
                    ModifiedDate = DateTime.Now, 
                    RowVersion = Array.Empty<byte>()
                }
            });

            inventories.Add(new Inventory("Inv001",2,1,300,2,17,12,0)
            { 
                CreatedDate = DateTime.Now,
                ID = 2,
                IsDeleted = false,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>(),
                Product = new Product(1,1,"Pname","B002","I002",1,1,false,4)
                { 
                    CreatedDate = DateTime.Now,
                    ID = 2,
                    IsDeleted = false,
                    ModifiedDate = DateTime.Now, 
                    RowVersion = Array.Empty<byte>()
                }
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
            mocklogger = new Mock<ILogger<ProductsController>>();
        }

        [Fact()]
        public async Task GetProducts2Test()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetProducts(1);
            mockRepository.Verify();
            Assert.IsType<List<Product>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetAllProductsTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetAllProducts();
            mockRepository.Verify();
            Assert.IsType<List<Product>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductsByCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetProductsByCategory(1);
            mockRepository.Verify();
            Assert.IsType<List<Product>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductsTest1()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetProducts(1);
            mockRepository.Verify();
            Assert.IsType<List<Product>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductsTest2()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetProducts(1);
            mockRepository.Verify();
            Assert.IsType<List<Product>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProducts3Test()
        {
            Product product = new(1,1,"Pname","B001","I001",1,1,false,5)
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                IsDeleted = false,
                ModifiedDate = DateTime.Now, 
                ReOrderLevel = 1,
                RowVersion = Array.Empty<byte>(),
                TaxGroupID = 1,
                TaxInclude = false,
                TaxRate = 5,
                Inventories = new List<Inventory>()
                {
                    new Inventory("INV001",1,1,1,1,43,40,32)
                    {
                        ID=1,CreatedDate=DateTime.Now, IsDeleted=false, ModifiedDate=DateTime.Now,
                       ReservedQuantity=12, RowVersion=Array.Empty<byte>(),
                          Product = new Product(1,1,"Pname","B001","I001",1,1,false,5){ID=1}
                    }
                }
            };

            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.AsQueryable().GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.AsQueryable().GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));
            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
 
            var INmockset = new Mock<DbSet<Inventory>>();
            INmockset.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            INmockset.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            INmockset.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.AsQueryable().GetEnumerator());
            INmockset.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.AsQueryable().GetEnumerator()));
            INmockset.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));
            INmockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => INmockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            INmockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await INmockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
 
            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(INmockset.Object);
            mockDbContext.Setup(d => d.Inventories).Returns(INmockset.Object);
            mockDbContext.Setup(v => v.Entry<Product>(It.IsAny<Product>())).Throws<InvalidOperationException>();
 
            var mockedDbTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            var dbconnect = new Mock<System.Data.Common.DbConnection>();
            dbconnect.Setup(g => g.ConnectionString).Returns(It.IsAny<string>());
            mockedDbTransaction.Setup(j => j.Commit());
            var mockexecutionstrategy = new Mock<Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>();
            var databaseFacadeMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(mockDbContext.Object);
            databaseFacadeMock.Setup(x => x.EnsureCreated()).Returns(true);
            databaseFacadeMock.Setup(c => c.CreateExecutionStrategy()).Returns(mockexecutionstrategy.Object);
            mockDbContext.SetupGet(c => c.Database).Returns(databaseFacadeMock.Object);
            mockDbContext.Setup(x => x.Database.BeginTransaction()).Returns(mockedDbTransaction.Object);

            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);
            Mock<EShopModels.Repository.IRepository<Inventory>> Invrepo = new();
            IInventoryRepository Invmockrepo = new InventoryRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            unitofwork.Setup(c => c.InventoryRepository).Returns(Invmockrepo);
            unitofwork.Setup(c => c.Repository<Inventory>()).Returns(Invrepo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object); 
            Task task() => productsController.GetProducts();
            InvalidOperationException ex = await Assert.ThrowsAsync<InvalidOperationException>(task);            
            mockRepository.Verify();
            //Assert.IsType<List<Product>>((objectResult).Value);
        }

        [Fact()]
        public async Task GetProductTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));
            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            var mocksetuc = new Mock<DbSet<UnitChart>>();
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mocksetuc.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));
            mocksetuc.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mocksetuc.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mocksetuc.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mocksetuc.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            var mocksetinv = new Mock<DbSet<Inventory>>();
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.AsQueryable().GetEnumerator());
            mocksetinv.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.AsQueryable().GetEnumerator()));
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));
            mocksetinv.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mocksetinv.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mocksetinv.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mocksetinv.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mocksetinv.Object);
            mockDbContext.Setup(d => d.Inventories).Returns(mocksetinv.Object);
            mockDbContext.Setup(d => d.Set<UnitChart>()).Returns(mocksetuc.Object);
            mockDbContext.Setup(d => d.UnitCharts).Returns(mocksetuc.Object);

            var mockedDbTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            var dbconnect = new Mock<System.Data.Common.DbConnection>();
            dbconnect.Setup(g => g.ConnectionString).Returns(It.IsAny<string>());
            mockedDbTransaction.Setup(j => j.Commit());
            var mockexecutionstrategy = new Mock<Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>();
            var databaseFacadeMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(mockDbContext.Object);
            databaseFacadeMock.Setup(x => x.EnsureCreated()).Returns(true);
            databaseFacadeMock.Setup(c => c.CreateExecutionStrategy()).Returns(mockexecutionstrategy.Object);
            mockDbContext.SetupGet(c => c.Database).Returns(databaseFacadeMock.Object);
            mockDbContext.Setup(x => x.Database.BeginTransaction()).Returns(mockedDbTransaction.Object);

            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetProduct(1);
            mockRepository.Verify();
            Assert.IsType<Product>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductTest1()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));
            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            var mocksetuc = new Mock<DbSet<UnitChart>>();
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.GetEnumerator());
            mocksetuc.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.GetEnumerator()));
            mocksetuc.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));
            mocksetuc.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mocksetuc.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mocksetuc.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mocksetuc.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            var mocksetinv = new Mock<DbSet<Inventory>>();
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.AsQueryable().GetEnumerator());
            mocksetinv.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.AsQueryable().GetEnumerator()));
            mocksetinv.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));
            mocksetinv.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mocksetinv.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mocksetinv.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mocksetinv.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mocksetinv.Object);
            mockDbContext.Setup(d => d.Inventories).Returns(mocksetinv.Object);
            mockDbContext.Setup(d => d.Set<UnitChart>()).Returns(mocksetuc.Object);
            mockDbContext.Setup(d => d.UnitCharts).Returns(mocksetuc.Object);

            var mockedDbTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            var dbconnect = new Mock<System.Data.Common.DbConnection>();
            dbconnect.Setup(g => g.ConnectionString).Returns(It.IsAny<string>());
            mockedDbTransaction.Setup(j => j.Commit());
            var mockexecutionstrategy = new Mock<Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>();
            var databaseFacadeMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(mockDbContext.Object);
            databaseFacadeMock.Setup(x => x.EnsureCreated()).Returns(true);
            databaseFacadeMock.Setup(c => c.CreateExecutionStrategy()).Returns(mockexecutionstrategy.Object);
            mockDbContext.SetupGet(c => c.Database).Returns(databaseFacadeMock.Object);
            mockDbContext.Setup(x => x.Database.BeginTransaction()).Returns(mockedDbTransaction.Object);

            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetProduct(1);
            mockRepository.Verify();
            Assert.IsType<Product>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductInventoriesTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<Product>>();
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));
            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            var mocksetInv = new Mock<DbSet<Inventory>>();
            mocksetInv.As<IQueryable<Inventory>>().Setup(m => m.Expression).Returns(inventories.AsQueryable().Expression);
            mocksetInv.As<IQueryable<Inventory>>().Setup(m => m.ElementType).Returns(inventories.AsQueryable().ElementType);
            mocksetInv.As<IQueryable<Inventory>>().Setup(m => m.GetEnumerator()).Returns(inventories.GetEnumerator());
            mocksetInv.As<IAsyncEnumerable<Inventory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Inventory>(inventories.GetEnumerator()));
            mocksetInv.As<IQueryable<Inventory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Inventory>(inventories.AsQueryable().Provider));
            mocksetInv.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mocksetInv.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mocksetInv.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mocksetInv.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new() { CallBase = true };
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<Inventory>()).Returns(mocksetInv.Object);
            mockDbContext.Setup(d => d.Inventories).Returns(mocksetInv.Object);

            var mockedDbTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            var dbconnect = new Mock<System.Data.Common.DbConnection>();
            dbconnect.Setup(g => g.ConnectionString).Returns(It.IsAny<string>());
            mockedDbTransaction.Setup(j => j.Commit());
            var mockexecutionstrategy = new Mock<Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>();
            var databaseFacadeMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(mockDbContext.Object);
            databaseFacadeMock.Setup(x => x.EnsureCreated()).Returns(true);
            databaseFacadeMock.Setup(c => c.CreateExecutionStrategy()).Returns(mockexecutionstrategy.Object);
            mockDbContext.SetupGet(c => c.Database).Returns(databaseFacadeMock.Object);
            mockDbContext.Setup(x => x.Database.BeginTransaction()).Returns(mockedDbTransaction.Object);

            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            IUnitOfWork work = unitofwork.Object;

            IProductService service = new EShopServices.Services.ProductService(work);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);
            var result = await productsController.GetProductInventories(1);
            mockRepository.Verify();
            Assert.IsType<List<Inventory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task PutProductTest()
        {
            Product product = new(1,1,"Pname","B003","I003",1,1,false,5)
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                IsDeleted = false,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>()
            };

            var mockset = new Mock<DbSet<Product>>(MockBehavior.Strict);
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

     

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object); 
            mockDbContext.Setup(r => r.MarkAsModified(It.IsAny<Product>())).Verifiable();
            mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<EShopModels.IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));

            IProductService service = new EShopServices.Services.ProductService(unitofwork.Object);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);

            var httpActionResult =await productsController.PutProduct(1, product);
            mockRepository.Verify();
            Assert.NotNull(httpActionResult);
            Assert.NotNull(httpActionResult);
            Assert.IsType<Product>(((ObjectResult)httpActionResult).Value);
        }

        [Fact()]
        public async Task PutProductTest1()
        {
            Product product = new(1,1,"Pname","B003","I003",1,1,false,5)
            { 
                CreatedDate = DateTime.Now,
                ID = 1,
                IsDeleted = false,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>()
            };

            var mockset = new Mock<DbSet<Product>>(MockBehavior.Strict);
            mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
            mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
            mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
            mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
            mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));
            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockset.Setup(g => g.Attach(It.IsAny<Product>())).Callback((Product cg) => mockset.Object.Attach(cg));//.Returns(product);

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Products).Returns(mockset.Object); 
            mockDbContext.Setup(r => r.MarkAsModified(It.IsAny<Product>())).Verifiable();
            mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var mockedDbTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
            var dbconnect = new Mock<System.Data.Common.DbConnection>();
            dbconnect.Setup(g => g.ConnectionString).Returns(It.IsAny<string>());
            mockedDbTransaction.Setup(j => j.Commit());
            var mockexecutionstrategy = new Mock<Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>();
            var databaseFacadeMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(mockDbContext.Object);
            databaseFacadeMock.Setup(x => x.EnsureCreated()).Returns(true);
            databaseFacadeMock.Setup(c => c.CreateExecutionStrategy()).Returns(mockexecutionstrategy.Object);
            mockDbContext.SetupGet(c => c.Database).Returns(databaseFacadeMock.Object);
            mockDbContext.Setup(x => x.Database.BeginTransaction()).Returns(mockedDbTransaction.Object);
            mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            Mock<EShopModels.Repository.IRepository<Product>> repo = new();
            IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<EShopModels.IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
            unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));

            IProductService service = new EShopServices.Services.ProductService(unitofwork.Object);
            ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);

            var httpActionResult =await productsController.PutProduct(1,product);
            mockRepository.Verify();
            Assert.NotNull(httpActionResult);
            Assert.IsType<Product>(((ObjectResult)httpActionResult).Value);
        }

        [Fact()]
        public async Task PostProductTest()
        { 
                Product product = new(1,1,"Pname","B005","I003",1,1,false,5)
                { 
                    CreatedDate = DateTime.Now,
                    ID = 5,
                    IsDeleted = false,
                    ModifiedDate = DateTime.Now,
                    RowVersion = Array.Empty<byte>() 
                };

                var mockset = new Mock<DbSet<Product>>(MockBehavior.Strict);
                mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
                mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
                mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
                mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
                mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));
                mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
                mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
                mockset.Setup(b => b.AddAsync(It.IsAny<Product>(),It.IsAny<CancellationToken>()))
                .Callback<Product, CancellationToken>((s, token) =>
                {
                    products.Add(s);
                })
                .Returns((Product model,CancellationToken token) => new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<Product>>());

                var ucmockset = new Mock<DbSet<UnitChart>>(MockBehavior.Strict);
                ucmockset.As<IQueryable<UnitChart>>().Setup(m => m.Expression).Returns(unitCharts.AsQueryable().Expression);
                ucmockset.As<IQueryable<UnitChart>>().Setup(m => m.ElementType).Returns(unitCharts.AsQueryable().ElementType);
                ucmockset.As<IQueryable<UnitChart>>().Setup(m => m.GetEnumerator()).Returns(unitCharts.AsQueryable().GetEnumerator());
                ucmockset.As<IAsyncEnumerable<UnitChart>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<UnitChart>(unitCharts.AsQueryable().GetEnumerator()));
                ucmockset.As<IQueryable<UnitChart>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<UnitChart>(unitCharts.AsQueryable().Provider));
                ucmockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => ucmockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
                ucmockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await ucmockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
                ucmockset.Setup(b => b.AddAsync(It.IsAny<UnitChart>(), It.IsAny<CancellationToken>())).Returns((UnitChart model, CancellationToken token) => new ValueTask<Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<UnitChart>>());
 
                Mock<ApplicationDbContext> mockDbContext = new();
                mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
                mockDbContext.Setup(d => d.Products).Returns(mockset.Object); 
                mockDbContext.Setup(d => d.Set<UnitChart>()).Returns(ucmockset.Object);
                mockDbContext.Setup(d => d.UnitCharts).Returns(ucmockset.Object);

                var mockedDbTransaction = new Mock<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction>();
                var dbconnect = new Mock<System.Data.Common.DbConnection>();
                dbconnect.Setup(g => g.ConnectionString).Returns(It.IsAny<string>());
                mockedDbTransaction.Setup(j => j.Commit());
                var mockexecutionstrategy = new Mock<Microsoft.EntityFrameworkCore.Storage.IExecutionStrategy>();
                var databaseFacadeMock = new Mock<Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade>(mockDbContext.Object);
                databaseFacadeMock.Setup(x => x.EnsureCreated()).Returns(true);
                databaseFacadeMock.Setup(c => c.CreateExecutionStrategy()).Returns(mockexecutionstrategy.Object);
                mockDbContext.SetupGet(c => c.Database).Returns(databaseFacadeMock.Object);
                mockDbContext.Setup(x => x.Database.BeginTransaction()).Returns(mockedDbTransaction.Object);
                mockDbContext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

                Mock<EShopModels.Repository.IRepository<Product>> repo = new();
                IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

                MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
                Mock<EShopModels.IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
                unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
                unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
                unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
                unitofwork.Setup(c => c.Repository<Product>().Add(It.IsAny<Product>())).Callback<Product>((s) => products.Add(s)).Returns(product);
                IUnitOfWork work = unitofwork.Object;

                IProductService service = new EShopServices.Services.ProductService(work);
                ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);

                var httpActionResult = await productsController.PutProduct(1,product);
                mockRepository.Verify();
                var actionresult = httpActionResult as CreatedAtRouteResult;

                Assert.NotNull(actionresult);
                Assert.Equal("DefaultApi", actionresult.RouteName);
                Assert.Equal(product.ID, actionresult.RouteValues["id"]);
                Assert.Equal(3, products.Count); 
        }

        [Fact()]
        public async Task DeleteProductTest()
        { 
                var mockset = new Mock<DbSet<Product>>(MockBehavior.Strict);
                mockset.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.AsQueryable().Expression);
                mockset.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.AsQueryable().ElementType);
                mockset.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());
                mockset.As<IAsyncEnumerable<Product>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<Product>(products.GetEnumerator()));
                mockset.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<Product>(products.AsQueryable().Provider));

                mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
                mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
                //mockset.Setup(s => s.Include(It.IsAny<string>())).Returns(mockset.Object);

                Mock<ApplicationDbContext> mockDbContext = new();
                mockDbContext.Setup(d => d.Set<Product>()).Returns(mockset.Object);
                mockDbContext.Setup(d => d.Products).Returns(mockset.Object);
                Mock<EShopModels.Repository.IRepository<Product>> repo = new();

                repo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()
                    , It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>()
                    , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<Product, bool>> ex, Func<IQueryable<Product>, IOrderedQueryable<Product>> value, string vvc) =>
                    {
                        return await mockset.Object.SingleOrDefaultAsync(ex);
                    });

                IProductRepository mockrepo = new ProductRepository(mockDbContext.Object);

                MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
                Mock<EShopModels.IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
                unitofwork.Setup(c => c.ProductRepository).Returns(mockrepo);
                unitofwork.Setup(c => c.Repository<Product>()).Returns(repo.Object);
                unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));

                IProductService service = new EShopServices.Services.ProductService(unitofwork.Object);
                ProductsController productsController = new(service, contextAccessor.Object, mocklogger.Object);

                var httpActionResult = await productsController.DeleteProduct(0);
                mockRepository.Verify();
                var actionresult = httpActionResult as OkObjectResult;

                Assert.NotNull(actionresult);
                Assert.NotNull(actionresult.Value);
                Assert.True(((Product)actionresult.Value).IsDeleted);
           
        }
    }
}