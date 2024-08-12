using Pizza_Place_Challenge.Core.Data;
using Microsoft.EntityFrameworkCore;
using Pizza_Place_Challenge.Core.Helper;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
                    .AddNewtonsoftJson(options => {
                        options.SerializerSettings.ContractResolver = new LowerCaseContractResolverHelper();
                    })
                    .AddJsonOptions(options => {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {
        Version = "v1",
        Title = "Pizza Sales API",
        Description = "A simple example ASP.NET Core Web API",
    });

    // Custom grouping logic
    c.TagActionsBy(api =>
    {
        return new[] { api.GroupName ?? api.ActionDescriptor.RouteValues["controller"] };
    });

    c.DocInclusionPredicate((name, api) => true);
});

// ADD DB CONTEXT TO SERVICES
builder.Services.AddDbContext<DataContext>(options => {
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI(
        options => {
            options.DefaultModelsExpandDepth(-1); // this hides the schema section on the bottom 
            options.EnableTryItOutByDefault();
            options.DocumentTitle = "Pizza Sales API";
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pizza Sales");
        });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
