using System.Reflection;
using System.Text;
using HotelListing.Api.Common.Constants;
using HotelListing.Api.Common.Models;
using HotelListing.Api.Contracts;
using HotelListing.Api.Data;
using HotelListing.Api.Handlers;
using HotelListing.Api.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

// Add services to the Inversion of Control (IoC) container.

// Add Identity services to the IoC container, specifying ApplicationUser as the user type and configuring it to use Entity Framework Core for data storage with the HotelListingDbContext
builder.Services.AddIdentityApiEndpoints<ApplicationUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<HotelListingDbContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));

//Hard binding of the jwtSettings in AppSettings.Json, and gets the configured settings and if its empty, creates a new object then checks the key before app starts.
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();

if(string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException("JwtSettings: Key is not configured");
}
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey= true,
    ValidIssuer = jwtSettings.Issuer,
    ValidAudience = jwtSettings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key)),
    ClockSkew = TimeSpan.Zero //Default is 5 Minutes
};
})
.AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(AuthenticationDefaults.BasicScheme, _ => { })
.AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationDefaults.ApiKeyScheme, _ => { });

builder.Services.AddAuthorization();

// Get the connection string from the configuration and use it to configure the DbContext for dependency injection
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<HotelListingDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register the services for countries, hotels, users, and API key validation in the IoC container
builder.Services.AddScoped<ICountriesService, CountriesService>();
builder.Services.AddScoped<IHotelsService, HotelsService>();
builder.Services.AddScoped<IUsersService, UsersService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IApiKeyValidatorService, ApiKeyValidatorService>();

// Register AutoMapper and add the mapping profiles for hotels and countries to the configuration

// builder.Services.AddAutoMapper(cfg =>
// {
//     cfg.AddProfile<HotelMappingProfile>();
//     cfg.AddProfile<CountryMappingProfile>();
// });

builder.Services.AddAutoMapper(cfg => {}, Assembly.GetExecutingAssembly());

// Add controllers to the IoC container and configure JSON serialization options to ignore reference cycles
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
    { 
        Title = "Hotel Listing API", 
        Version = "v1" 
    });

    // 1. Define the HTTP Bearer Security Scheme
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token below.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http, // Changed from ApiKey to Http
        Scheme = "bearer",                                       // Lowercase 'bearer' tells Swagger to prefix it automatically
        BearerFormat = "JWT"                                     // Optional: provides a hint to the user
    });

    // 2. Link the security requirement
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
//builder.Services.AddSwaggerGen();
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo 
//     { 
//         Title = "Hotel Listing API", 
//         Version = "v1" 
//     });

//     // 1. Define the Bearer Security Scheme
//     options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//     {
//         Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
//                       Enter 'Bearer' [space] and then your token in the text input below.
//                       \r\n\r\nExample: 'Bearer 12345abcdef'",
//         Name = "Authorization",
//         In = Microsoft.OpenApi.Models.ParameterLocation.Header,
//         Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
//         Scheme = "Bearer"
//     });

//     // 2. Make Swagger use the security scheme globally for endpoints with [Authorize]
//     options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
//     {
//         {
//             new Microsoft.OpenApi.Models.OpenApiSecurityScheme
//             {
//                 Reference = new Microsoft.OpenApi.Models.OpenApiReference
//                 {
//                     Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
//                     Id = "Bearer"
//                 },
//                 Scheme = "oauth2",
//                 Name = "Bearer",
//                 In = Microsoft.OpenApi.Models.ParameterLocation.Header
//             },
//             new List<string>()
//         }
//     });
// });


var app = builder.Build();

// Add Identity API endpoints to the application, specifying ApplicationUser as the user type
app.MapGroup("api/defaultauth").MapIdentityApi<ApplicationUser>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
