# 🛍️ ProductSoapService

Modern .NET 8 ve CoreWCF kullanarak geliştirilmiş, PostgreSQL veritabanı ile entegre edilmiş SOAP web servisi projesi. Çözüm, API ve Test projelerini birlikte içerir ve Hangfire ile zamanlanmış JSON dışa aktarma (export) job’ı çalıştırır.

## 📚 Çözüm Yapısı

```
ProductSoapService.sln
├─ ProductSoapService/                 # API (CoreWCF SOAP)
│  ├─ Data/                            # EF Core DbContext
│  ├─ Dtos/                            # Data Transfer Objects
│  ├─ Jobs/                            # Hangfire job ve zamanlayıcı
│  ├─ Json/                            # Ürünlerin JSON çıktıları (job kaydeder)
│  ├─ Mapping/                         # AutoMapper profilleri
│  ├─ Migrations/                      # EF Core migration'lar
│  ├─ Models/                          # Entity modelleri
│  ├─ Services/                        # İş mantığı (Service katmanı)
│  ├─ ProductSoapService.API.csproj    # API proje dosyası
│  └─ README.md                        # API odaklı detaylı dokümantasyon
└─ ProductSoapService.Test/            # xUnit test projesi
   ├─ ProductSoapService.Test.csproj
   ├─ ProductServiceTest.cs
   └─ README.md                        # Testleri çalıştırma ve coverage rehberi
```

## 🛠️ Teknoloji Stack

- **.NET 8** (C#)
- **CoreWCF** (SOAP servisleri)
- **PostgreSQL** + **Entity Framework Core** (ORM)
- **AutoMapper** (mapping)
- **Hangfire** (background jobs, cron planlama)
- **xUnit** + **FluentAssertions** (test)

## 🚀 Kurulum

### 1) Depoyu Klonla
```bash
# Doğru depo URL'si
# Açıklama: Çözümü yerel makinenize indirir
git clone https://github.com/aliayass/ProductSoapService.git
cd ProductSoapService
```

### 2) Veritabanı Bağlantısı
`ProductSoapService/appsettings.json` içinde PostgreSQL bağlantı bilgisini güncelleyin:
```json
{
  "ConnectionStrings": {
    "ProductDb": "Host=localhost;Port=5432;Database=ProductSOAPDb;Username=postgres;Password=yourpassword"
  }
}
```

### 3) Migration ve Veritabanı
```bash
# Açıklama: Migration'ları veritabanına uygular
dotnet ef database update --project ProductSoapService/ProductSoapService.API.csproj
```

> Not: `dotnet-ef` yüklü değilse `dotnet tool install -g dotnet-ef` komutunu çalıştırın ve oturumu yeniden açın.

### 4) Uygulamayı Çalıştır
```bash
# API klasörüne geç ve çalıştır
cd ProductSoapService
# Açıklama: Local ortamda CoreWCF servislerini ayağa kaldırır
dotnet run
```

## 📡 SOAP Servis Bilgileri

- **Base URL**
```
http://localhost:5192/ProductService.svc
```
- **WSDL**
```
http://localhost:5192/ProductService.svc?wsdl
```

### Postman Örnekleri

- GetAllProducts
```http
POST http://localhost:5192/ProductService.svc
Content-Type: text/xml; charset=utf-8
SOAPAction: "http://localhost:7015/ProductService/IProductService/GetAllProducts"

<soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/" xmlns:prod="http://localhost:7015/ProductService">
  <soapenv:Header/>
  <soapenv:Body>
    <prod:GetAllProducts/>
  </soapenv:Body>
</soapenv:Envelope>
```

- AddProduct
```http
POST http://localhost:5192/ProductService.svc
Content-Type: text/xml; charset=utf-8
SOAPAction: "http://localhost:7015/ProductService/IProductService/AddProduct"

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

## ⏱️ Background Job (JSON Export)

- `Jobs/JobScheduler.cs`: Hangfire ile "*/2 * * * *" cron ifadesiyle her 2 dakikada bir `ProductJsonJob` tetiklenir.
- `Jobs/ProductJsonJob.cs`: SOAP `GetAllProducts` çağrısını yapar ve sonucu JSON olarak diske kaydeder.
- Varsayılan JSON klasörü şu anda makineye özel bir yola ayarlı:
  - `C:\Users\ali.ayas\Desktop\SOAP ve REST\ProductSoapService\ProductSoapService\Json`
  - Bu yolu ihtiyacınıza göre `ProductJsonJob.cs` içindeki `JsonFolderPath` alanından güncelleyebilirsiniz.
- Kullanılan SOAPAction: `http://localhost:7015/ProductService/IProductService/GetAllProducts`

> Not: Hangfire konfigürasyonunuz `Program.cs` içinde yapılıdır. UI kullanıyorsanız dashboard adresinizi eklemeyi unutmayın.

## 🧪 Testler

Test projesi `ProductSoapService.Test/` altında yer alır.

```bash
# Çözüm kökünden tüm testleri çalıştırın
dotnet test -c Release

# Sadece test projesi
dotnet test ProductSoapService.Test/ProductSoapService.Test.csproj -c Release
```

Detaylı kullanım ve coverage için `ProductSoapService.Test/README.md` dosyasına bakın.

## 📊 Veritabanı Şeması (Products)

| Alan   | Tip          | Açıklama                    |
| ------ | ------------ | --------------------------- |
| Id     | INT          | Primary Key, Auto Increment |
| ItemId | VARCHAR(100) | Ürün kodu                   |
| Beden  | VARCHAR(50)  | Ürün bedeni                 |
| Barkod | VARCHAR(50)  | Barkod numarası             |
| Renk   | VARCHAR(50)  | Ürün rengi                  |
| Price  | DECIMAL      | Ürün fiyatı                 |

## 🔧 Koddan Önemli Parçalar

- `Program.cs` (özet):
```csharp
// CoreWCF servisleri
builder.Services.AddServiceModelServices();
builder.Services.AddServiceModelMetadata();

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductDb")));

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));
```

- `IProductService` (özet):
```csharp
[ServiceContract(Namespace = "http://localhost:7015/ProductService")]
public interface IProductService
{
    [OperationContract] List<ProductDto> GetAllProducts();
    [OperationContract] ProductDto GetProductById(int id);
    [OperationContract] void AddProduct(ProductDto product);
    [OperationContract] void UpdateProduct(ProductDto product);
    [OperationContract] void DeleteProduct(int id);
}
```

## 🚨 Sorun Giderme

- SSL Sertifikası:
```bash
dotnet dev-certs https --trust
```
- Migration sorunları:
```bash
dotnet ef migrations add InitialCreate --project ProductSoapService/ProductSoapService.API.csproj
dotnet ef database update --project ProductSoapService/ProductSoapService.API.csproj
```
- Port çakışması: `Properties/launchSettings.json` dosyasındaki portları güncelleyin.
- JSON klasör izni/yolu: `ProductJsonJob.cs` içindeki `JsonFolderPath`’i var olan ve yazılabilir bir klasör olarak güncelleyin.

## 🧭 İlgili Dokümanlar

- API detayları: `ProductSoapService/README.md`
- Test rehberi: `ProductSoapService.Test/README.md`

## 👥 Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/AmazingFeature`)
3. Commit edin (`git commit -m "Add some AmazingFeature"`)
4. Push edin (`git push origin feature/AmazingFeature`)
5. Pull Request açın

## 📎 Bağlantılar

- GitHub depo: https://github.com/aliayass/ProductSoapService

---

⭐ Bu projeyi beğendiyseniz yıldız vermeyi unutmayın!
