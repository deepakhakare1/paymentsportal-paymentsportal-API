using Microsoft.EntityFrameworkCore;
using PaymentsApi.Data;
using PaymentsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure DbContext - SQLite by default (file: payments.db)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=payments.db";
builder.Services.AddDbContext<PaymentsDbContext>(options =>
    options.UseSqlite(connectionString));

// DI for Payment service
builder.Services.AddScoped<IPaymentService, PaymentService>();

// CORS - allow local dev clients (adjust for production)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
var app = builder.Build();

// Ensure DB created / apply migrations at startup (safe for dev)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PaymentsDbContext>();
    db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
