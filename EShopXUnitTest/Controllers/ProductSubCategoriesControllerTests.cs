using Castle.Core.Logging;
using EShopApi.Controllers;
using EShopModels;
using EShopModels.Common;
using EShopModels.Repository;
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
    public class ProductSubCategoriesControllerTests
    {
        private readonly List<ProductSubCategory> subCategories=new(); 
        private readonly Mock<IHttpContextAccessor> contextAccessor;
        private readonly Mock<Microsoft.AspNetCore.Http.HttpContext> mockHttpContext;
        private readonly Mock<ILogger<ProductSubCategoriesController>> mocklogger;
        public ProductSubCategoriesControllerTests()
        { 
            subCategories.Add(new ProductSubCategory(1,"vegitable")
            {
                CreatedDate = DateTime.Now,
                ID = 1,
                ModifiedDate = DateTime.Now,
                RowVersion = Array.Empty<byte>() 
            });

            subCategories.Add(new ProductSubCategory(1, "Rice")
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
            mocklogger = new Mock<ILogger<ProductSubCategoriesController>>();
        }

        [Fact()]
        public async Task GetProductSubCategoriesTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockset = new Mock<DbSet<ProductSubCategory>>(MockBehavior.Strict);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.Set<ProductSubCategory>()).Returns(mockset.Object);
            mockDbContext.Setup(d => d.ProductSubCategories).Returns(mockset.Object);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mockDbContext.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();
            urepo.Setup(c => c.GetAllAsync()).Returns(async () =>
            {
                return await mockset.Object.ToListAsync();
            });

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var result = await productSubCategoriesController.GetProductSubCategories();
            mockRepository.Verify();
            Assert.IsType<List<EShopModels.ProductSubCategory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductSubCategoriesTest1()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockset = new Mock<DbSet<ProductSubCategory>>(MockBehavior.Strict);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.ProductSubCategories).Returns(mockset.Object);
            mockDbContext.Setup(d => d.Set<ProductSubCategory>()).Returns(mockset.Object);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mockDbContext.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();
            urepo.Setup(c => c.GetAllAsync()).Returns(async () =>
            {
                return await mockset.Object.ToListAsync();
            });

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var result = await productSubCategoriesController.GetProductSubCategories(1);
            mockRepository.Verify();
            Assert.IsType<List<ProductSubCategory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetSortedProductSubCategoriesTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockset = new Mock<DbSet<ProductSubCategory>>(MockBehavior.Strict);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            //mockset.Setup(b => b.Include("ProductCategory")).Returns(mockset.Object);

            Mock<ApplicationDbContext> mockDbContext = new();
            mockDbContext.Setup(d => d.ProductSubCategories).Returns(mockset.Object);
            mockDbContext.Setup(f => f.Set<ProductSubCategory>()).Returns(mockset.Object);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mockDbContext.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object,mocklogger.Object);

            var result = await productSubCategoriesController.GetProductSubCategories(1, 1);
            mockRepository.Verify();
            Assert.IsType<List<ProductSubCategory>>(((OkObjectResult)result).Value);
        }

        [Fact()]
        public async Task GetProductSubCategoriesTest2()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockdbset = new Mock<DbSet<ProductSubCategory>>();
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mock = new();
            mock.Setup(h => h.ProductSubCategories).Returns(mockdbset.Object);
            mock.Setup(f => f.Set<ProductSubCategory>()).Returns(mockdbset.Object);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mock.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();
            urepo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>>>()
             , It.IsAny<Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>>>()
             , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>> ex, Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>> value, string vvc) =>
             {
                 return await mockdbset.Object.SingleOrDefaultAsync(ex);
             });

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice,contextAccessor.Object, mocklogger.Object);

            // Act
            var httpActionResult = await productSubCategoriesController.GetProductSubCategory(1);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            // Assert
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.Equal(1, ((ProductSubCategory)actionresult.Value).ID);
        }

        [Fact()]
        public async Task GetSubCategoriesByCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockdbset = new Mock<DbSet<ProductSubCategory>>();
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mock = new();
            mock.Setup(h => h.ProductSubCategories).Returns(mockdbset.Object);
            mock.Setup(f => f.Set<ProductSubCategory>()).Returns(mockdbset.Object);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mock.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();
            urepo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>>>()
             , It.IsAny<Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>>>()
             , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>> ex, Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>> value, string vvc) =>
             {
                 return await mockdbset.Object.SingleOrDefaultAsync(ex);
             });

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            // Act
            var httpActionResult = await productSubCategoriesController.GetSubCategoriesByCategory(1);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            // Assert
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.Equal(2, ((List<ProductSubCategory>)actionresult.Value).Count);
        }

        [Fact()]
        public async Task GetProductSubCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockdbset = new Mock<DbSet<ProductSubCategory>>();
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));

            Mock<ApplicationDbContext> mock = new();
            mock.Setup(h => h.ProductSubCategories).Returns(mockdbset.Object);
            mock.Setup(f => f.Set<ProductSubCategory>()).Returns(mockdbset.Object);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mock.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();
            urepo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>>>()
            , It.IsAny<Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>>>()
            , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>> ex, Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>> value, string vvc) =>
            {
                return await mockdbset.Object.SingleOrDefaultAsync(ex);
            });

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object,mocklogger.Object);

            // Act
            var httpActionResult = await productSubCategoriesController.GetProductSubCategory(1);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            // Assert
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.Equal(1, ((ProductSubCategory)actionresult.Value).ID);
        }

        [Fact()]
        public async Task PutProductSubCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            ProductSubCategory productSubCategory = new(1,"Rice")
            {
                CreatedDate = DateTime.Now,
                ID = 2,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
            };

            var mockdbset = new Mock<DbSet<ProductSubCategory>>() { CallBase = false };
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(g => g.Attach(It.IsAny<ProductSubCategory>())).Callback((ProductSubCategory cg) => mockdbset.Object.Attach(cg));//.Returns(productSubCategory);

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<ProductSubCategory>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.ProductSubCategories).Returns(mockdbset.Object);
            mockdbcontext.Setup(r => r.MarkAsModified(It.IsAny<ProductSubCategory>())).Verifiable();
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mockdbcontext.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await productSubCategoriesController.PutProductSubCategory(1, productSubCategory);
            var actionresult = httpActionResult as ObjectResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult); 
            Assert.IsType<ProductSubCategory>(actionresult.Value);
        }

        [Fact()]
        public async Task PostProductSubCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            ProductSubCategory productSubCategory = new(1,"test")
            {
                CreatedDate = DateTime.Now,
                ID = 3,
                ModifiedDate = DateTime.Now, 
                RowVersion = Array.Empty<byte>() 
            };

            var mockdbset = new Mock<DbSet<ProductSubCategory>>() { CallBase = false };
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(f => f.Add(It.IsAny<ProductSubCategory>())).Callback<ProductSubCategory>((entity) => subCategories.Add(productSubCategory));

            Mock<ApplicationDbContext> mockdbcontext = new() { CallBase = false };
            mockdbcontext.Setup(m => m.Set<ProductSubCategory>()).Returns(mockdbset.Object);
            mockdbcontext.Setup(e => e.ProductSubCategories).Returns(mockdbset.Object);
            mockdbcontext.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(mockdbcontext.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();
            urepo.Setup(g => g.Add(It.IsAny<ProductSubCategory>())).Callback<ProductSubCategory>((loc) => subCategories.Add(loc)).Returns(productSubCategory);

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await productSubCategoriesController.PostProductSubCategory(productSubCategory);
            var actionresult = httpActionResult as CreatedAtRouteResult;

            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.Equal("DefaultApi", actionresult.RouteName);
            Assert.Equal(productSubCategory.ID, actionresult.RouteValues["id"]);
            Assert.Equal(3, subCategories.Count);
        }

        [Fact()]
        public async Task DeleteProductSubCategoryTest()
        {
            MockRepository mockRepository = new(MockBehavior.Strict) { DefaultValue = DefaultValue.Mock };
            Mock<IUnitOfWork> unitofwork = mockRepository.Create<EShopModels.IUnitOfWork>();

            var mockdbset = new Mock<DbSet<ProductSubCategory>>();
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Expression).Returns(subCategories.AsQueryable().Expression);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.ElementType).Returns(subCategories.AsQueryable().ElementType);
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.GetEnumerator()).Returns(subCategories.GetEnumerator());
            mockdbset.As<IAsyncEnumerable<ProductSubCategory>>().Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>())).Returns(new TestDbAsyncEnumerator<ProductSubCategory>(subCategories.GetEnumerator()));
            mockdbset.As<IQueryable<ProductSubCategory>>().Setup(m => m.Provider).Returns(new TestDbAsyncQueryProvider<ProductSubCategory>(subCategories.AsQueryable().Provider));

            mockdbset.Setup(r => r.Find(It.IsAny<object[]>())).Returns<object[]>(ids => mockdbset.Object.FirstOrDefault(a => a.ID == (int)ids[0]));
            mockdbset.Setup(m => m.FindAsync(It.IsAny<object[]>())).Returns<object[]>(async (d) => await mockdbset.Object.FirstOrDefaultAsync(a => a.ID == (int)d[0]));
            mockdbset.Setup(u => u.Attach(It.IsAny<ProductSubCategory>())).Verifiable();
            mockdbset.Setup(m => m.Remove(It.IsAny<ProductSubCategory>())).Callback<ProductSubCategory>((entity) => subCategories.Remove(entity));

            Mock<ApplicationDbContext> dbmock = new() { CallBase = true };
            dbmock.Setup(u => u.Set<ProductSubCategory>()).Returns(mockdbset.Object);
            dbmock.Setup(n => n.ProductSubCategories).Returns(mockdbset.Object);
            dbmock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            IProductSubCategoryRepository umockrepo = new ProductSubCategoryRepository(dbmock.Object);
            Mock<EShopModels.Repository.IRepository<ProductSubCategory>> urepo = new();
            urepo.Setup(c => c.GetSingleAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>>>()
            , It.IsAny<Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>>>()
            , It.IsAny<string>())).Returns(async (System.Linq.Expressions.Expression<Func<ProductSubCategory, bool>> ex, Func<IQueryable<ProductSubCategory>, IOrderedQueryable<ProductSubCategory>> value, string vvc) =>
            {
                return await mockdbset.Object.SingleOrDefaultAsync(ex);
            });

            unitofwork.Setup(c => c.ProductSubCategoryRepository).Returns(umockrepo);
            unitofwork.Setup(c => c.Repository<ProductSubCategory>()).Returns(urepo.Object);
            unitofwork.Setup(c => c.CompleteAsync()).Returns(Task.FromResult<int>(1));
            IUnitOfWork work = unitofwork.Object;

            EShopServices.Services.ProductSubCategoryService mockservice = new(work);
            ProductSubCategoriesController productSubCategoriesController = new(mockservice, contextAccessor.Object, mocklogger.Object);

            var httpActionResult = await productSubCategoriesController.DeleteProductSubCategory(1);
            var actionresult = httpActionResult as OkObjectResult;
            mockRepository.Verify();
            Assert.NotNull(actionresult);
            Assert.NotNull(actionresult.Value);
            Assert.True(((ProductSubCategory)actionresult.Value).IsDeleted);
        }
    }
}