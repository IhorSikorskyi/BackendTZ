using BackendTZ.Data;
using BackendTZ.Extensions;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddConfiguredCors(builder.Configuration);
builder.Services.AddSwaggerWithJwt();
builder.Services.AddJwtAuthentication(builder.Configuration);

// Configure SQLite database connection
builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI Container registrations for repositories
builder.Services.AddRepositories();

// DI Container registrations for services
builder.Services.AddApplicationServices();

// DI Container registrations for data seeding
builder.Services.AddDataSeeding();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Conference Room Booking API v1");
    });

    await app.Services.SeedDatabaseAsync();
}

app.UseHttpsRedirection();
app.UseCors("AllowConfiguredOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

await app.RunAsync();