using Microsoft.AspNetCore.Components;
using CHKS.Services;
using System.Linq.Dynamic.Core;
using MudBlazor;
using CHKS.Entity;
using CHKS.Pages.Component;

namespace CHKS.Pages
{
    public partial class Inventories : IAsyncDisposable
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected IDialogService MudDialogService { get; set; }

        [Inject]
        protected InventoryControlService InventoryControlService { get; set; }

        [Parameter]
        public string ProductIdParam { get; set; } = "";
        [Parameter]
        public Tag Tag { get; set; } = null;

        private IEnumerable<Product> _inventories = [];
        private IEnumerable<Product> inventories = [];
        private MudDataGrid<Product> productdatagrid;
        private Product Product;

        protected override async Task OnInitializedAsync()
        {
            await GetProductFromInventory();

            GetUriQuery();
            if (!string.IsNullOrEmpty(ProductIdParam))
            {
                Select_Product(inventories.First(i => i.Id == Guid.Parse(ProductIdParam)));
            }
            /*
            if (Tag is not null)
            {
                _inventories = _inventories
                                .Where(i => i.Tags.Contains(Tag));
                inventories = _inventories;
            }*/
        }

        private async Task GetProductFromInventory()
        {
            var query_product =
                await InventoryControlService.GetProductList();
            _inventories = query_product;
            inventories = _inventories;
        }

        private string search = "";
        private async Task Search(string args)
        {
            search = args;
            inventories = _inventories
                            .AsParallel()
                            .Where(i => i.Name.Contains(args))
                            .OrderByDescending(i => i.Stock);
        }

        private void GetUriQuery()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParam = System.Web.HttpUtility.ParseQueryString(uri.Query);
            ProductIdParam = queryParam["ProductIdParam"];
        }

        private void Select_Product(Product ChoosenProduct) => Product = ChoosenProduct;
        
        private bool isEditing = false;
        private IDialogReference ModifyProduct;
        private async Task StartedEditingItem(Product item)
        {
            if (isEditing) return;
            isEditing = true;
            ModifyProduct = await MudDialogService
                .ShowAsync<CreateProduct>("Modify Product",
                    new DialogParameters
                    {
                        ["Mode"] = sbyte.Parse("1"),
                        ["_product"] = item
                    },
                    new DialogOptions
                    {
                        FullWidth = true,
                        MaxWidth = MaxWidth.Small,
                        BackdropClick = false,
                        CloseButton = true,
                        CloseOnEscapeKey = true,
                        
                    });

            var result = await ModifyProduct.Result;
            if (result.Data is not null && result.Data.Equals(true))
            {
                await GetProductFromInventory();
                await Search(search);

            }
            isEditing = false;

        }

        private IDialogReference NewProduct;
        private async Task CreateNewProduct()
        {
            NewProduct = await MudDialogService
                    .ShowAsync<CreateProduct>("New Product",
                    new DialogOptions
                    {
                        
                        FullWidth = true,
                        BackdropClick = false,
                        CloseButton = true,
                        CloseOnEscapeKey = true
                    });
        }
        public async ValueTask DisposeAsync()
        {            
            productdatagrid.Dispose();

            GC.SuppressFinalize(this);
            GC.Collect();
        }
    }
}