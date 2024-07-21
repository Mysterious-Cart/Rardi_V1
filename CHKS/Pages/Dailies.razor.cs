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
using System.Globalization;

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
        protected IEnumerable<Models.mydb.Cashback> Cashbacks;
        protected IEnumerable<Models.mydb.Dailyexpense> Dailyexpenses;
        protected CHKS.Models.mydb.Inventory Inventories;

        protected static string dates = DateTime.Now.ToString("dd/MM/yyyy");
        protected bool NoEmptyImport = true;
        protected bool changeDataMode = false;

        protected string Total = "0";
        protected string ExpenseTotal = "0";

        protected RadzenDataGrid<CHKS.Models.mydb.Historyconnector> grid1;
        protected RadzenDataGrid<CHKS.Models.mydb.History> grid0;
        protected RadzenDataGrid<Models.mydb.Dailyexpense> grid2;
        protected RadzenDataGrid<Models.mydb.Cashback> grid3;

        protected override async Task OnInitializedAsync()
        {
            Cashbacks = await mydbService.GetCashbacks();
            Cashbacks = Cashbacks.Where(i => i.Key.Contains(DateTime.Now.ToString("dd/MM/yyyy")));
            Dailyexpenses = await mydbService.GetDailyexpenses();
            Dailyexpenses = Dailyexpenses.Where(i => i.Key.Contains(DateTime.Now.ToString("dd/MM/yyyy")));
            History = await mydbService.GetHistories();
            History = History.Where(i => LikeOperator.LikeString(i.CashoutDate, dates + "*", Microsoft.VisualBasic.CompareMethod.Text));
            Historyconnectors = await mydbService.GetHistoryconnectors();
            changeDataMode = false;
            GetProductWithoutImport();
            await GetAllNumberForToday();
        }

        protected async Task GetAllNumberForToday(){
            Total = History.Sum(i => i.Total).ToString() + " $";
            ExpenseTotal = Dailyexpenses.Sum(i => i.Expense).ToString() + " $";
        }

        private CultureInfo DollarSign = new CultureInfo("us-US");
        private CultureInfo BahtSign = new CultureInfo("kh-KH");
        private CultureInfo RielSign = new CultureInfo("th-TH");
        protected string PaymentType;
        protected async Task GetPaymentTypeOfHistory(){
            foreach(var Customer in History){
                if(Customer.Baht != 0){
                    
                }

                if(Customer.Riel != 0){
                    
                }

                if(Customer.Dollar != 0){
                    
                }

                if(Customer.Bank != 0){
                    
                }
            }
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

        [Inject]
        protected SecurityService Security { get; set; }

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
            await mydbService.CancelInventoryChanges(Inventories);
        }

        protected async Task AddBtnClick(MouseEventArgs args)
        {
            await grid2.InsertRow(new Models.mydb.Dailyexpense());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Dailyexpense data){
            try{
                if(await DialogService.Confirm("Are you sure?", "Important!", new ConfirmOptions{OkButtonText="Yes", CancelButtonText="No"})== true){
                    await mydbService.DeleteDailyexpense(data.Key);
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
                data.Key = DateTime.Now.ToString("dd/MM/yyyy") + ":" + data.Note;
                await mydbService.CreateDailyexpense(data);
                await grid2.Reload();
            }catch(Exception exc){
                if(exc.Message =="Item already available"){
                    await DialogService.Alert("Apology, This expense already listed. Please choose a different name or update the already listed expense.","Important");
                    await grid2.Reload();
                }
            }
        }

        protected async Task AddBtnClickCashback(MouseEventArgs args)
        {
            await grid3.InsertRow(new Models.mydb.Cashback());
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Dailyexpense args)
        {
            await mydbService.UpdateDailyexpense(args.Key, args);
        }


        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Cashback data){
            try{
                if(await DialogService.Confirm("Are you sure?", "Important!", new ConfirmOptions{OkButtonText="Yes", CancelButtonText="No"})== true){
                    await mydbService.DeleteCashback(data.Key);
                    await grid3.Reload();
                }
            }catch(Exception exc){
                
            }
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Cashback data)
        {
            await grid3.EditRow(data);

        }


        protected async Task GridCreate(Models.mydb.Cashback data){
            try{
                data.Key = DateTime.Now.ToString("dd/MM/yyyy") + ":" + data.Name;
                await mydbService.CreateCashback(data);
                await grid3.Reload();
            }catch(Exception exc){
                if(exc.Message =="Item already available"){
                    await DialogService.Alert("Apology, This expense already listed. Please choose a different name or update the already listed expense.","Important");
                    await grid3.Reload();
                }
            }
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Cashback args)
        {
            await mydbService.UpdateCashback(args.Key, args);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Cashback data)
        {
            await grid3.UpdateRow(data);

        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Cashback data)
        {
            grid3.CancelEditRow(data);
            await mydbService.CancelCashbackChanges(data);
        }

    }
}