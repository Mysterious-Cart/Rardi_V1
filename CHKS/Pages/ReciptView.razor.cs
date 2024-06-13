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
    public partial class ReciptView
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
        protected mydbService MydbService {get; set;}

        protected IEnumerable<CHKS.Models.mydb.Historyconnector> Historyconnector;
        protected IEnumerable<CHKS.Models.mydb.Inventory> Inventory;

        protected RadzenDataGrid<CHKS.Models.mydb.Historyconnector> grid0;

        protected string Date;
        protected string CustomerID;
        protected bool Isloading = true;

        [Parameter]
        public string ID {get; set;}

        protected override async Task OnInitializedAsync()
        { 
            Historyconnector = await MydbService.GetHistoryconnectors();
            Inventory = await MydbService.GetInventories();
            GettingHistory();
        }

        protected CHKS.Models.mydb.Inventory Inventories;

        protected void GettingHistory(){
            Historyconnector = Historyconnector.Where(i => i.CartId == ID);
            
        }
        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid0.EditRow(data);
            Inventories = await MydbService.GetInventoryByName(data.Product);
        }

         protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid0.UpdateRow(data);
            await MydbService.UpdateInventory(data.Product, Inventories);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            grid0.CancelEditRow(data);
            await MydbService.CancelHistoryconnectorChanges(data);
            await MydbService.CancelInventoryChanges(Inventories);
        }

    }
}