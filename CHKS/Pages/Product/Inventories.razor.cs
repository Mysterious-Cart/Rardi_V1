using CHKS.Models;
using CHKS.Models.mydb;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;

namespace CHKS.Pages
{
    public partial class Inventories
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected MudBlazor.IDialogService DialogService { get; set; }

        [Inject]
        protected mydbService mydbService { get; set; }

        [Inject]
        protected IDbProvider _dbProvider { get; set; }

        [Parameter]
        public string ProductIdParam { get; set; } = "";

        private List<Inventory> _inventories = [];
        private IEnumerable<Inventory> inventories = [];
        private RadzenDataGrid<Inventory> grid0;

        private Inventory Product;
        private int TotalSale = 0;
        private decimal Revenue = 0;
        private decimal Acquisition = 0;


        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await GetProductFromInventory();
            GetUriQuery();
            if (!string.IsNullOrEmpty(ProductIdParam))
            {
                Select_Product(inventories.Where(i => i.Id == Guid.Parse(ProductIdParam)).First());
            }
        }

        private async Task GetProductFromInventory()
        {
            var query_product = await _dbProvider.GetData<Inventory>([nameof(Inventory.HistoryConnectors), nameof(Inventory.Tags)]);
            _inventories = query_product.OrderByDescending(i => i.Sold_Total).AsEnumerable().ToList();
            inventories = _inventories;

        }

        private async Task Search(ChangeEventArgs args)
        {
            inventories = _inventories.AsParallel().Where(i => i.Name.Contains(args.Value.ToString()));

        }

        private void GetUriQuery()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParam = System.Web.HttpUtility.ParseQueryString(uri.Query);
            ProductIdParam = queryParam["ProductIdParam"];
        }

        private void Select_Product(Inventory ChoosenProduct)
        {
            Product = ChoosenProduct;
            TotalSale = ChoosenProduct.GetSoldTotal();
            Revenue = ChoosenProduct.HistoryConnectors?.Sum(i => i.Export * i.Qty) ?? 0;
            Acquisition = ChoosenProduct.Stock * ChoosenProduct.Import;
        }

        protected bool IsCombiningMode = false;
        protected List<Inventory> ChosenProduct;

        protected async Task CombineProduct()
        {
            IsCombiningMode = !IsCombiningMode;
            ChosenProduct = new();
            if (IsCombiningMode == false)
            {
                await Toasting("ប៉ះបង់បានសម្រេច");
            }
        }

        [Obsolete]
        protected async Task RewriteHistory(Models.mydb.Inventory NewProduct, List<Guid> OldProductCode)
        {
            foreach (Guid i in OldProductCode)
            {
                IEnumerable<Models.mydb.Historyconnector> historyconnectors = await mydbService.GetHistoryconnectors();
                historyconnectors = historyconnectors.Where(b => b.Inventory.Id == i);
                foreach (var f in historyconnectors)
                {
                    f.ProductId = NewProduct.Id;
                    await mydbService.UpdateHistoryconnector(f.Id, f);
                }
            }

        }

        protected string ToastState = "";
        protected string ToastString = "";
        protected async Task Toasting(string ToastText)
        {
            ToastState = "show";
            ToastString = ToastText;
            await Task.Delay(300);
            ToastState = "";
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new Inventory());

        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Inventory args)
        {
            await mydbService.UpdateInventory(args);
        }
        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Inventory data)
        {
            await grid0.EditRow(data);

        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Inventory data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Inventory data)
        {
            grid0.CancelEditRow(data);
            await mydbService.CancelInventoryChanges(data);
        }

    }
}