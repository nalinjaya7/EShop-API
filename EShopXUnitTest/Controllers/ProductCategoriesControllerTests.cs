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
    public class ProductCategoriesControllerTests
    {
        private readonly List<ProductCategory> productCategories=new(); 
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpContext> mockHttpContext;
        private readonly Mock<ILogger<ProductCategoriesController>> mockLogger;
        public ProductCategoriesControllerTests()
        { 

            productCategories.Add(new ProductCategory("Food")
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
            });
            productCategories.Add(new ProductCategory("hardware")
            {
                CreatedDate = DateTime.Now,
                ID = 2,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
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
            mockLogger = new Mock<ILogger<ProductCategoriesController>>();
        }

        [Fact()]
        public async Task GetProductCategoriesTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<ProductCategory>>(MockBehavior.Strict);
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.Expression).Returns(productCategories.AsQueryable().Expression);
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.ElementType).Returns(productCategories.AsQueryable().ElementType);
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.GetEnumerator()).Returns(productCategories.GetEnumerator());
            mockset.As<IAsyncEnumerable<ProductCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductCategory>(productCategories.GetEnumerator()));
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductCategory>(productCategories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.ProductCategories).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<ProductCategory>()).Returns(mockset.Object);

            IProductCategoryRepository umockrepo = new ProductCategoryRepository(mockDbContext.Object);
            Mock<EShopModels.Repository.IRepository<ProductCategory>> urepo = new();

            unitofwork.Setup(c => c.ProductCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IProductCategoryService mockservice = new EShopServices.Services.ProductCategoryService(work);
            ProductCategoriesController productCategoriesController = new(mockservice, contextAccessor.Object,mockLogger.Object);
            var result = await productCategoriesController.GetProductCategories(1);
            mockRepository.Verify();
            Assert.IsType<List<ProductCategory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductCategoriesTest1()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            var mockset = new Mock<DbSet<ProductCategory>>(MockBehavior.Strict);
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.Expression).Returns(productCategories.AsQueryable().Expression);
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.ElementType).Returns(productCategories.AsQueryable().ElementType);
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.GetEnumerator()).Returns(productCategories.GetEnumerator());
            mockset.As<IAsyncEnumerable<ProductCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductCategory>(productCategories.GetEnumerator()));
            mockset.As<IQueryable<ProductCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductCategory>(productCategories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Set<ProductCategory>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.ProductCategories).Returns(mockset.Object);

            IProductCategoryRepository umockrepo = new ProductCategoryRepository(mockDbContext.Object);
            Mock<EShopModels.Repository.IRepository<ProductCategory>> urepo = new();

            unitofwork.Setup(c => c.ProductCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IProductCategoryService mockservice = new EShopServices.Services.ProductCategoryService(work);
            ProductCategoriesController productCategoriesController = new(mockservice, contextAccessor.Object, mockLogger.Object);
            var result = await productCategoriesController.GetProductCategories(1);
            mockRepository.Verify();
            Assert.IsType<List<EShopModels.ProductCategory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            int ProductCategoryID = 1;

            var mockdbset = new Mock<DbSet<ProductCategory>>();
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Expression).Returns(productCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.ElementType).Returns(productCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.GetEnumerator()).Returns(productCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductCategory>(productCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductCategory>(productCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            Mock<ApplicationDbContext> mock = new();
            mock.Setup(h => h.ProductCategories).Returns(mockdbset.Object);
            mock.Setup(f => f.Set<ProductCategory>()).Returns(mockdbset.Object);

            IProductCategoryRepository umockrepo = new ProductCategoryRepository(mock.Object);
            Mock<EShopModels.Repository.IRepository<ProductCategory>> urepo = new();
            urepo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductCategory, bool>>>()
              , It.IsAny<Func<IQueryable<ProductCategory>, IOrderedQueryable<ProductCategory>>>()
              , It.IsAny<string>())).Returns(async
              (System.Linq.Expressions.Expression<Func<ProductCategory, bool>> ex, Func<IQueryable<ProductCategory>, IOrderedQueryable<ProductCategory>> value, string vvc) =>
              {
                  return await mockdbset.Object.SingleOrDefaultAsync(ex);
              });

            unitofwork.Setup(c => c.ProductCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IProductCategoryService mockservice = new EShopServices.Services.ProductCategoryService(work);
            ProductCategoriesController productCategoriesController = new(mockservice, contextAccessor.Object, mockLogger.Object);
            // Act
            var httpActionResult = await productCategoriesController.GetProductCategory(ProductCategoryID);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            // Assert
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.Equal(((ProductCategory)actionresult.Value).ID, ProductCategoryID);
        }

        [Fact()]
        public async Task PutProductCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            ProductCategory productCategory = new("hardware")
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
            };

            var mockdbset = new Mock<DbSet<ProductCategory>>() { CallBase = false };
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Expression).Returns(productCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.ElementType).Returns(productCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.GetEnumerator()).Returns(productCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductCategory>(productCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductCategory>(productCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(g => g.Attach(It.IsAny<ProductCategory>())).Callback((ProductCategory cg) => mockdbset.Object.Attach(cg));//.Returns(productCategory);

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<ProductCategory>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.ProductCategories).Returns(mockdbset.Object);
            mockdbcontext.Setup(r => r.MarkAsModified(It.IsAny<ProductCategory>())).Verifiable();
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IProductCategoryRepository umockrepo = new ProductCategoryRepository(mockdbcontext.Object);
            Mock<EShopModels.Repository.IRepository<ProductCategory>> urepo = new();

            unitofwork.Setup(c => c.ProductCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IProductCategoryService mockservice = new EShopServices.Services.ProductCategoryService(work);
            ProductCategoriesController productCategoriesController = new(mockservice, contextAccessor.Object,mockLogger.Object);
            var httpActionResult = await productCategoriesController.PutProductCategory(1, productCategory);
            var actionresult = httpActionResult as ObjectResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult); 
            Assert.IsType<ProductCategory>(actionresult.Value);
        }

        [Fact()]
        public async Task PostProductCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            ProductCategory productCategory = new("test")
            {
                CreatedDate = DateTime.Now,
                ID = 3,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
            };

            var mockdbset = new Mock<DbSet<ProductCategory>>() { CallBase = false };
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Expression).Returns(productCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.ElementType).Returns(productCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.GetEnumerator()).Returns(productCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductCategory>(productCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductCategory>(productCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(f => f.Add(It.IsAny<ProductCategory>())).Callback<ProductCategory>((entity) => productCategories.Add(entity));

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<ProductCategory>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.ProductCategories).Returns(mockdbset.Object);
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IProductCategoryRepository umockrepo = new ProductCategoryRepository(mockdbcontext.Object);
            Mock<EShopModels.Repository.IRepository<ProductCategory>> urepo = new();
            urepo.Setup(g => g.Add(It.IsAny<ProductCategory>())).Callback<ProductCategory>((loc) => productCategories.Add(loc)).Returns(productCategory);

            unitofwork.Setup(c => c.ProductCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IProductCategoryService mockservice = new EShopServices.Services.ProductCategoryService(work);
            ProductCategoriesController productCategoriesController = new(mockservice, contextAccessor.Object, mockLogger.Object);

            var httpActionResult = await productCategoriesController.PostProductCategory(productCategory);
            var actionresult = httpActionResult as CreatedAtRouteResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.Equal("DefaultApi", actionresult.RouteName);
            Assert.Equal(productCategory.ID, actionresult.RouteValues["id"]);
            Assert.Equal(3, productCategories.Count);
        }

        [Fact()]
        public async Task DeleteProductCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();
            int ProductCategoryID = 1;

            var mockdbset = new Mock<DbSet<ProductCategory>>();
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Expression).Returns(productCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.ElementType).Returns(productCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.GetEnumerator()).Returns(productCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductCategory>(productCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductCategory>(productCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            mockdbset.Setup(u => u.Attach(It.IsAny<ProductCategory>())).Verifiable();
            mockdbset.Setup(m => m.Remove(It.IsAny<ProductCategory>())).Callback<ProductCategory>((entity) => productCategories.Remove(entity));

            Mock<ApplicationDbContext> dbmock = new() { CallBase = true };
            dbmock.Setup(u => u.Set<ProductCategory>()).Returns(mockdbset.Object);
            dbmock.Setup(n => n.ProductCategories).Returns(mockdbset.Object);
            dbmock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IProductCategoryRepository umockrepo = new ProductCategoryRepository(dbmock.Object);
            Mock<EShopModels.Repository.IRepository<ProductCategory>> urepo = new();
            urepo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductCategory, bool>>>()
            , It.IsAny<Func<IQueryable<ProductCategory>, IOrderedQueryable<ProductCategory>>>()
            , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<ProductCategory, bool>> ex, Func<IQueryable<ProductCategory>, IOrderedQueryable<ProductCategory>> value, string vvc) =>
            {
                return await mockdbset.Object.FirstOrDefaultAsync(ex);
            });

            unitofwork.Setup(c => c.ProductCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            IProductCategoryService mockservice = new EShopServices.Services.ProductCategoryService(work);
            ProductCategoriesController productCategoriesController = new(mockservice, contextAccessor.Object, mockLogger.Object);

            var httpActionResult = await productCategoriesController.DeleteProductCategory(ProductCategoryID);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();

            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.True(((ProductCategory)actionresult.Value).IsDeleted);
        }
    }
}