using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
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

            inventories = await mydbService.GetInventories(new Query { Filter = $@"i =>( i.Name.Contains(@0) || i.Barcode.Contains(@0)) && i.IsDeleted == 0 ", FilterParameters = new object[] { search , "Service Charge"} });
        }

        protected override async Task OnInitializedAsync()
        {   
            inventories = await mydbService.GetInventories(new Query { Filter = $@"i =>( i.Name.Contains(@0) || i.Barcode.Contains(@0)) && i.IsDeleted == 0", FilterParameters = new object[] { search , "Service Charge"} }); 
            CarBrands = await mydbService.GetCarBrands();
        }

        protected bool IsCombiningMode = false;
        protected List<Models.mydb.Inventory> ChosenProduct;

        protected async Task CombineProduct(){
            IsCombiningMode = !IsCombiningMode;
            ChosenProduct = new();
            if(IsCombiningMode == false){await Toasting("ប៉ះបង់បានសម្រេច");}
        }

        protected async Task CombiningModeCombine(){
            List<string> product = new();
            product.Add("Combine");
            foreach(var i in ChosenProduct){
                product.Add(i.Code);
            }
            string[] productArray = product.ToArray();
            Models.mydb.Inventory ChoseProduct = await DialogService.OpenAsync<SingleInputPopUp>("បញ្ចូល", new Dictionary<string, object>{{"Info",productArray}}, new DialogOptions{Width="fit-content", Height="max-content"});
            if(ChosenProduct != null){
                product.Remove("Combine");
                product.Remove(ChoseProduct.Code);
                await RewriteHistory(ChoseProduct, product);
            }
        } 

        protected async Task RewriteHistory(Models.mydb.Inventory NewProduct, List<string> OldProductCode){
            foreach(string i in OldProductCode){
                await PublicCommand.PseudoDeleteInventory(await mydbService.GetInventoryByCode(i));
                IEnumerable<Models.mydb.Historyconnector> historyconnectors = await mydbService.GetHistoryconnectors();
                historyconnectors = historyconnectors.Where(b => b.Inventory.Code == i);
                foreach(var f in historyconnectors){
                    f.Product = NewProduct.Code;
                    await mydbService.UpdateHistoryconnector(f.Id, f);
                }
            }
            
        }

        protected async Task SelectProduct(Models.mydb.Inventory product){
            if(IsCombiningMode == true){
                bool Dupe = false;
                foreach(var i in ChosenProduct){
                    if(i.Code == product.Code){ 
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
            await grid0.InsertRow(new CHKS.Models.mydb.Inventory());
            
        }

        protected async Task GridDeleteButtonClick( CHKS.Models.mydb.Inventory inventory)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure?") == true)
                {
                    inventory.IsDeleted = 1;
                    inventory.Info = "Deleted By:" + Security.User?.Name + "("+ DateTime.Now.ToString() +")";
                    await mydbService.UpdateInventory(inventory.Name, inventory);

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
            await mydbService.UpdateInventory(args.Code,args);
            isModifying = false;

        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Inventory args)
        {
            try{
                args.NormalizeName = args.Name.Trim().ToLower();
                args.Code = await GetKey();
                await mydbService.CreateInventory(args);
                await grid0.Reload();
            }catch(Exception exc){
                if(exc.Message =="Item already available"){
                    await DialogService.Alert("Apology, This product already exist. Please choose a different name or update the already existed product.","Important");
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

        protected async Task<string> GetKey(){
            string GenKey = PublicCommand.GenerateRandomKey();
            IEnumerable<Models.mydb.Inventory> Temp  = inventories.Where(i => i.Code == GenKey);
            return Temp.Any() == false?GenKey: await GetKey();
        }          
        
    }
}