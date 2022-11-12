using Api;
using Api.Filters;
using Api.Swagger;
using AppLogic.Events;
using DevOps.AppLogic;
using DevOps.AppLogic.Events;
using DevOps.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//Configurations
ConfigurationManager configuration = builder.Configuration;
builder.Services.AddDbContext<DevOpsContext>(options =>
{
    string connectionString = configuration["ConnectionString"];
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
    });
#if DEBUG
    options.UseLoggerFactory(LoggerFactory.Create(loggingBuilder => loggingBuilder.AddDebug()));
    options.EnableSensitiveDataLogging();
#endif
});

// Add services to the container.
builder.Services.AddScoped<DevOpsDbInitializer>();
builder.Services.AddScoped<IDeveloperRepository, DeveloperDbRepository>();
builder.Services.AddScoped<ITeamRepository, TeamDbRepository>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//lab04-AMQP
builder.Services.AddRabbitMQEventBus(configuration);
builder.Services.AddScoped<EmployeeHiredEventHandler>();

//lab05-Identity
var readPolicy = new AuthorizationPolicyBuilder()
    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
    .RequireAuthenticatedUser()
    .RequireClaim("scope", "devops.read")
    .Build();
builder.Services.AddSingleton(provider => new ApplicationExceptionFilterAttribute(provider.GetRequiredService<ILogger<ApplicationExceptionFilterAttribute>>()));

builder.Services.AddControllers(options =>
{
    options.Filters.AddService<ApplicationExceptionFilterAttribute>(); //added in lab02
    options.Filters.Add(new AuthorizeFilter(readPolicy)); //added in lab05-Identity
});

//also lab05-Identity
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

//swagger incl authentication (lab05)
string identityUrlExternal = builder.Configuration.GetValue<string>("Urls:IdentityUrlExternal");
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "DevOps.Api", Version = "v1" });
    string securityScheme = "OpenID";
    var scopes = new Dictionary<string, string>
    {
        { "devops.read", "DevOps API - Read access" },
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
        options.Audience = "devops";
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false
        };
    });

var app = builder.Build();

// start initializer
IServiceScope startUpScope = app.Services.CreateScope();
var initializer = startUpScope.ServiceProvider.GetRequiredService<DevOpsDbInitializer>();
initializer.MigrateDatabase();
initializer.SeedData();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevOps.Api v1");
        c.OAuthClientId("swagger.devops");
        c.OAuthUsePkce();
    });
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

AddEventBusSubscriptions(app);

app.Run();

// helper method
void AddEventBusSubscriptions(IApplicationBuilder app)
{
    IEventBus eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

    eventBus.Subscribe<EmployeeHiredIntegrationEvent, EmployeeHiredEventHandler>();
}