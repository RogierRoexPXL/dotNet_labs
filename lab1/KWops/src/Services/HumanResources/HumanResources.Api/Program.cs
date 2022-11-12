using Api;
using Api.Swagger;
using HumanResources.Api.Filters;
using HumanResources.AppLogic;
using HumanResources.Domain;
using HumanResources.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//lab01
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<HumanResourcesContext>(options =>
{
    string connectionString = configuration["ConnectionString"];
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 15,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    });

#if DEBUG
    options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddDebug()));
    options.EnableSensitiveDataLogging();
#endif
});


// Add services to the container.
builder.Services.AddScoped<IEmployeeRepository, EmployeeDbRepository>();
builder.Services.AddScoped<HumanResourcesDbInitializer>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddSingleton<IEmployeeFactory, Employee.Factory>();
builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//lab04
builder.Services.AddRabbitMQEventBus(configuration);

//lab05
var readPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireClaim("scope", "hr.read")
    .Build();

//builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApplicationExceptionFilterAttribute>();
    options.Filters.Add(new AuthorizeFilter(readPolicy));
});

var writePolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireClaim("scope", "manage")
    .Build();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("write", writePolicy);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

//lab05 swagger incl auth
string identityUrlExternal = builder.Configuration.GetValue<string>("Urls:IdentityUrlExternal");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HR.Api", Version = "v1" });
    string securityScheme = "OpenID";
    var scopes = new Dictionary<string, string>
    {
        { "hr.read", "DevOps API - Read access" },
        { "manage", "Write access" }
    };
    c.AddSecurityDefinition(securityScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.OAuth2,
        Flows = new OpenApiOAuthFlows
        {
            AuthorizationCode = new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{identityUrlExternal}/connect/authorize"),
                TokenUrl = new Uri($"{identityUrlExternal}/connect/token"),
                Scopes = scopes
            }
        }
    });
    c.OperationFilter<AlwaysAuthorizeOperationFilter>(securityScheme, scopes.Keys.ToArray());
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        string identityUrl = builder.Configuration.GetValue<string>("Urls:IdentityUrl");
        options.Authority = identityUrl;
        options.Audience = "hr";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false
        };

    });

var app = builder.Build();

//lab01
IServiceScope startUpScope = app.Services.CreateScope();
var initializer = startUpScope.ServiceProvider.GetRequiredService<HumanResourcesDbInitializer>();
initializer.MigrateDatabase();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HR.Api v1");
        c.OAuthClientId("swagger.hr");
        c.OAuthUsePkce();
    });
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
