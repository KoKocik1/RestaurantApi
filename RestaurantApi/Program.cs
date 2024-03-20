using System.Reflection;
using System.Text;
using Azure.Core;
using Azure.Identity;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using RestaurantApi;
using RestaurantApi.Authorization;
using RestaurantApi.Entities;
using RestaurantApi.IService;
using RestaurantApi.Middleware;
using RestaurantApi.Models;
using RestaurantApi.Services;
using RestaurantApi.Validators;

var builder = WebApplication.CreateBuilder();

// Nlog
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

// authentication
var authenticationSettings = new AuthenticationSettings();

builder.Configuration.GetSection("Authentication").Bind(authenticationSettings);
builder.Services.AddSingleton(authenticationSettings);
builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = "Bearer";
    option.DefaultScheme = "Bearer";
    option.DefaultChallengeScheme = "Bearer";
}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey)),
    };
});
builder.Services.AddAuthorization(option =>
{
    option.AddPolicy("HasNationality", builder => builder.RequireClaim("Nationality", "German", "Polish"));
    option.AddPolicy("atleast20", builder => builder.AddRequirements(new MinimumAgeRequirement(20)));
    option.AddPolicy("CreatedAtkeast2Restaurants", builder => builder.AddRequirements(new MinimumTwoRestaurant(2)));
});

//Authorization
builder.Services.AddScoped<IAuthorizationHandler, MinimumTwoRestaurantHandler>();
builder.Services.AddScoped<IAuthorizationHandler, MinimumAgeRequirementHandler>();
builder.Services.AddScoped<IAuthorizationHandler, ResourceOperationRequirementHandler>();
builder.Services.AddControllers();


// Add services to the container.
builder.Services.AddRazorPages();

// Seeder
builder.Services.AddScoped<RestaurantSeeder>();

// Mapper
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

builder.Services.AddScoped<IRestaurantService, RestaurantService>();
builder.Services.AddScoped<IDishService, DishService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ErrorHandlingMiddleware>();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IValidator<RegisterUserDto>, RegisterUserDtoValidator>();
builder.Services.AddScoped<IValidator<RestaurantQuery>, RestaurantQueryWalidator>();
builder.Services.AddScoped<RequestTimeMiddleware>();
builder.Services.AddScoped<IUserContentService, UserContentService>();


builder.Services.AddHttpContextAccessor();

//swagger
builder.Services.AddSwaggerGen();

//Cors 
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEndClient", policyBuilder =>

        policyBuilder.AllowAnyMethod()
        .AllowAnyHeader()
        .WithOrigins(builder.Configuration["AllowedOrigins"])
    );
});


// Db

//var defaultCredential = new DefaultAzureCredential();
//var tokenRequestContext = new TokenRequestContext(new[] { "https://database.windows.net/.default" });
//var accessToken = defaultCredential.GetToken(tokenRequestContext);

builder.Services.AddDbContext<RestaurantDBContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection"));
    //options.UseToken();
    //options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantDbConnection"), sqlServerOptions =>
    //{
    //    sqlServerOptions.AddAzureIdentity(accessToken.Token); // Dodaj token dostępu
    //});
});

var app = builder.Build();
///////////////////////////////////

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<RestaurantSeeder>();


app.UseResponseCaching();
app.UseStaticFiles();
app.UseCors("FrontEndClient");

seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}



app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<RequestTimeMiddleware>();

app.UseAuthentication();
app.UseHttpsRedirection();


app.UseSwagger();
app.UseSwaggerUI(x =>
{
    x.SwaggerEndpoint("/swagger/v1/swagger.json", "Restaurant Api");
});

app.UseRouting();

app.UseAuthorization();

app.UseEndpoints(endpoints =>
    endpoints.MapControllers()
);

app.Run();

