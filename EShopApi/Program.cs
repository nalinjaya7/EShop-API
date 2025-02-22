using EShopApi.Filters;
using EShopModels;
using EShopModels.Common;
using EShopModels.Services;
using EShopRepository;
using EShopServices.Services;
using EShopServices.ServicesAddNewShoppingCartItemAsync;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
 
var services = builder.Services;
services.AddMvc(options =>
{
   // options.OutputFormatters.Insert(0,Microsoft.AspNetCore.Mvc.Formatters.xml)
});
services.AddControllersWithViews();
services.AddDbContextPool<ApplicationDbContext>(o => o.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]));
var SecretKey = Encoding.ASCII.GetBytes("Jagath98989765123");
services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(token =>
{
    token.RequireHttpsMetadata = false;
    token.SaveToken = true;
    token.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        //Same Secret key will be used while creating the token
        IssuerSigningKey = new SymmetricSecurityKey(SecretKey),
        //Usually, this is your application base URL
        ValidIssuer = "https://localhost:20731/",
        //Here, we are creating and using JWT within the same application.
        //In this case, base URL is fine.
        //If the JWT is created using a web service, then this would be the consumer URL.
        ValidAudience = "https://localhost:44392/",
        RequireExpirationTime = true,
        ClockSkew = TimeSpan.Zero
    };
    token.Events = new JwtBearerEvents
    {   
        //OnTokenValidated = context =>
        //{
        //    var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
        //    var userId = int.Parse(context.Principal.Identity.Name);
        //    var user = userService.GetById(userId);
        //    if (user == null)
        //    {
        //        // return unauthorized if user no longer exists
        //        context.Fail("Unauthorized");
        //    }
        //    return Task.CompletedTask;
        //}
        OnAuthenticationFailed = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            //message += "From OnAuthenticationFailed \n";
            // message += FlattenException(ctx.Exception);
            ctx.Response.Redirect("/Errors");
            //  return Task.FromResult(0);
            return System.Threading.Tasks.Task.CompletedTask;
        }
        //OnChallenge = ctx =>
        //{
        //    message += "FROM OnChallenge : \n";
        //    ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //    ctx.Response.ContentType = "text/plain";
        //    ctx.Response.Redirect("/Errors");
        //    return Task.FromResult(0);
        //    //return ctx.Response.WriteAsync(message);
        //    // ctx.Response.Redirect(ctx.ErrorUri);
        //},
        //OnMessageReceived = ctx =>
        //{
        //    message += "From OnMessageReceived \n";
        //    ctx.Request.Headers.TryGetValue("Authorization", out var BearerToken);
        //    if (BearerToken.Count == 0)
        //    {
        //        BearerToken = "no Bearer token sent\n";
        //    }
        //    message += "Authorization Header sent:" + BearerToken + "\n";
        //    return Task.CompletedTask;
        //}
        ,
        OnTokenValidated = ctx =>
        {
            //Debug.WriteLine("Token : " + ctx.SecurityToken.ToString());
            return System.Threading.Tasks.Task.CompletedTask;
        }
    };
});

services.AddControllers().AddJsonOptions(v => {
    v.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    v.JsonSerializerOptions.WriteIndented = true; 
});

services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EShopApi", Version = "v1" });  
});

services.AddScoped<IUnitOfWork, UnitOfWork>();  
services.AddScoped<IInventoryService, InventoryService>(); 
services.AddScoped<IEShopUserService, EShopUserService>(); 
services.AddScoped<IUnitTypeService, UnitTypeService>();  
services.AddScoped<IProductService, ProductService>();
services.AddScoped<IProductSubCategoryService, ProductSubCategoryService>();
services.AddScoped<IProductCategoryService, ProductCategoryService>(); 
services.AddScoped<IUnitChartService, UnitChartService>(); 
services.AddScoped<IShoppingCartService,ShoppingCartService>();
services.AddScoped<IShoppingCartItemService, ShoppingCartItemService>();
services.AddSingleton<IShoppingCart, ShoppingCart>();

services.Configure<MvcOptions>(options =>
{
    options.Filters.Add(new ProducesAttribute("application/json"));  
});
services.AddHttpContextAccessor();

services.AddSingleton<Microsoft.AspNetCore.Mvc.Infrastructure.IActionContextAccessor, Microsoft.AspNetCore.Mvc.Infrastructure.ActionContextAccessor>();
services.AddSingleton<Microsoft.AspNetCore.Mvc.Routing.IUrlHelperFactory, Microsoft.AspNetCore.Mvc.Routing.UrlHelperFactory>();
services.Configure<CustomFileLoggerOptions>(builder.Configuration.GetSection("Logging:CustomLoggingFile:Options"));
services.AddSingleton<ILoggerProvider, CustomFileLoggerProvider>(); 
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler("/error");
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();    
}
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EShopApi v1");
    //c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.Full);
    //c.EnableDeepLinking();
    c.EnableFilter();
    c.EnableValidator();
    //c.OAuth2RedirectUrl("api/WeatherForecast");
    //c.OAuthAppName("Oath");
    //c.ShowCommonExtensions();
    //c.ShowExtensions();
    //c.SupportedSubmitMethods();
});

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern:"{controller=Home}/{action=Index}/{id?}"
    );
});
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

//app.UseAsyncMiddleware();  //Use custom middleware to insert admin user
 
app.MapControllers();

app.Run();
