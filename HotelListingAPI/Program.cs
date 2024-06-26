using HotelListingAPI.Config;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnection");
builder.Services.AddDbContext<HotelDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});
builder.Services.AddIdentityCore<APIUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HotelDbContext>();

builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IAuthManager,AuthManager>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Host.UseSerilog((context, lc) =>
{
    lc.WriteTo.Console().ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();
