using AutoMapper;
using ProductSoapService.Data;
using ProductSoapService.Dtos;
using ProductSoapService.Models;

public class ProductService : IProductService
{
    private readonly AppDbContext _context;
    private readonly IMapper _mapper;

    public ProductService(AppDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public List<ProductDto> GetAllProducts() =>
        _mapper.Map<List<ProductDto>>(_context.Products.ToList());

    public ProductDto GetProductById(int id) =>
        _mapper.Map<ProductDto>(_context.Products.Find(id));

    public void AddProduct(ProductDto dto)
    {
        var product = _mapper.Map<Product>(dto);
        _context.Products.Add(product);
        _context.SaveChanges();
    }

    public void UpdateProduct(ProductDto dto)
    {
        var product = _context.Products.Find(dto.id);
        _mapper.Map(dto, product);
        _context.SaveChanges();
    }

    public void DeleteProduct(int id)
    {
        var product = _context.Products.Find(id);
        _context.Products.Remove(product);
        _context.SaveChanges();
    }
}