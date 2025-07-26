using CHKS.Entity;
using CHKS.Models;
using System.Linq.Expressions;

namespace CHKS.Mappers;

/// <summary>
/// Use in Select queries or Conversion Expression.
/// This class provides expression mappings for converting between Product_Model and Product.
/// It also includes a method to convert CreateProductRequest to Product_Model.
/// </summary>
public static class ProductExpressionMapper
{
    /// <summary>
    /// Converts a Product_Model to a Product.
    /// This expression can be used in LINQ queries to project Product_Model to Product.
    /// </summary>
    /// <example>
    /// <code>
    /// dbContext.DbSet.Select(ProductExpressionMapper.ToProduct);
    /// </code>
    /// </example>
    public static Expression<Func<ProductModel, Product>> ToProduct =>
        product => new Product(
            product.Id,
            product.Name,
            product.Stock,
            product.Import,
            product.Export,
            product.Description,
            product.Status,
            new ProductSetting(
                product.AllowTracking,
                product.AllowWarning,
                null
            )
    );
    /// <summary>
    /// Converts a CreateProductRequest to a Product_Model.
    /// This expression can be used in LINQ queries to project CreateProductRequest to Product_Model
    /// </summary>
    public static Expression<Func<CreateProductRequest, ProductModel>> ToProductModel =>
        product => new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = product.Name,
            Stock = product.Stock,
            Import = product.Import,
            Export = product.Export,
            Description = product.Description,
            Status = product.Status
        };
}


/// <summary>
/// Extension methods for mapping between Product_Model and Product.
/// It also provides a method to convert CreateProductRequest to Product_Model.
/// </summary>
public static class ProductMapper
{
    public static Product ToProduct(this CreateProductRequest request) =>
        new(
            Guid.NewGuid(),
            request.Name,
            request.Stock,
            request.Import,
            request.Export,
            request.Description,
            request.Status,
            request.Setting
        );
    /// <summary>
    /// Converts a Product_Model to a Product.
    /// </summary>
    /// <param name="productModel"></param>
    public static Product ToProduct(this ProductModel productModel) =>
        new(
            productModel.Id,
            productModel.Name,
            productModel.Stock,
            productModel.Import,
            productModel.Export,
            productModel.Description,
            productModel.Status,
            new ProductSetting(
                productModel.AllowTracking,
                productModel.AllowWarning,
                null
            )
        );

    /// <summary>
    /// Converts a CreateProductRequest to a Product_Model.
    /// </summary>
    /// <param name="request"></param>
    public static ProductModel ToProductModel(this CreateProductRequest request) =>
        new()
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Stock = request.Stock,
            Import = request.Import,
            Export = request.Export,
            Description = request.Description,
            Status = request.Status
        };
}