using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.EntityFrameworkCore;
using ProductSoapService.Data;
using ProductSoapService.Jobs;

var builder = WebApplication.CreateBuilder(args);

// Hangfire konfig�rasyonu
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("ProductDb")));
builder.Services.AddHangfireServer();

// HttpClient'� singleton olarak ekle
builder.Services.AddHttpClient<ProductJsonJob>();

// CoreWCF Konfigürasyonu
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductDb")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// JobScheduler'� DI container'a ekle
builder.Services.AddScoped<JobScheduler>();

// ProductService'i DI container'a ekle
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

// Scheduler'� ba�lat
using (var scope = app.Services.CreateScope())
{
    var jobScheduler = scope.ServiceProvider.GetRequiredService<JobScheduler>();
    jobScheduler.Schedule();
}

// SOAP Endpoint - Tam konfigürasyon
app.UseServiceModel(builder =>
{
    builder.AddService<ProductService>();

    // HTTP endpoint
    builder.AddServiceEndpoint<ProductService, IProductService>(
        new BasicHttpBinding(),
        "/ProductService.svc"
    );

    // WSDL Desteği
    var serviceMetadataBehavior = app.Services.GetRequiredService<ServiceMetadataBehavior>();
    serviceMetadataBehavior.HttpGetEnabled = true;
    serviceMetadataBehavior.HttpsGetEnabled = true;
});

app.UseHangfireDashboard();


app.Run();