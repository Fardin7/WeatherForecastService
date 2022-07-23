using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherForecastService.Api;
using WeatherForecastService.Repository;
using Domain.Interface;
using DataAccess;
using WeatherForecastService.Dtos.Config;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(
    options => options.
    AddPolicy("CorsPolicy",
    builder => builder
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowAnyOrigin()
    .WithOrigins("http://localhost:8080")
    .AllowCredentials()));

builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
builder.Services.Configure<ApiConfig>(builder.Configuration.GetSection("ApiConfig"));

builder.Services.AddDbContext<ApiDbContext>(option =>
option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionStrings"),
                    sqlServerOptionsAction: sqlOPtion =>
                    {
                        sqlOPtion.EnableRetryOnFailure();
                    }));
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<ApiDbContext>();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ICurrentWeatherRepository, CurrentWeatherRepository>();
builder.Services.AddScoped<IUserCurrentWeatherRepository, UserCurrentWeatherRepository>();
builder.Services.AddScoped<IWeatherForecastApi, WeatherForecastApi>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
            .AddJwtBearer(jwt => {
                var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Key"]);

                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    RequireExpirationTime = false
                };
            });

builder.Services.AddHttpClient("ServicePlicy")
                .AddPolicyHandler(new ClientPolicy().exponentialRetryPolicy)
                .AddPolicyHandler(new ClientPolicy().circutBreakerPolicy);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApiDbContext>();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();