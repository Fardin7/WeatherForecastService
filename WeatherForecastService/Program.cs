using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherForecastService.Client;
using WeatherForecastService.Model;
using WeatherForecastService.Dtos;


var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));

builder.Services.AddDbContext<ApiDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionStrings")));



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtConfig:Issuer"],
                        ValidAudience = builder.Configuration["JwtConfig:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtConfig:Key"]))
                    };
                });
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApiDbContext>();

builder.Services.AddCors(options => options.AddDefaultPolicy(p => p.WithOrigins("http://localhost:8080")));
builder.Services.AddHttpClient("ServicePlicy");
builder.Services.AddScoped<IClientUpdate, OpenWeathermapService>();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiDbContext>();
    context.Database.Migrate();
    // requires using Microsoft.Extensions.Configuration;
    // Set password with the Secret Manager tool.
    // dotnet user-secrets set SeedUserPW <pw>

   // var testUserPw = builder.Configuration.GetValue<string>("SeedUserPW");

    //await Initialize(services, testUserPw);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
//{
//    using (var context = new ApiDbContext(
//        serviceProvider.GetRequiredService<DbContextOptions<ApiDbContext>>()))
//    {
//        // For sample purposes seed both with the same password.
//        // Password is set with the following:
//        // dotnet user-secrets set SeedUserPW <pw>
//        // The admin user can do anything

//        var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@contoso.com");


//        SeedDB(context, adminID);
//    }
//}

// static async Task<string> EnsureUser(IServiceProvider serviceProvider,
//                                            string testUserPw, string UserName)
//{
//    var userManager = serviceProvider.GetService<UserManager<User>>();

//    var user = await userManager.FindByNameAsync(UserName);
//    if (user == null)
//    {
//        user = new User
//        {
//            UserName = UserName,
//            EmailConfirmed = true
//        };
//      var result=await userManager.CreateAsync(user, testUserPw);

//        //if (!result.Succeeded)
//        //    return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "User creation failed! Please check user details and try again." });
//    }

//    if (user == null)
//    {
//        throw new Exception("The password is probably not strong enough!");
//    }

//    return user.Id;
//}
//static void SeedDB(ApiDbContext context, string adminID)
//{
//    if (context.userCurrentWeather.Any())
//    {
//        return;   // DB has been seeded
//    }

//    context.userCurrentWeather.AddRange(
//        new UserCurrentWeather
//        {
//            UserId = adminID,
//            CurrentWeatherId = 1
//        });
//    context.SaveChanges();


//}