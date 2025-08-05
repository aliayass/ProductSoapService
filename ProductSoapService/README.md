# 🛍️ ProductSoapService

Modern .NET 8 ve CoreWCF kullanarak geliştirilmiş, PostgreSQL veritabanı ile entegre edilmiş SOAP web servisi projesi.

## 📋 Proje Özeti

Bu proje, ürün yönetimi için CRUD (Create, Read, Update, Delete) operasyonları sunan bir SOAP web servisi oluşturur. REST API yerine geleneksel SOAP protokolü kullanarak, enterprise uygulamalar için uygun bir çözüm sunar.

## 🏗️ Mimari Yapı

```
ProductSoapService/
├── Models/          # Entity modelleri
├── DTOs/           # Data Transfer Objects  
├── Services/       # Business logic katmanı
├── Data/           # Database context
├── Mapping/        # AutoMapper profilleri
└── Migrations/     # Database migrations
```

## 🛠️ Teknoloji Stack

- **.NET 8** - Modern framework
- **CoreWCF** - SOAP web servisleri
- **PostgreSQL** - Veritabanı
- **Entity Framework Core** - ORM
- **AutoMapper** - Object mapping
- **Layered Architecture** - Temiz kod yapısı

## 🚀 Kurulum

### Gereksinimler
- .NET 8 SDK
- PostgreSQL
- Visual Studio 2022 veya VS Code

### Adım 1: Projeyi Klonlayın
```bash
git clone https://github.com/yourusername/ProductSoapService.git
cd ProductSoapService
```

### Adım 2: Veritabanı Bağlantısını Yapılandırın
`appsettings.json` dosyasında PostgreSQL connection string'ini güncelleyin:

```json
{
  "ConnectionStrings": {
    "ProductDb": "Host=localhost;Port=5432;Database=ProductSOAPDb;Username=postgres;Password=yourpassword"
  }
}
```

### Adım 3: Migration'ları Çalıştırın
```bash
dotnet ef database update
```

### Adım 4: Uygulamayı Başlatın
```bash
dotnet run
```

## 📡 API Endpoints

### Base URL
```
http://localhost:5192/ProductService.svc
```

### WSDL Dokümantasyonu
```
http://localhost:5192/ProductService.svc?wsdl
```

## 🧪 Test Etme

### SOAP UI ile Test

1. **SOAP UI'ı açın**
2. **File → New SOAP Project**
3. **Initial WSDL:** `http://localhost:5192/ProductService.svc?wsdl`
4. **Project Name:** `ProductSoapService`
5. **OK** butonuna tıklayın

### Postman ile Test

#### GetAllProducts
- **Method:** POST
- **URL:** `http://localhost:5192/ProductService.svc`
- **Headers:**
  ```
  Content-Type: text/xml; charset=utf-8
  SOAPAction: "http://localhost:7015/ProductService/IProductService/GetAllProducts"
  ```
- **Body:**
  ```xml
  <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:prod="http://localhost:7015/ProductService">
     <soapenv:Header/>
     <soapenv:Body>
        <prod:GetAllProducts/>
     </soapenv:Body>
  </soapenv:Envelope>
  ```

#### AddProduct
- **Method:** POST
- **URL:** `http://localhost:5192/ProductService.svc`
- **Headers:**
  ```
  Content-Type: text/xml; charset=utf-8
  SOAPAction: "http://localhost:7015/ProductService/IProductService/AddProduct"
  ```
- **Body:**
  ```xml
  <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:prod="http://localhost:7015/ProductService">
     <soapenv:Header/>
     <soapenv:Body>
        <prod:AddProduct>
           <prod:product>
              <prod:id>0</prod:id>
              <prod:itemId>PROD001</prod:itemId>
              <prod:beden>M</prod:beden>
              <prod:barkod>123456789</prod:barkod>
              <prod:renk>Kırmızı</prod:renk>
              <prod:price>99.99</prod:price>
           </prod:product>
        </prod:AddProduct>
     </soapenv:Body>
  </soapenv:Envelope>
  ```

## 📊 Veritabanı Şeması

### Products Tablosu
| Alan | Tip | Açıklama |
|------|-----|----------|
| Id | INT | Primary Key, Auto Increment |
| ItemId | VARCHAR(100) | Ürün kodu |
| Beden | VARCHAR(50) | Ürün bedeni |
| Barkod | VARCHAR(50) | Barkod numarası |
| Renk | VARCHAR(50) | Ürün rengi |
| Price | DECIMAL | Ürün fiyatı |

## 🔧 Konfigürasyon

### Program.cs
```csharp
// CoreWCF Konfigürasyonu
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();
builder.Services.AddSingleton<IServiceBehavior, UseRequestHeadersForMetadataAddressBehavior>();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductDb")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

### IProductService.cs
```csharp
[ServiceContract(Namespace = "http://localhost:7015/ProductService")]
public interface IProductService
{
    [OperationContract]
    List<ProductDto> GetAllProducts();

    [OperationContract]
    ProductDto GetProductById(int id);

    [OperationContract]
    void AddProduct(ProductDto product);

    [OperationContract]
    void UpdateProduct(ProductDto product);

    [OperationContract]
    void DeleteProduct(int id);
}
```

## 🎯 Özellikler

- ✅ **SOAP API** - REST yerine SOAP protokolü
- ✅ **CRUD İşlemleri** - Tam ürün yönetimi
- ✅ **PostgreSQL** - Modern veritabanı
- ✅ **Layered Architecture** - Temiz kod yapısı
- ✅ **AutoMapper** - Otomatik mapping
- ✅ **WSDL Desteği** - Otomatik dokümantasyon
- ✅ **Entity Framework Core** - Modern ORM
- ✅ **.NET 8** - En güncel framework

## 🚨 Sorun Giderme

### SSL Sertifikası Sorunu
```bash
dotnet dev-certs https --trust
```

### Migration Sorunu
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Port Sorunu
`Properties/launchSettings.json` dosyasında port ayarlarını kontrol edin.

## 📝 Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## 👥 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m 'Add some AmazingFeature'`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. Pull Request oluşturun

## 📞 İletişim

- **Proje Sahibi:** [Your Name]
- **Email:** your.email@example.com
- **GitHub:** [@yourusername](https://github.com/yourusername)

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın! 