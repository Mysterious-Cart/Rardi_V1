using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.VisualBasic.CompilerServices;
using Radzen;
using Radzen.Blazor;

namespace CHKS.Pages
{
    public partial class Dailies
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

        protected IEnumerable<CHKS.Models.mydb.History> History;
        protected IEnumerable<CHKS.Models.mydb.Historyconnector> Historyconnectors;
        protected IEnumerable<Models.mydb.Dailyexpense> Dailyexpenses;
        protected CHKS.Models.mydb.Inventory Inventories;

        protected static string dates = DateTime.Now.ToString("dd/MM/yyyy");
        protected bool NoEmptyImport = true;
        protected bool changeDataMode = false;

        protected string Total = "0";
        protected string ProductTotal = "0";
        protected string ServiceTotal = "0";
        protected string ExpenseTotal = "0";

        protected RadzenDataGrid<CHKS.Models.mydb.Historyconnector> grid1;
        protected RadzenDataGrid<CHKS.Models.mydb.History> grid0;
        protected RadzenDataGrid<Models.mydb.Dailyexpense> grid2;

        protected override async Task OnInitializedAsync()
        {
            Dailyexpenses = await mydbService.GetDailyexpenses();
            History = await mydbService.GetHistories();
            History = History.Where(i => LikeOperator.LikeString(i.CashoutDate, dates + "*", Microsoft.VisualBasic.CompareMethod.Text));
            Historyconnectors = await mydbService.GetHistoryconnectors();
            changeDataMode = false;
            GetProductWithoutImport();
        }

        protected async void reload(){
        }

        protected async Task LoadNotImport(){
            if(changeDataMode==false ){
                changeDataMode = true;
            }else{
                changeDataMode = false;
            }
        }

        protected void GetProductWithoutImport(){
            Historyconnectors = Historyconnectors.Where(i => i.CartId.Contains(dates) && i.Inventory.Import == 0 && i.Inventory.Name != "Service Charge");
            if(Historyconnectors.Any()){
                NoEmptyImport = false;
            }else{
                NoEmptyImport = true;
            }
        }

        protected async Task OpenHistory(CHKS.Models.mydb.History args)
        {
            await DialogService.OpenAsync<ReciptView>("Customer", new Dictionary<string, object>{{"ID",args.CashoutDate}}, new DialogOptions{Width="60%",Height="80%"});
        }

        protected bool errorVisible;

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid1.EditRow(data);
            Inventories = await mydbService.GetInventoryByName(data.Product);
        }

         protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid1.UpdateRow(data);
            await mydbService.UpdateInventory(data.Product, Inventories);
            if(Historyconnectors.Any()){
                NoEmptyImport = false;
            }else{
                NoEmptyImport = true;
                changeDataMode = false;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            grid1.CancelEditRow(data);
            await mydbService.CancelHistoryconnectorChanges(data);
            await mydbService.CancelInventoryChanges(Inventories);
        }

        protected async Task AddBtnClick(MouseEventArgs args)
        {
            await grid2.InsertRow(new Models.mydb.Dailyexpense());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Dailyexpense data){
            try{
                if(await DialogService.Confirm("Are you sure?", "Important!", new ConfirmOptions{OkButtonText="Yes", CancelButtonText="No"})== true){
                    await mydbService.DeleteDailyexpense(data.Note);
                    await grid2.Reload();
                }
            }catch(Exception exc){
                
            }
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Dailyexpense data)
        {
            await grid2.EditRow(data);

        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Dailyexpense data)
        {
            await grid2.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Dailyexpense data)
        {
            grid2.CancelEditRow(data);
            await mydbService.CancelDailyexpenseChanges(data);
        }
        protected async Task GridCreate(Models.mydb.Dailyexpense data){
            try{
                await mydbService.CreateDailyexpense(data);
                await grid2.Reload();
            }catch(Exception exc){
                if(exc.Message =="Item already available"){
                    await DialogService.Alert("Apology, This product already exist. Please choose a different name or update the already existed product.","Important");
                    await grid0.Reload();
                }
            }
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Dailyexpense args)
        {
            await mydbService.UpdateDailyexpense(args.Note, args);
        }

    }
}