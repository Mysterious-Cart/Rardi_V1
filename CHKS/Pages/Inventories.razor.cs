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


        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            inventories = await mydbService.GetInventories(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } });
        }
        protected override async Task OnInitializedAsync()
        {
            inventories = await mydbService.GetInventories(new Query { Filter = $@"i => i.Name.Contains(@0)", FilterParameters = new object[] { search } }); 
        }

        protected async Task SelectProduct(CHKS.Models.mydb.Inventory Product){
            if(Product.Stock!=0 && IsDialog == "true" && isModifying == false){
                int? Qty = await DialogService.OpenAsync<SingleInputPopUp>("Qty", new Dictionary<string, object>{{"Title","Qty"}});

                if(Qty != null && Qty <= Product.Stock){
                    Models.mydb.Inventory Temp = new Models.mydb.Inventory{};
                    Product.Stock = Product.Stock - Qty.GetValueOrDefault(1);
                    await mydbService.UpdateInventory(Product.Name, Product);
                    Temp.Export = Product.Export;
                    Temp.Import = Product.Import;
                    Temp.Name = Product.Name;
                    Temp.Stock = Qty.GetValueOrDefault(1);
                    DialogService.Close(Temp);
                }else if(Qty != null && Qty > Product.Stock){
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

        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Inventory inventory)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
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
            await mydbService.UpdateInventory(args.Name, args);
        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Inventory args)
        {
            await mydbService.CreateInventory(args);
            await grid0.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Inventory data)
        {
            isModifying = true;
            await grid0.EditRow(data);
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