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

    public List<ProductDto> GetProductsByFilter(ProductQueryFilterDTO filter)
    {
        var query = _context.Products.AsQueryable();

        // ID filtresi - sadece 0'dan büyük değerler için
        if (filter.id.HasValue && filter.id.Value > 0)
            query = query.Where(p => p.Id == filter.id.Value);

        // Barkod filtresi - boş string kontrolü
        if (filter.barkod != null && filter.barkod.Trim() != "")
            query = query.Where(p => p.Barkod == filter.barkod.Trim());

        // Renk filtresi - boş string kontrolü
        if (filter.renk != null && filter.renk.Trim() != "")
            query = query.Where(p => p.Renk == filter.renk.Trim());

        // ItemId filtresi - boş string kontrolü
        if (filter.itemId != null && filter.itemId.Trim() != "")
            query = query.Where(p => p.ItemId == filter.itemId.Trim());

        // Beden filtresi - boş string kontrolü
        if (filter.beden != null && filter.beden.Trim() != "")
            query = query.Where(p => p.Beden == filter.beden.Trim());

        var products = query.ToList();
        return _mapper.Map<List<ProductDto>>(products);
    }

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