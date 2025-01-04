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

        [Inject]
        protected PublicCommand PublicCommand {get; set;}

        [Parameter]
        public string IsDialog {get; set;}

        protected IEnumerable<CHKS.Models.mydb.Inventory> inventories;
        protected IEnumerable<Models.mydb.CarBrand> CarBrands;
        protected RadzenDataGrid<CHKS.Models.mydb.Inventory> grid0;

        protected string search = "";
        protected bool isModifying = false;
        protected bool isEditMode = false;

        protected string OriginalName;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task CreateNewCar(){
            Models.mydb.CarBrand car = new();
        }


        protected async Task Search(ChangeEventArgs args)
        {
            
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            inventories = await mydbService.GetInventories(new Query { Filter = $@"i =>( i.Name.Contains(@0) || i.Barcode.Contains(@0)) && i.IsDeleted == 0 ", FilterParameters = new object[] { search} });
        }

        protected override async Task OnInitializedAsync()
        {   
            inventories = await mydbService.GetInventories(new Query { Filter = $@"i =>( i.Name.Contains(@0) || i.Barcode.Contains(@0)) && i.IsDeleted == 0", FilterParameters = new object[] { search} }); 
            CarBrands = await mydbService.GetCarBrands();
        }

        protected bool IsCombiningMode = false;
        protected List<Models.mydb.Inventory> ChosenProduct;

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

        protected async Task SelectProduct(Models.mydb.Inventory product){
            if(IsCombiningMode == true){
                bool Dupe = false;
                foreach(var i in ChosenProduct){
                    if(i.Id == product.Id){ 
                        Dupe = true;
                        break;
                    }
                }
                if( Dupe == false){
                    ChosenProduct.Add(product);
                    await Toasting("បានចាប់យក");
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
            await grid0.InsertRow(new Inventory(){Name = ""});
            
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
                isModifying = false;   
            }
        }

        

        protected async Task GridRowUpdate(CHKS.Models.mydb.Inventory args)
        {
            await mydbService.UpdateInventory(args);
            isModifying = false;

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