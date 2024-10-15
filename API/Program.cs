using API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Haciendo uso de extensiones, 
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

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
