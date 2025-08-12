using Xunit;
using FluentAssertions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using ProductSoapService.Data;
using ProductSoapService.Dtos;
using ProductSoapService.Models;

public class ProductServiceTests
{
    private AppDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + System.Guid.NewGuid()) // Her test için farklý DB ismi
            .Options;
        return new AppDbContext(options);
    }

    private IMapper GetMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Product, ProductDto>().ReverseMap();
            cfg.CreateMap<ProductQueryFilterDTO, ProductQueryFilterDTO>(); // Gerekirse
        });
        return config.CreateMapper();
    }

    [Fact]
    public void AddProduct_Should_Add_New_Product()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var mapper = GetMapper();
        var service = new ProductService(context, mapper);

        var dto = new ProductDto
        {
            id = 1,
            barkod = "B123",
            itemId = "I123",
            price = 10,
            beden = "M",
            renk = "Kýrmýzý"
        };

        // Act
        service.AddProduct(dto);

        // Assert
        var productInDb = context.Products.FirstOrDefault(p => p.Id == dto.id);
        productInDb.Should().NotBeNull();
        productInDb.Barkod.Should().Be(dto.barkod);
    }

    [Fact]
    public void GetAllProducts_Should_Return_All_Products()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.AddRange(new List<Product>
        {
            new Product { Id = 1, Barkod = "B1", ItemId = "I1", Price = 5, Beden = "S", Renk = "Mavi" },
            new Product { Id = 2, Barkod = "B2", ItemId = "I2", Price = 10, Beden = "L", Renk = "Kýrmýzý" }
        });
        context.SaveChanges();

        var mapper = GetMapper();
        var service = new ProductService(context, mapper);

        // Act
        var products = service.GetAllProducts();

        // Assert
        products.Should().HaveCount(2);
        products.Select(p => p.barkod).Should().Contain(new[] { "B1", "B2" }); // barkod deðerlerini kontrol et
    }

    [Fact]
    public void GetProductById_Should_Return_Correct_Product()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Id = 2, Barkod = "B1", ItemId = "I1", Price = 5, Beden = "S", Renk = "Mavi" });
        context.SaveChanges();

        var mapper = GetMapper();
        var service = new ProductService(context, mapper);

        // Act
        var product = service.GetProductById(1);

        // Assert
        product.Should().NotBeNull();
        product.barkod.Should().Be("B1");
    }

    [Fact]
    public void UpdateProduct_Should_Modify_Existing_Product()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Id = 1, Barkod = "B1", ItemId = "I1", Price = 5, Beden = "S", Renk = "Mavi" });
        context.SaveChanges();

        var mapper = GetMapper();
        var service = new ProductService(context, mapper);

        var updatedDto = new ProductDto
        {
            id = 1,
            barkod = "B1-Updated",
            itemId = "I1",
            price = 20,
            beden = "M",
            renk = "Siyah"
        };

        // Act
        service.UpdateProduct(updatedDto);

        // Assert
        var updatedProduct = context.Products.Find(1);
        updatedProduct.Barkod.Should().Be("B1-Updated");
        updatedProduct.Price.Should().Be(20);
        updatedProduct.Renk.Should().Be("Siyah");
    }

    [Fact]
    public void DeleteProduct_Should_Remove_Product()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.Add(new Product { Id = 1, Barkod = "B1", ItemId = "I1", Price = 5, Beden = "S", Renk = "Mavi" });
        context.SaveChanges();

        var mapper = GetMapper();
        var service = new ProductService(context, mapper);

        // Act
        service.DeleteProduct(1);

        // Assert
        var product = context.Products.Find(1);
        product.Should().BeNull();
    }

    [Fact]
    public void GetProductsByFilter_Should_Filter_Correctly()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        context.Products.AddRange(new List<Product>
        {
            new Product { Id = 1, Barkod = "B1", ItemId = "I1", Price = 5, Beden = "S", Renk = "Mavi" },
            new Product { Id = 2, Barkod = "B2", ItemId = "I2", Price = 10, Beden = "L", Renk = "Kýrmýzý" },
            new Product { Id = 3, Barkod = "B3", ItemId = "I3", Price = 15, Beden = "M", Renk = "Mavi" }
        });
        context.SaveChanges();

        var mapper = GetMapper();
        var service = new ProductService(context, mapper);

        var filter = new ProductQueryFilterDTO
        {
            renk = "Mavi",
            beden = "M"
        };

        // Act
        var filteredProducts = service.GetProductsByFilter(filter);

        // Assert
        filteredProducts.Should().HaveCount(1);
        filteredProducts.First().id.Should().Be(3);
    }
}
