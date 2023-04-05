using Serilog;
using src.Adapters.Driven.Infra.ServiceLayer;
using Application;
using Api.Configurations;
using Infra.Message;
using Api.Helpers;
using Infra.Pdf;
using Infra.Intelipost;

var builder = WebApplication.CreateBuilder(args);
builder.AddSerilog(builder.Configuration, "API WM");
builder.AddUaa(builder.Configuration);
Log.Information("Starting API");

builder.Services.AddHttpContextAccessor();

ConfigurationManager configuration = builder.Configuration;
IServiceCollection services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();
services.AddServiceLayerModule(configuration);
services.AddRabbitMQ(configuration);
services.AddPdf();
services.AddIntelipostServiceDependency(configuration);
services.AddServicesInjector(configuration);
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddRouting(options => options.LowercaseUrls = true);

services.AddCors(options =>
{
    if (builder.Environment.IsDevelopment() || builder.Environment.EnvironmentName == "Staging")
    {
        options.AddDefaultPolicy(
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    }
    else
    {
        options.AddDefaultPolicy(
            builder =>
            {
                builder.WithOrigins("https://backoffice-hml.cetro.com.br/")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
                builder.WithOrigins("https://backoffice.cetro.com.br")
                                    .AllowAnyHeader()
                                    .AllowAnyMethod();
            });
    }
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseSerilog();

app.UseCors();

app.UseHttpsRedirection(); 

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers()
    .RequireAuthorization("ApiScope");

app.Run();