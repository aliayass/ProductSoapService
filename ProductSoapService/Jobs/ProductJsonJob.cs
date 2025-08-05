using Newtonsoft.Json;
using ProductSoapService.Dtos;
using System.Buffers;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ProductSoapService.Jobs
{
    public class ProductJsonJob
    {
        private readonly HttpClient _httpClient;

        private const string ApiUrl = "http://localhost:5192/ProductService.svc"; //endopint

        private readonly string JsonFolderPath = "C:\\Users\\ali.ayas\\Desktop\\SOAP ve REST\\ProductSoapService\\ProductSoapService\\Json";

        // SOAP Action header'ı için kullanılan endpoint
        public string soapEndpoint = "http://localhost:7015/ProductService/IProductService/GetAllProducts";

       
        public ProductJsonJob(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
        }

       
        // Ana işlem metodu - SOAP API'den veri çekip JSON olarak kaydeder
        public async Task ExecuteAsync()
        {
            try
            {
                Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] SOAP API'den veri çekiliyor...");

                // 1. SOAP XML isteği oluştur - GetAllProducts metodunu çağırmak için
                var soapXml = $"""
                <soapenv:Envelope xmlns:soapenv="http://schemas.xmlsoap.org/soap/envelope/"
                                  xmlns:prod="http://localhost:7015/ProductService">
                    <soapenv:Header/>
                    <soapenv:Body>
                        <prod:GetAllProducts/>
                    </soapenv:Body>
                </soapenv:Envelope>
                """;

                // 2. HTTP isteği için content hazırla
                var content = new StringContent(soapXml, Encoding.UTF8, "text/xml");
                // SOAP Action header'ını ekle - hangi metodun çağrılacağını belirtir
                content.Headers.Add("SOAPAction", "http://localhost:7015/ProductService/IProductService/GetAllProducts");

                // 3. SOAP API'ye HTTP POST isteği gönder
                var response = await _httpClient.PostAsync(ApiUrl, content);

                // 4. İstek başarılı mı kontrol et
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[ERROR] API isteği başarısız: {response.StatusCode}");
                    return;
                }

                // 5. XML yanıtını string olarak al
                var xmlResponse = await response.Content.ReadAsStringAsync();

                // 6. XML'i JSON formatına dönüştür
                var jsonData = ConvertXmlToJson(xmlResponse);

                // 7. Eski JSON dosyalarını temizle - sadece en son dosyayı tutmak için
                var oldFiles = Directory.GetFiles(JsonFolderPath, "*.json");
                foreach (var file in oldFiles)
                    File.Delete(file);

                // 8. Yeni JSON dosyasını kaydet
                var fileName = $"products_{DateTime.Now:yyyyMMdd_HHmmss}.json"; // Zaman damgalı dosya adı
                var filePath = Path.Combine(JsonFolderPath, fileName);

                // JSON klasörünü oluştur (yoksa)
                Directory.CreateDirectory(JsonFolderPath);

                // JSON verisini dosyaya yaz
                await File.WriteAllTextAsync(filePath, jsonData, Encoding.UTF8);

                Console.WriteLine($"[✓] JSON dosyası kaydedildi: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Hata: {ex.Message}");
            }
        }

        /// <summary>
        /// XML string'ini JSON formatına dönüştürür
        /// </summary>
        /// <param name="xmlString">Dönüştürülecek XML string'i</param>
        /// <returns>JSON formatında string</returns>
        private string ConvertXmlToJson(string xmlString)
        {
            try
            {
                // XML string'ini XDocument olarak parse et
                var xmlDoc = XDocument.Parse(xmlString); // XDocument ==>> string veriyi üzerinde işlem yapılabilecek bir xml formatına getirir.

                // SOAP envelope'dan gerçek veriyi çıkar
                // GetAllProductsResponse > GetAllProductsResult yolunu takip et
                var body = xmlDoc.Descendants()
                    .FirstOrDefault(x => x.Name.LocalName == "GetAllProductsResponse")? //XML'deki GetAllProductResponse elementine erişip ==>>
                    .Descendants()// XML'deki tüm elementleri (recursive olarak) listeler.
                    .FirstOrDefault(x => x.Name.LocalName == "GetAllProductsResult"); // onunda içinden GetAllResponse elementini body'e kopyalıyor.

                if (body != null)
                {
                    // Bulunan veriyi JSON'a dönüştür
                    return JsonConvert.SerializeXNode(body, Newtonsoft.Json.Formatting.Indented); //JsonConvert.SerializeXNode(...) Newtonsoft.Json kütüphanesinden gelir.  XML verisini alır ve onu JSON string haline getirir.
                } //Formatting.Indented ==>> JSON çıktısının güzel, okunabilir şekilde (girintili) oluşturulmasını sağlar. Her satır ayrı yazılır, girintiler olur.

                // Eğer beklenen yapı bulunamazsa tüm XML'i JSON'a çevir
                return JsonConvert.SerializeXNode(xmlDoc.Root, Newtonsoft.Json.Formatting.Indented);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] XML dönüştürme hatası: {ex.Message}");

                // Hata durumunda hata bilgilerini içeren JSON döndür
                return JsonConvert.SerializeObject(new
                {
                    error = "XML parse hatası",
                    xml = xmlString,
                    timestamp = DateTime.Now
                }, Newtonsoft.Json.Formatting.Indented);
            }
        }
    }
}