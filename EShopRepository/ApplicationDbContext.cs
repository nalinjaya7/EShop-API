using EShopModels;
using EShopModels.Common;
using EShopRepository.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace EShopRepository
{
    public class ApplicationDbContext : DbContext
    {  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
        }
          
        public virtual DbSet<Product> Products { get; set; } 
        public virtual DbSet<UnitType> UnitTypes { get; set; }
        public virtual DbSet<UnitChart> UnitCharts { get; set; } 
        public virtual DbSet<Inventory> Inventories { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<ProductSubCategory> ProductSubCategories { get; set; }
        public virtual DbSet<Login> Logins { get; set; }
        public virtual DbSet<LoginDetail> LoginDetails { get; set; }  
        public virtual DbSet<EShopUser> Users { get; set; }    
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<ShoppingCartItem> ShoppingCartItems { get; set; }

        public virtual void MarkAsModified(object item)
        {
            Entry(item).State = EntityState.Modified;
        }

        public virtual void MarkAsDeleted(object item)
        {
            Entry(item).State = EntityState.Deleted;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // IConfigurationRoot configurationRoot = new ConfigurationBuilder().SetBasePath(AppDomain.CurrentDomain.BaseDirectory).AddJsonFile("appsettings.json").Build();
                // optionsBuilder.UseSqlServer(configurationRoot.GetConnectionString("DefaultConnection"));
            }

            //optionsBuilder.UseLazyLoadingProxies(useLazyLoadingProxies: false);
            base.OnConfiguring(optionsBuilder);
        }

        public virtual System.Data.Common.DbConnection GetDbConnection()
        {
            return Database.GetDbConnection();
        }

        public virtual IDbContextTransaction GetBeginTransaction()
        {
            return Database.BeginTransaction();
        }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var Entityitems = modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()).Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);
            foreach (var Entityitem in Entityitems)
            {
                Entityitem.DeleteBehavior = DeleteBehavior.Restrict;
            }
             
            modelBuilder.ApplyConfiguration(new InventoryConfiguration()); 
            modelBuilder.ApplyConfiguration(new LoginConfiguration());  
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new ProductCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new ProductSubCategoryConfiguration()); 
            modelBuilder.ApplyConfiguration(new EShopUserConfiguration());  
            modelBuilder.ApplyConfiguration(new UnitChartConfiguration());
            modelBuilder.ApplyConfiguration(new UnitTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ShoppingCartConfiguration());
            modelBuilder.ApplyConfiguration(new ShoppingCartItemConfiguration());

            modelBuilder.Entity<ProductCategory>().HasData(new ProductCategory[] {
                new ProductCategory("Foods"){ID=1}
            });

            modelBuilder.Entity<ProductSubCategory>().HasData(new ProductSubCategory[]
            {
                new ProductSubCategory(1, "Fresh vegetables") { ID = 1 },
                new ProductSubCategory(1, "Seafood") { ID = 13 }
            });

            modelBuilder.Entity<Product>().HasData(new Product[]
            {
                new Product(1, 1, "Asparagus", "FVG001", "FVG001", 0, null, true, 0.00m){ID = 1, ProductImage = System.IO.File.ReadAllBytes("Images/Asparagus.png")},
                new Product(1, 1, "Broccoli", "FVG002", "FVG002", 0, null, true, 0.00m){ID = 2, ProductImage = System.IO.File.ReadAllBytes("Images/Broccoli.png")},
                new Product(1, 1, "Carrots", "FVG003", "FVG003", 0, null, true, 0.00m){ ID = 3, ProductImage = System.IO.File.ReadAllBytes("Images/Carrots.png")},
                new Product(1, 1, "Cauliflower", "FVG004", "FVG004", 0, null, true, 0.00m){ID = 4, ProductImage = System.IO.File.ReadAllBytes("Images/Cauliflower.png")},
                new Product(1, 1, "Celery", "FVG005", "FVG005", 0, null, true, 0.00m){ID = 5, ProductImage = System.IO.File.ReadAllBytes("Images/Celery.png")},
                new Product(1, 1, "Corn", "FVG006", "FVG006", 0, null, true, 0.00m){ID = 6, ProductImage = System.IO.File.ReadAllBytes("Images/Corn.png")},
                new Product(1, 1, "Cucumbers", "FVG007", "FVG007", 0, null, true, 0.00m){ID = 8, ProductImage = System.IO.File.ReadAllBytes("Images/Cucumbers.png")},
                new Product(1, 1, "Lettuce / Greens", "FVG008", "FVG008", 0, null, true, 0.00m){ID = 9, ProductImage = System.IO.File.ReadAllBytes("Images/Greens.png") },
                new Product(1, 1, "Mushrooms", "FVG009", "FVG009", 0, null, true, 0.00m){ID = 10, ProductImage = System.IO.File.ReadAllBytes("Images/Mushrooms.png")},
                new Product(1, 1, "Onions", "FVG010", "FVG010", 0, null, true, 0.00m){ID = 11, ProductImage = System.IO.File.ReadAllBytes("Images/Onions.png")},
                new Product(1, 1, "Peppers", "FVG011", "FVG011", 0, null, true, 0.00m){ID = 12, ProductImage = System.IO.File.ReadAllBytes("Images/Peppers.png") },
                new Product(1, 1, "Potatoes", "FVG012", "FVG012", 0, null, true, 0.00m){ID = 13, ProductImage = System.IO.File.ReadAllBytes("Images/Potatoes.png")},
                new Product(1, 1, "Spinach", "FVG013", "FVG013", 0, null, true, 0.00m){ID = 14, ProductImage = System.IO.File.ReadAllBytes("Images/Spinach.png")},
                new Product(1, 1, "Squash", "FVG014", "FVG014", 0, null, true, 0.00m){ ID = 15, ProductImage = System.IO.File.ReadAllBytes("Images/Squash.png")},
                new Product(1, 1, "Zucchini", "FVG015", "FVG015", 0, null, true, 0.00m){ ID = 16, ProductImage = System.IO.File.ReadAllBytes("Images/Zucchini.png")},
                new Product(1, 1, "Tomatoes", "FVG016", "FVG016", 0, null, true, 0.00m){ID = 17, ProductImage = System.IO.File.ReadAllBytes("Images/Tomatoes.png")},
                new Product(13, 1, "Catfish", "SEA001", "SEA001", 0, null, true, 0.00m){ ID = 201, ProductImage = System.IO.File.ReadAllBytes("Images/Catfish.png")},
                new Product(13, 1, "Crab", "SEA002", "SEA002", 0, null, true, 0.00m){ID = 202, ProductImage = System.IO.File.ReadAllBytes("Images/Crab.png")},
                new Product(13, 1, "Lobster", "SEA003", "SEA003", 0, null, true, 0.00m){ ID = 203, ProductImage = System.IO.File.ReadAllBytes("Images/Lobster.png")},
                new Product(13, 1, "Mussels", "SEA004", "SEA004", 0, null, true, 0.00m){ID = 204, ProductImage = System.IO.File.ReadAllBytes("Images/Mussels.png")},
                new Product(13, 1, "Oysters", "SEA005", "SEA005", 0, null, true, 0.00m){ ID = 205, ProductImage = System.IO.File.ReadAllBytes("Images/Oysters.png")},
                new Product(13, 1, "Salmon", "SEA006", "SEA006", 0, null, true, 0.00m){ID = 206, ProductImage = System.IO.File.ReadAllBytes("Images/Salmon.png") },
                new Product(13, 1, "Shrimp", "SEA007", "SEA007", 0, null, true, 0.00m){ID = 207, ProductImage = System.IO.File.ReadAllBytes("Images/Shrimp.png")},
                new Product(13, 1, "Tilapia", "SEA008", "SEA008", 0, null, true, 0.00m){ID = 208, ProductImage = System.IO.File.ReadAllBytes("Images/Tilapia.png")},
                new Product(13, 1, "Tuna", "SEA009", "SEA009", 0, null, true, 0.00m){ ID = 209, ProductImage = System.IO.File.ReadAllBytes("Images/Tuna.png")}
            });

            modelBuilder.Entity<UnitType>().HasData(new UnitType[]
            {
                new UnitType("UTY0000000007", "KG", false){ID=7},
                new UnitType("UTY0000000004", "PACK", false){ID=4},
                new UnitType("UTY0000000002", "BOX", false){ID=2},
                new UnitType("UTY0000000001", "EA", true){ID=1}
            });

            modelBuilder.Entity<UnitChart>().HasData(new UnitChart[]
            {
                new UnitChart(2, 1, 15.00m, "Box") { ID = 2 },//, UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 1, 150.00m, "Packet") { ID = 3 }, //UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 1, 1.00m, "Kilogram") { ID = 315 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(4, 2, 30.00m, "Packet") { ID = 4 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 2, 5.00m, "Kilogram") { ID = 354 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 2, 5.00m, "BOX") { ID = 355 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(7, 3, 1.00m, "Kilogram") { ID = 5 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 4, 10.00m, "XLBox") { ID = 6 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(1, 4, 1.00m, "Each") { ID = 376 },// UnitType = new UnitType("UTY0000000001", "EA", true) },
                new UnitChart(4, 4, 20.00m, "Packet") { ID = 377 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(4, 5, 20.00m, "Packet") { ID = 7 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(2, 5, 5.00m, "XLBox") { ID = 8 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 6, 100.00m, "Packet") { ID = 9 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(1, 6, 1.00m, "Each") { ID = 411 },// UnitType = new UnitType("UTY0000000001", "EA", true) },
                new UnitChart(2, 6, 50.00m, "XLBox") { ID = 412 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(2, 8, 10.00m, "XLBox") { ID = 10 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 8, 100.00m, "Packet") { ID = 11 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 8, 1.00m, "Kilogram") { ID = 427 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(7, 9, 1.00m, "Kilogram") { ID = 12 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 9, 9.00m, "XLBox") { ID = 13 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 9, 20.00m, "Packet") { ID = 539 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 9, 1.00m, "Kilogram") { ID = 540 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(7, 11, 1.00m, "Kilogram") { ID = 14 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 11, 122.00m, "XLBox") { ID = 556 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(2, 12, 4.00m, "XLBox") { ID = 15 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 12, 10.00m, "Packet") { ID = 16 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 12, 1.00m, "Kilogram") { ID = 581 }, //UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(7, 13, 1.00m, "Kilogram") { ID = 17 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(4, 13, 1.00m, "Packet") { ID = 592 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(2, 14, 5.00m, "XXLBox") { ID = 18 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 14, 10.00m, "Packet") { ID = 19 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 14, 1.00m, "Kilogram") { ID = 627 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 15, 10.00m, "XLBox") { ID = 20 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 15, 45.00m, "Packet") { ID = 21 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 15, 1.00m, "Kilogram") { ID = 631 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 16, 7.00m, "XXLBox") { ID = 22 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(2, 17, 1.00m, "XXLBox") { ID = 23 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(7, 17, 5.00m, "Kilogram") { ID = 648 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 201, 5.00m, "XLBox") { ID = 219 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(1, 201, 1.00m, "Each") { ID = 374 },// UnitType = new UnitType("UTY0000000001", "EA", true) },
                new UnitChart(4, 201, 100.00m, "Packet") { ID = 375 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 202, 1.00m, "Kilogram") { ID = 220 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(1, 202, 1.00m, "Each") { ID = 420 },// UnitType = new UnitType("UTY0000000001", "EA", true) },
                new UnitChart(2, 202, 20.00m, "XLBox") { ID = 421 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 202, 100.00m, "Packet") { ID = 422 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(2, 202, 20.00m, "XLBox") { ID = 222 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(1, 202, 1.00m, "Each") { ID = 519 },// UnitType = new UnitType("UTY0000000001", "EA", true) },
                new UnitChart(4, 202, 100.00m, "Packet") { ID = 520 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(2, 204, 20.00m, "XLBox") { ID = 223 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 204, 50.00m, "Packet") { ID = 541 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 204, 1.00m, "Kilogram") { ID = 542 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(2, 205, 30.00m, "XLBox") { ID = 224 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 205, 50.00m, "Packet") { ID = 559 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(2, 206, 50.00m, "XLBox") { ID = 225 }, //UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(1, 206, 1.00m, "Each") { ID = 610 },// UnitType = new UnitType("UTY0000000001", "EA", true) },
                new UnitChart(2, 207, 44.00m, "XLBox") { ID = 226 },// UnitType = new UnitType("UTY0000000002", "BOX", false) },
                new UnitChart(4, 207, 50.00m, "Packet") { ID = 619 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 207, 1.00m, "Kilogram") { ID = 620 }, //UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(7, 208, 1.00m, "Kilogram") { ID = 227 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(4, 208, 5.00m, "Packet") { ID = 643 },// UnitType = new UnitType("UTY0000000004", "PACK", false) },
                new UnitChart(7, 209, 1.00m, "Kilogram") { ID = 228 },// UnitType = new UnitType("UTY0000000007", "KG", false) },
                new UnitChart(4, 209, 2.00m, "Packet") { ID = 649 },// UnitType = new UnitType("UTY0000000004", "PACK", false) }
            });

            modelBuilder.Entity<Inventory>().HasData(new Inventory[]
            {
                new Inventory("STK0000000959", 2, 4, 500.00m, 2128, 51.02m, 49.53m, 0){ ID=1},
                new Inventory("STK0000000960", 2, 354, 573.00m, 2128, 102.26m, 101.24m, 0){ ID=2},
                new Inventory("STK0000000961", 2, 355, 2573.00m, 2128, 109.26m, 107.24m, 0){ ID=3},
                new Inventory("STK0000000991", 3, 5, 1500.00m, 2128, 231.02m, 201.53m, 0){ ID=4},
                new Inventory("STK0000000998", 4, 6, 1500.00m, 2128, 231.02m, 201.53m, 0){ ID=5},
                new Inventory("STK0000000999", 4, 376, 160.00m, 2128, 201.02m, 191.53m, 0){ ID=6},
                new Inventory("STK0000001000", 4, 377, 110.00m, 2128, 31.02m, 21.53m, 0){ ID=7},
                new Inventory("STK0000001004", 5, 7, 710.00m, 2128, 731.02m, 621.53m, 0){ ID=8},
                new Inventory("STK0000001005", 5, 8, 910.00m, 2128, 331.02m, 321.53m, 0){ ID=9},
                new Inventory("STK0000001055", 6, 9, 400.00m, 2128, 221.02m, 191.53m, 0){ ID=10},
                new Inventory("STK0000001056", 6, 411, 260.00m, 2128, 981.02m, 891.53m, 0){ ID=11},
                new Inventory("STK0000001057", 6, 412, 190.00m, 2128, 901.02m, 791.53m, 0){ ID=12},
                new Inventory("STK0000001080", 8, 10, 54.00m, 2128, 871.50m, 791.53m, 0){ ID=13},
                new Inventory("STK0000001081", 8, 11, 543, 2128, 432.50m, 761.53m, 0){ ID=14},
                new Inventory("STK0000001082", 8, 427, 76, 2128, 961.02m, 891.53m, 0){ ID=15},
                new Inventory("STK0000001213", 9, 12, 890.90m, 2128, 91.92m, 91.53m, 0){ ID=16},
                new Inventory("STK0000001252", 10, 13, 900.00m, 2128, 81.92m, 41.53m, 0){ ID=17},
                new Inventory("STK0000001253", 10, 539, 390.90m, 2128, 231.92m, 191.53m, 0){ ID=18},
                new Inventory("STK0000001254", 10, 540, 290.90m, 2128, 671.92m, 691.53m, 0){ ID=19},
                new Inventory("STK0000001279", 11, 14, 90.90m, 2128, 271.92m, 291.53m, 0){ ID=20},
                new Inventory("STK0000001280", 11, 556, 890.90m, 2128, 61.92m, 61.53m, 0){ ID=21},
                new Inventory("STK0000001320", 12, 15, 1090.90m, 2128, 691.92m, 611.53m, 0){ ID=22},
                new Inventory("STK0000001321", 12, 16, 900.90m, 2128, 361.92m, 261.53m, 0){ ID=23},
                new Inventory("STK0000001322", 12, 581, 400.90m, 2128, 161.92m, 61.53m, 0){ ID=24},
                new Inventory("STK0000001346", 13, 17, 900.90m, 2128, 11.92m, 1.53m, 0){ ID=25},
                new Inventory("STK0000001347", 13, 592, 2400.90m, 2128, 181.92m, 161.53m, 0){ ID=26},
                new Inventory("STK0000001412", 14, 18, 2400.90m, 2128, 181.92m, 161.53m, 0){ ID=27},
                new Inventory("STK0000001413", 14, 19, 3400.90m, 2128, 581.92m, 461.53m, 0){ ID=28},
                new Inventory("STK0000001414", 14, 627, 400.90m, 2128, 101.92m, 101.53m, 0){ ID=29},
                new Inventory("STK0000001420", 15, 20, 7400.90m, 2128, 401.92m, 301.53m, 0){ ID=30},
                new Inventory("STK0000001421", 15, 21, 2400.90m, 2128, 801.92m, 701.53m, 0){ ID=31},
                new Inventory("STK0000001422", 15, 631, 400.90m, 2128, 11.92m, 10.53m, 0){ ID=32},
                new Inventory("STK0000001367", 16, 22, 300.90m, 2128, 561.92m, 410.53m, 0){ ID=33},
                new Inventory("STK0000001453", 17, 23, 9300.90m, 2128, 501.92m, 410.53m, 0){ ID=34},
                new Inventory("STK0000001454", 17, 648, 600.90m, 2128, 871.92m, 7410.53m, 0){ ID=35},
                new Inventory("STK0000000995", 201, 219, 500.00m, 2128, 71.92m, 10.53m, 0){ ID=36},
                new Inventory("STK0000000996", 201, 374, 700.00m, 2128, 81.92m, 10.53m, 0){ ID=37},
                new Inventory("STK0000000997", 201, 375, 900.00m, 2128, 901.92m, 410.53m, 0){ ID=38},
                new Inventory("STK0000001070", 202, 220, 600.00m, 2128, 541.92m, 410.53m, 0){ ID=39},
                new Inventory("STK0000001071", 202, 420, 500.00m, 2128, 701.92m, 410.53m, 0){ ID=40},
                new Inventory("STK0000001072", 202, 421, 400.00m, 2128, 801.92m, 410.53m, 0){ ID=41},
                new Inventory("STK0000001073", 202, 422, 200.00m, 2128, 91.92m, 40.53m, 0){ ID=42},
                new Inventory("STK0000001220", 203, 222, 600.00m, 2128, 591.92m, 490.53m, 0){ ID=43},
                new Inventory("STK0000001221", 203, 519, 800.00m, 2128, 901.92m, 810.53m, 0){ ID=44},
                new Inventory("STK0000001222", 203, 520, 500.00m, 2128, 941.92m, 810.53m, 0){ ID=45},
                new Inventory("STK0000001255", 204, 223, 900.00m, 2128, 491.92m, 440.53m, 0){ ID=46},
                new Inventory("STK0000001256", 204, 541, 9000.00m, 2128, 291.92m, 190.53m, 0){ ID=47},
                new Inventory("STK0000001257", 204, 542, 700.00m, 2128, 391.92m, 40.53m, 0){ ID=48},
                new Inventory("STK0000001285", 205, 224, 900.00m, 2128, 91.92m, 90.53m, 0){ ID=49},
                new Inventory("STK0000001286", 205, 559, 300.00m, 2128, 91.92m, 47.53m, 0){ ID=50},
                new Inventory("STK0000001381", 206, 225, 700.00m, 2128, 123.92m, 57.53m, 0){ ID=51},
                new Inventory("STK0000001382", 206, 610, 400.00m, 2128, 41.92m, 27.53m, 0){ ID=52},
                new Inventory("STK0000001396", 207, 226, 900.00m, 2128, 491.92m, 427.53m, 0){ ID=53},
                new Inventory("STK0000001397", 207, 619, 490.00m, 2128, 341.92m, 227.53m, 0){ ID=54},
                new Inventory("STK0000001398", 207, 620, 4300.00m, 2128, 441.92m, 427.53m, 0){ ID=55},
                new Inventory("STK0000001443", 208, 227, 400.00m, 2128, 141.92m, 47.53m, 0){ ID=56},
                new Inventory("STK0000001444", 208, 643, 7300.00m, 2128, 741.92m, 427.53m, 0){ ID=57},
                new Inventory("STK0000001458", 209, 228, 300.00m, 2128, 41.92m, 27.53m, 0){ ID=58},
                new Inventory("STK0000001459", 209, 649, 90.00m, 2128, 741.92m, 97.53m, 0){ ID=59}
            });

            modelBuilder.Entity<EShopUser>().HasData(new EShopUser[]
            {
                new EShopUser(1, "dit7m77@Keels.com", "test", "test", "test", "dit7m77@Keels.com", true, Guid.NewGuid(), "123Pp[]", Roles.User),
                new EShopUser(2, "nalinmyid@Keels.com", "test", "test", "test", "nalinmyid@Keels.com", true, Guid.NewGuid(), "123Pp[]", Roles.Admin)
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
