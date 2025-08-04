using CoreWCF;
using ProductSoapService.Dtos;

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