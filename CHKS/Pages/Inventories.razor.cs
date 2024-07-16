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
        public mydbService mydbService { get; set; }

        [Parameter]
        public string IsDialog {get; set;}

        protected IEnumerable<CHKS.Models.mydb.Inventory> inventories;

        protected RadzenDataGrid<CHKS.Models.mydb.Inventory> grid0;

        protected string search = "";
        protected bool isModifying = false;
        protected bool isEditMode = false;

        protected string OriginalName;

        [Inject]
        protected SecurityService Security { get; set; }

        private string Barcode;
        protected async Task getBar(KeyboardEventArgs keyboard){
            if(IsDialog == "true"){
                if(keyboard.Key != "Enter" && keyboard.CtrlKey == false && keyboard.ShiftKey == false && keyboard.AltKey == false ){
                    Barcode += keyboard.Key;
                }else if(keyboard.CtrlKey == false && keyboard.ShiftKey == false && keyboard.AltKey == false ){
                    if(keyboard.Key == "Enter" && Barcode != ""){
                        if(inventories.Any() == true && inventories.FirstOrDefault().Barcode == Barcode){
                            await SelectProduct(inventories.FirstOrDefault());
                            Barcode = "";
                        }else if( inventories.Any() == false){
                            await DialogService.Alert("ទំនេញមិនមាន។", "សំខាន់");
                        }
                    }
                }
            }
            
        }


        protected async Task Search(ChangeEventArgs args)
        {
            
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            inventories = await mydbService.GetInventories(new Query { Filter = $@"i => i.Name.Contains(@0) && i.Name != (@1) || i.Barcode.Contains(@0) ", FilterParameters = new object[] { search , "Service Charge"} });
        }
        protected override async Task OnInitializedAsync()
        {   
            inventories = await mydbService.GetInventories(new Query { Filter = $@"i => i.Name.Contains(@0) && i.Name != (@1) || i.Barcode.Contains(@0)", FilterParameters = new object[] { search , "Service Charge"} }); 
        }

        RadzenTextBox searchbar;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(isEditMode == false && isModifying == false ){
                await searchbar.Element.FocusAsync();
            }
        }

        protected async Task SelectProduct(CHKS.Models.mydb.Inventory Product){
            if(Product.Stock!=0 && IsDialog == "true" && isModifying == false){
                var Qty = await DialogService.OpenAsync<SingleInputPopUp>(Product.Name, new Dictionary<string, object>{{"Info",new string[]{"Qty", Product.Export.ToString()}}}, new DialogOptions{Width="13%"});

                if(Qty is Array && Qty[0] != null && Qty[0] is decimal && Qty[0] <= Product.Stock){
                    Models.mydb.Inventory Temp = new(){};
                    Product.Stock -=  Qty[0];
                    await mydbService.UpdateInventory(Product.Name, Product);
                    Temp.Export = Qty[1];
                    Temp.Import = Qty[2];
                    Temp.Name = Product.Name;
                    Temp.Stock = Qty[0];
                    
                    DialogService.Close(Temp);
                }else if(Qty is Array && Qty[0] != null && Qty[0] > Product.Stock){
                    await DialogService.Alert("Too much! Not available In stock. Have Left: " + Product.Stock + ".","Warning");
                    await SelectProduct(Product);
                }
            }else if(Product.Stock == 0 && isModifying == false){
                await DialogService.Alert("Not available In stock");
            }else if(IsDialog == "false"){}
            
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isModifying = true;
            await grid0.InsertRow(new CHKS.Models.mydb.Inventory());
            
        }

        protected async Task GridDeleteButtonClick( CHKS.Models.mydb.Inventory inventory)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure?") == true)
                {
                    Models.mydb.InventoryTrashcan trashed = new(){
                        Date = DateTime.Now.ToString(),
                        Name = inventory.Name,
                        Barcode = inventory.Barcode,
                        Stock = inventory.Stock,
                        Import = inventory.Import,
                        Export = inventory.Export,
                    };
                    await mydbService.CreateInventoryTrashcan(trashed);
                    var deleteResult = await mydbService.DeleteInventory(inventory.Name);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                        isModifying = false;
                    }
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
            if(args.Name != OriginalName){
                Models.mydb.Inventory Checking = await mydbService.GetInventoryByName(args.Name);
                if(Checking == null){
                    string tempname = args.Name;
                    args.Name = OriginalName;
                    if(args.Import == null){
                        args.Import = 0;
                    }
                    await GridDeleteButtonClick(args);
                    try{
                        args.Name = tempname;
                        await mydbService.CreateInventory(args);
                        isEditMode = false;
                    }catch(Exception exc){

                    }
                }else{
                    await DialogService.Alert("The name you trying to change to already exist.","Important");
                    args.Name = OriginalName;
                    grid0.CancelEditRow(args);
                    await grid0.Reload();
                    isModifying = false;
                }

            }else{
                await mydbService.UpdateInventory(args.Name,args);
                isModifying = false;
            }

        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Inventory args)
        {
            try{
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
            OriginalName = data.Name;
            isModifying = true;
            await grid0.EditRow(data);
            isEditMode = true;

        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Inventory data)
        {
            isModifying = false;
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Inventory data)
        {
            isModifying = false;
            grid0.CancelEditRow(data);
            await mydbService.CancelInventoryChanges(data);
        }
        
    }
}