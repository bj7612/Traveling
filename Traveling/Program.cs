using Microsoft.EntityFrameworkCore;
using Traveling.Database;
using Traveling.Services;
using Microsoft.Extensions.Configuration;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using Traveling.Models;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews(setupAction => {
    setupAction.ReturnHttpNotAcceptable = true; 
    // setupAction.OutputFormatters.Add(
    //    new XmlDataContractSerializerOutputFormatter()    
    //);
})
// 8-5 【应用】使用PATCH部分更新资源
 .AddNewtonsoftJson(setupAction => {
     setupAction.SerializerSettings.ContractResolver =
         new CamelCasePropertyNamesContractResolver();
 })
.AddXmlDataContractSerializerFormatters()
// 7-9 【应用】输出状态码 422
.ConfigureApiBehaviorOptions(setupAction =>    
{
     setupAction.InvalidModelStateResponseFactory = context =>
     {
         var problemDetail = new ValidationProblemDetails(context.ModelState)
         {
             Type = "Data erro",
             Title = "Data validation failed",
             Status = StatusCodes.Status422UnprocessableEntity,
             Detail = "See detail",
             Instance = context.HttpContext.Request.Path
         };
         problemDetail.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
         return new UnprocessableEntityObjectResult(problemDetail)
         {
             ContentTypes = { "application/problem+json" }
         };
     };
 });

builder.Services.AddEndpointsApiExplorer();

// Add Configuration
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Configuration.AddConfiguration(configuration);


// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options => {
    //options.UseSqlServer("server=localhost; Database=TravelingDb; User Id=sa; Password=PaSSword12!;"); 
    options.UseSqlServer(configuration["DbContext:ConnectionString"]);
    //options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=master;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False");
});


// Add Automapper: scan all profile to create the relationship of projecting based on the constructor in profile,
// then load them into appDomain.
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


// Register dependencies
builder.Services.AddTransient<ITouristRouteRepository, TouristRouteRepository>();

// 11-6 【应用】用户模型设计与数据库更新
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

//11-4 【应用】启动API授权
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
                {
                    var secretByte = Encoding.UTF8.GetBytes(configuration["Authentication:SecretKey"]);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration["Authentication:Issuer"],

                        ValidateAudience = true,
                        ValidAudience = configuration["Authentication:Audience"],

                        ValidateLifetime = true,

                        IssuerSigningKey = new SymmetricSecurityKey(secretByte)
                    };
                });


// App
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Where are you
app.UseRouting();
// who are you
app.UseAuthentication();
// what is your permission
app.UseAuthorization();
app.UseHttpsRedirection();
app.UseStaticFiles();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// middle ware
app.UseEndpoints(endpoints => {
    endpoints.MapGet("/test", async context =>
    {
        //throw new Exception("test");
        await context.Response.WriteAsync("Hello from test!");
    });

    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

app.Run();
