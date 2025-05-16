using CHKS.Models.Enum;
using CHKS.Entity;
using CHKS.Extras.Class.DTOs;
using CHKS.Models.mydb;

namespace CHKS.Models.Builder
{
    public class ProductBuilder : IEntityBuilder<Product, Product_Model>
    {
        private Guid _id;
        private int _stock;
        private decimal _importPrice;
        private decimal _exportPrice;
        private string _productName;
        private string _normalizedName;
        private List<TagsDTO> _tags;
        private ProductSettingBuilder _settingBuilder = ProductSettingBuilder.Default();

        public static ProductBuilder Empty()
        {
            return new ProductBuilder();
        }

        public static ProductBuilder Modify(Product product)
        {
            return new ProductBuilder()
                .SetId(product.Id)
                .WithName(product.Name)
                .WithStock(product.Stock)
                .WithImportCost(product.Import)
                .PriceAt(product.Export)
                .Setting(setting => setting
                    .SetAllowTracking(product.Setting.AllowTracking)
                    .SetAllowWarning(product.Setting.AllowWarning)
                );
        }

        public static IEntityBuilder<Product, Product_Model> FromModel(Product_Model product)
        {
            return new ProductBuilder()
                .SetId(product.Id)
                .WithName(product.Name)
                .WithStock(product.Stock)
                .WithImportCost(product.Import)
                .PriceAt(product.Export)
                .Setting(setting => setting
                    .SetAllowTracking(product.AllowTracking)
                    .SetAllowWarning(product.AllowWarning)
                );
        }
        public static Product_Model ToModel(Product product)
        {
            return new Product_Model
            {
                Id = product.Id,
                Name = product.Name,
                Stock = product.Stock,
                Import = product.Import,
                Export = product.Export,
                Status = product.Status,
                AllowTracking = product.Setting.AllowTracking,
                AllowWarning = product.Setting.AllowWarning,
            };
        }
        public ProductBuilder SetId(Guid Id)
        {
            _id = Id;
            return this;
        }

        public ProductBuilder WithName(string name)
        {
            _productName = name;
            return this;
        }

        public ProductBuilder WithStock(int stock)
        {
            _stock = stock;
            return this;
        }

        public ProductBuilder WithImportCost(decimal cost)
        {
            _importPrice = cost;
            return this;
        }

        public ProductBuilder SetNormalizedName(string name)
        {
            _normalizedName = name;
            return this;
        }

        public ProductBuilder SetTags(List<TagsDTO> Tags)
        {
            _tags = Tags;
            return this;
        }

        public ProductBuilder PriceAt(decimal price)
        {
            _exportPrice = price;
            return this;
        }

        public ProductBuilder Setting(Action<ProductSettingBuilder> setting)
        {
            setting(_settingBuilder);
            return this;
        }

        public Product Build()
        {
            var setting_module = _settingBuilder.Build();
            Product product = new(_id, _productName, _stock, _importPrice, _exportPrice, ProductStatus.Active
            , setting_module.AllowTracking, setting_module.AllowWarning);

            return product;
        }
    }
}