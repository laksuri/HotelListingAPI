using HotelListingAPI.Config;
using HotelListingAPI.Contracts;
using HotelListingAPI.Data;
using HotelListingAPI.Middleware;
using HotelListingAPI.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;
using HotelListingAPI.Model;
using HotelListingAPI.Model.Country;
using System.Reflection.Emit;

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
    .AddTokenProvider<DataProtectorTokenProvider<APIUser>>("HotelListingAPI")
    .AddEntityFrameworkStores<HotelDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSwaggerGen();
builder.Services.AddScoped(typeof(IGenericRepository<>),typeof(GenericRepository<>));
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IAuthManager,AuthManager>();
//CORS changes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        b => b.AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod());
});
//Configuration for automapper
builder.Services.AddAutoMapper(typeof(MapperConfig));
//Code for JWT access token validation
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme= JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options=> {
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ClockSkew = TimeSpan.Zero,
        ValidateLifetime=true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWTSettings:Issuer"],
        IssuerSigningKey=new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTSettings:Key"])),
        ValidAudience = builder.Configuration["JWTSettings:Audience"]
    };
});
//Changes to cache the respoinse for a maximum of 10s. 
builder.Services.AddResponseCaching(options =>
{
    options.MaximumBodySize = 1024;
    options.UseCaseSensitivePaths = true;

});
//Changes to use Serilog
builder.Host.UseSerilog((context, lc) =>
{
    lc.WriteTo.Console().ReadFrom.Configuration(context.Configuration);
});
//Add API Versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),
        new HeaderApiVersionReader("api-version"),
        new MediaTypeApiVersionReader("api-version"));
});
//Add versioning to swagger
builder.Services.AddVersionedApiExplorer(options =>
{
    options.SubstituteApiVersionInUrl = true;
    options.GroupNameFormat = "'v'VVV";
});
//OData changes
//var odataModelBuilder = new ODataConventionModelBuilder();
//odataModelBuilder.EntityType<PagedResult<GetCountryDto>>();
//odataModelBuilder.EntitySet<GetCountryDto>("Items");

//$expand=items($select=name)
//$expand = items($filter = id eq 3)
//$expand = items($filter = name eq 'Singapore'; $select = name)
builder.Services.AddControllers().AddOData(options =>
{
    options.Select().Filter().OrderBy().Expand();
    //.AddRouteComponents(
    //"odata",
    //odataModelBuilder.GetEdmModel());
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseResponseCaching();
app.Use(async (context, next) =>
{
context.Response.GetTypedHeaders().CacheControl =
new Microsoft.Net.Http.Headers.CacheControlHeaderValue
{
    Public = true,
    MaxAge = TimeSpan.FromSeconds(10),
};
    context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.Vary] =
        new string[] { "Accept-Encoding" };
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
