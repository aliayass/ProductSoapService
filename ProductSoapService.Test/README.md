# ProductSoapService.Test

Bu klasör, SOAP servisinin iş kurallarını doğrulayan birim testlerini içerir. Testler, Entity Framework Core InMemory sağlayıcısı ve AutoMapper kullanır. Aşağıdaki adımlarla testleri çalıştırabilir, kapsam (coverage) üretebilir ve yeni test ekleyebilirsiniz.

## İçerik ve Mimari

- **`ProductServiceTests`**: Servis katmanı için birim testleri barındırır.
  - InMemory EF Core ile hızlı ve izole test veritabanı kurar.
  - Minimal AutoMapper konfigürasyonu ile `Product` ↔ `ProductDto` dönüşümlerini yapar.
  - Testler Arrange → Act → Assert düzenindedir.
- **`ProductSoapService.Test.csproj`**:
  - Hedef çerçeve: `net8.0`
  - Bağımlılıklar: `xunit`, `FluentAssertions`, `Microsoft.EntityFrameworkCore.InMemory`, `coverlet.collector`, `Microsoft.NET.Test.Sdk`
  - API projesine referans: `..\ProductSoapService\ProductSoapService.API.csproj`

## Önkoşullar

- .NET SDK 8
- NuGet paketleri için internet erişimi

## Testleri Çalıştırma

### Komut Satırından (cmd.exe)

```bat
REM Çözüm köküne geçin
cd /d "C:\Users\ali.ayas\Desktop\SOAP ve REST\ProductSoapService"

REM (Opsiyonel) Hızlı derleme
dotnet build -c Release

REM Sadece test projesini çalıştırın
cd "ProductSoapService.Test"
dotnet test -c Release --logger "trx;LogFileName=TestResults.trx"

REM Çözüm üzerinden tüm testleri çalıştırın
cd ..
dotnet test ProductSoapService.sln -c Release --logger "trx;LogFileName=TestResults.trx"
```

### Visual Studio 2022 ile

- Test Explorer’ı açın: Test → Test Explorer
- Build sonrası testler otomatik keşfedilir.
- Tümü için: Run All
- Tek test/trait bazında çalıştırma için sağ tık → Run/Debug Selected Tests

## Kod Kapsamı (Coverage)

`coverlet.collector` sayesinde ek araca gerek olmadan kapsam toplanabilir.

```bat
REM XPlat Code Coverage topla (coverage.cobertura.xml üretir)
cd /d "C:\Users\ali.ayas\Desktop\SOAP ve REST\ProductSoapService\ProductSoapService.Test"
dotnet test -c Release --collect:"XPlat Code Coverage" --results-directory "..\TestResults"
```

Ortaya çıkan kapsama dosyası: `TestResults/<run-id>/coverage.cobertura.xml`

İsteğe bağlı HTML raporu için ReportGenerator kullanabilirsiniz:

```bat
REM ReportGenerator kurulum (global)
dotnet tool install -g dotnet-reportgenerator-globaltool

REM Raporu üret (HTML)
reportgenerator -reports:"..\TestResults\**\coverage.cobertura.xml" -targetdir:"..\CoverageReport" -reporttypes:Html

REM Raporu açın
start "" "..\CoverageReport\index.html"
```

## Testlerin Yapısı ve Örüntüler

- InMemory DbContext: Her test için benzersiz veritabanı adı kullanılarak test izolasyonu sağlanır.
- AutoMapper: Test özelinde minimal profil kurulur.
- Örnek senaryolar:
  - `AddProduct_Should_Add_New_Product`
  - `GetAllProducts_Should_Return_All_Products`
  - `GetProductById_Should_Return_Correct_Product`
  - `UpdateProduct_Should_Modify_Existing_Product`
  - `DeleteProduct_Should_Remove_Product`
  - `GetProductsByFilter_Should_Filter_Correctly`

## Yeni Test Ekleme Rehberi

1. Test adını davranış odaklı verin: `MethodName_Should_ExpectedBehavior_When_Context`.
2. Arrange: InMemory veritabanını ve AutoMapper’ı kurun.
3. Act: Tek bir davranışı tetikleyin.
4. Assert: `FluentAssertions` ile net, niyeti açık doğrulamalar yazın.

Örnek iskelet:

```csharp
// Arrange
var context = GetInMemoryDbContext();
var mapper = GetMapper();
var service = new ProductService(context, mapper);

// Act
var result = service.GetAllProducts();

// Assert
result.Should().NotBeNull();
result.Should().BeEmpty();
```

## Sık Karşılaşılan Sorunlar

- Türkçe karakterler bozuk görünüyor (örn. "K�rm�z�"):
  - Dosya kodlamasını UTF-8 yapın (VS: File → Save As → Save with Encoding… → UTF-8).
- NuGet paket sürümleri ile .NET sürümü uyumsuzluğu:
  - `dotnet restore -f` ve ardından `dotnet build` deneyin.
  - Gerekirse `ProductSoapService.Test.csproj` paket sürümlerini hedef çerçeve ile uyumlu olacak şekilde güncelleyin.
- Testler görünmüyor:
  - Çözümü temizleyip yeniden derleyin (Clean/Build Solution).
  - Test Explorer’da “Run All Tests In View” deneyin.
  - `xunit.runner.visualstudio` paketinin kurulu olduğundan emin olun.

## Hızlı Komut Başvuru

```bat
REM Tüm testleri çalıştır
cd /d "C:\Users\ali.ayas\Desktop\SOAP ve REST\ProductSoapService\ProductSoapService.Test"
dotnet test -c Release

REM Kapsam topla ve TRX üret
dotnet test -c Release --collect:"XPlat Code Coverage" --logger "trx;LogFileName=TestResults.trx"

REM Belirli bir isim desenine göre çalıştır (örnek: ProductServiceTests)
dotnet test -c Release --filter FullyQualifiedName~ProductServiceTests
```

---

Bu README, `ProductSoapService.Test` projesini hızlıca çalıştırmanız, kapsam almanız ve yeni test eklemeniz için pratik bir başvuru kılavuzudur.
