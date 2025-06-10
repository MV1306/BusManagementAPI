using BusManagementAPI.Data;
using BusManagementAPI.Models;
using BusManagementAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.AllowAnyOrigin()
        //WithOrigins("https://localhost:44315")  // Allow the specific origin
                           .AllowAnyHeader()
                           .AllowAnyMethod());
});

builder.Services.AddDbContext<BusDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable middleware
app.UseStaticFiles();

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting(); // <-- Add this
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization(); // <-- Add this if using [Authorize] or similar

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
