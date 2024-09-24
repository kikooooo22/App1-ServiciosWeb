using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddCors();

// En donde encuentre 'ITokenService' lo remplaza por (injecta) 'TokenService'
builder.Services.AddScoped<ITokenService, TokenService>(); 

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => 
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new ArgumentNullException(nameof(builder.Configuration["TokenKey"]));
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false, 
            ValidateAudience = false
        };
    });

var app = builder.Build();

// Configura the http request pipeline
app.UseCors((cors) => cors
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins(
        "http://localhost:4200",
        "https://localhost:4200"
    )
);

app.UseAuthorization();

app.MapControllers();

app.Run();
