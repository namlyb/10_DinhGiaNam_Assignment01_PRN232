using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using _10_DinhGiaNam_Assignment01_BackEnd.DataAccess;
using _10_DinhGiaNam_Assignment01_BackEnd.Models;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("MyCnn")
    ?? throw new InvalidOperationException("Connection string 'MyCnn' was not found.");

builder.Services.AddDbContextFactory<FUNewsManagementContext>(opt => opt.UseSqlServer(connectionString), ServiceLifetime.Singleton);

var odata = new ODataConventionModelBuilder();

odata.EntitySet<SystemAccount>("Accounts");
odata.EntityType<SystemAccount>()
     .HasKey(x => x.AccountId);

odata.EntitySet<Category>("Categories");
odata.EntityType<Category>()
     .HasKey(x => x.CategoryId);

odata.EntitySet<NewsArticle>("NewsArticles");
odata.EntityType<NewsArticle>()
     .HasKey(x => x.NewsArticleId);

odata.EntitySet<Tag>("Tags");
odata.EntityType<Tag>()
     .HasKey(x => x.TagId);

builder.Services.AddControllers()
    .AddOData(opt => opt.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100).AddRouteComponents("odata", odata.GetEdmModel()))
    .AddJsonOptions(opt => opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// JWT is validated by the server; untrusted X-User-* headers are never used for authorization.
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Set Jwt:Key to a random secret of at least 32 UTF-8 bytes (via user-secrets or environment variables).");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "FUNewsManagement";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "FUNewsManagementClient";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = "role",
            NameClaimType = "name"
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Singleton-pattern repositories. Each operation creates a short-lived DbContext via IDbContextFactory.
builder.Services.AddSingleton<SystemAccountRepository>();
builder.Services.AddSingleton<CategoryRepository>();
builder.Services.AddSingleton<NewsArticleRepository>();
builder.Services.AddSingleton<TagRepository>();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
