using CoreWCF;
using CoreWCF.Configuration;
using CoreWCF.Description;
using Microsoft.EntityFrameworkCore;
using ProductSoapService.Data;

var builder = WebApplication.CreateBuilder(args);

// CoreWCF Konfigürasyonu
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductDb")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ProductService'i DI container'a ekle
builder.Services.AddScoped<ProductService>();

var app = builder.Build();

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

app.Run();