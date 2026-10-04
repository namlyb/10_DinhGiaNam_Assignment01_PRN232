using System.Text.Json.Serialization;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.ModelBuilder;
using _10_DinhGiaNam_Assignment01_BackEnd.DataAccess;
using _10_DinhGiaNam_Assignment01_BackEnd.Models;
using _10_DinhGiaNam_Assignment01_BackEnd.Repositories;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

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
app.MapControllers();
app.Run();
