using BusManagementAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure CORS to allow specific frontend origin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policyBuilder =>
    {
        policyBuilder
            .WithOrigins("http://mtcchennaibus.infinityfreeapp.com") // Replace/add other origins as needed
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Add EF Core DbContext
builder.Services.AddDbContext<BusDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add MVC controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware setup

// For serving static files (if any)
app.UseStaticFiles();

// Use routing
app.UseRouting();

// Enable CORS BEFORE authorization
app.UseCors("AllowSpecificOrigin");

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

// Optional: Use HTTPS redirection
app.UseHttpsRedirection();

// Authorization (add authentication if needed)
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Run the app
app.Run();
