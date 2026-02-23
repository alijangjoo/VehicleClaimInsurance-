using Insurance.UserService.AppCore.AppServices.AAA.Services;
using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Roles.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Add this using
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AuthenticationService = Insurance.UserService.AppCore.AppServices.AAA.Services.AuthenticationService;
using IAuthenticationService = Insurance.UserService.AppCore.Shared.AAA.Services.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<InsuranceUserServiceDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<AppUser, AppRole>(options =>
{
    // تنظیمات Password
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;

    // تنظیمات Lockout
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 5;

    // تنظیمات User
    options.User.RequireUniqueEmail = true;

    // تنظیمات SignIn
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<InsuranceUserServiceDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<ITokenService, JwtTokenService>();

// اضافه کردن JwtSettings به تنظیمات
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// اضافه کردن HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// اضافه کردن سرویس‌ها
builder.Services.AddScoped<ITokenService, JwtTokenService>(); // Duplicate registration, you can remove one
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>(); // Duplicate registration, you can remove one
builder.Services.AddScoped<IUserContextService, UserContextService>(); // Duplicate registration, you can remove one
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// تنظیمات احراز هویت با JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings?.Issuer,
        ValidAudience = jwtSettings?.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings?.SecretKey ?? "")),
        ClockSkew = TimeSpan.Zero
    };

    // اضافه کردن رویدادها برای استفاده در SignalR یا WebSockets
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Don't forget to add Authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();