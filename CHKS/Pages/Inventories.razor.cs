using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using CHKS.Models.mydb;
using CHKS.Pages.Component.Popup;

namespace CHKS.Pages
{
    public partial class Inventories
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected mydbService mydbService { get; set; }

        [Parameter]
        public string ProductIdParam {get; set;} = "";

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
            if(!string.IsNullOrEmpty(ProductIdParam)){
                Select_Product(inventories.Where(i => i.Id == Guid.Parse(ProductIdParam)).First());
            }
        }

        private async Task GetProductFromInventory(){
            var query_product = await mydbService.GetInventories(new Query(){Expand="HistoryConnectors, Tags"});
            _inventories = query_product.OrderByDescending(i => i.Sold_Total).AsEnumerable().ToList();
            inventories = _inventories;
            
        }

        private async Task Search(ChangeEventArgs args){
            inventories = _inventories.AsParallel().Where(i => i.Name.Contains(args.Value.ToString()));
            
        }

        private void GetUriQuery(){
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var queryParam = System.Web.HttpUtility.ParseQueryString(uri.Query);
            ProductIdParam = queryParam["ProductIdParam"];
        }
        
        private void Select_Product(Inventory ChoosenProduct){
            Product = ChoosenProduct;
            TotalSale = ChoosenProduct.GetSoldTotal();
            Revenue = ChoosenProduct.HistoryConnectors?.Sum(i => i.Export * i.Qty)??0;
            Acquisition = ChoosenProduct.Stock * ChoosenProduct.Import;
        }

        protected bool IsCombiningMode = false;
        protected List<Inventory> ChosenProduct;
        
        protected async Task CombineProduct(){
            IsCombiningMode = !IsCombiningMode;
            ChosenProduct = new();
            if(IsCombiningMode == false){
                await Toasting("ប៉ះបង់បានសម្រេច");
            }
        }

        protected async Task CombiningModeCombine(){
            List<Guid> product = new();
            foreach(var i in ChosenProduct){
                product.Add(i.Id);
            }
            Guid[] productArray = product.ToArray();
            Models.mydb.Inventory ChoseProduct = await DialogService.OpenAsync<SingleInputPopUp>("បញ្ចូល", new Dictionary<string, object>{{"Info",productArray}}, new DialogOptions{Width="fit-content", Height="max-content"});
            if(ChosenProduct != null){
                product.Remove(ChoseProduct.Id);
                await RewriteHistory(ChoseProduct, product);
            }
        } 
        [Obsolete]
        protected async Task RewriteHistory(Models.mydb.Inventory NewProduct, List<Guid> OldProductCode){
            foreach(Guid i in OldProductCode){
                IEnumerable<Models.mydb.Historyconnector> historyconnectors = await mydbService.GetHistoryconnectors();
                historyconnectors = historyconnectors.Where(b => b.Inventory.Id == i);
                foreach(var f in historyconnectors){
                    f.ProductId = NewProduct.Id;
                    await mydbService.UpdateHistoryconnector(f.Id, f);
                }
            }
            
        }

        protected string ToastState = "";
        protected string ToastString = "";
        protected async Task Toasting(string ToastText){
            ToastState = "show";
            ToastString = ToastText;
            await Task.Delay(300);
            ToastState = "";
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new Inventory());
            
        }

        private async Task EditTags(){
            if(Product is not null){
                await DialogService.OpenAsync<NewTag>("Tags", new Dictionary<string, object>{{"product", Product}}, new DialogOptions{Height="600px",});
                StateHasChanged();
            }
        }

        protected async Task GridDeleteButtonClick( CHKS.Models.mydb.Inventory inventory)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure?") == true)
                {
                    inventory.IsDeleted = 1;
                    inventory.Info = "Deleted By:" + Security.User?.Name + "("+ DateTime.Now.ToString() +")";
                    await mydbService.UpdateInventory(inventory);

                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Inventory"
                    
                });
            }
        }

        

        protected async Task GridRowUpdate(CHKS.Models.mydb.Inventory args)
        {
            await mydbService.UpdateInventory(args);
        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Inventory args)
        {
            try{
                await mydbService.CreateInventory(args);
                await grid0.Reload();
            }catch(Exception exc){
                if(exc.Message =="Item already available"){
                    await DialogService.Alert("This product already exist.","Important");
                    await grid0.Reload();
                }
            }

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