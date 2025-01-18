using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using CHKS.Pages.Component;
using CHKS.Models.mydb;
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

        private IEnumerable<CHKS.Models.mydb.History> History = [];
        private IEnumerable<CHKS.Models.mydb.Historyconnector> Historyconnectors = [];
        private IEnumerable<Models.mydb.Dailyexpense> Dailyexpenses = [];
        private IEnumerable<Models.mydb.Inventory> Inventories = [] ;

        private DateOnly StartingDate = new(DateTime.Now.Year, DateTime.Now.Month, 1);
        private DateOnly EndDate = DateOnly.FromDateTime(DateTime.Today);
        private bool changeDataMode = false;

        private decimal Revenue = 0;
        private decimal Total = 0;
        private decimal ImportTotal = 0;
        private decimal ExpenseTotal = 0;
        private int SoldTotal = 0;

        private string TotalBaht = "0";
        private string TotalDollar="0";
        private string TotalRiel= "0";
        private string TotalAba = "0";

        private RadzenDataGrid<CHKS.Models.mydb.Historyconnector> grid1;
        private RadzenDataGrid<Models.mydb.Dailyexpense> grid2;

        List<DailyRevenue> Dailyrevenues = [];
        List<DailyRevenue> DailyExpenses = [];

        protected override async Task OnInitializedAsync()
        {
            await ToLoad();
        }

        protected async Task ToLoad(){
            await Task.Run(async () =>{
                History = await mydbService.GetHistories(new Query{Expand="Historyconnectors"});
            }).ContinueWith(async (i) => {
                Dailyexpenses = await mydbService.GetDailyexpenses();
            });
        
            await FilterData();
            
        }

        private async Task FilterData(){
            History = History.Where(i => 
                DateOnly.ParseExact(i.CashoutDate.Split('(',2)[0], "dd/MM/yyyy") <= EndDate &&
                DateOnly.ParseExact(i.CashoutDate.Split('(',2)[0],"dd/MM/yyyy") >= StartingDate
            );
            History = History.OrderByDescending(i => i.CashoutDate);
            Dailyexpenses = Dailyexpenses.Where(i => 
                DateOnly.ParseExact(i.Date, "dd/MM/yyyy") <= EndDate && 
                DateOnly.ParseExact(i.Date, "dd/MM/yyyy") >= StartingDate
            );

            Dailyexpenses = Dailyexpenses.OrderByDescending(i => DateOnly.ParseExact(i.Date.Split('(',2)[0], "dd/MM/yyyy"));
            await GetGraphData();
            await GenerateStatisticReport();
        }

        private async Task GenerateStatisticReport(){
            Total = Dailyrevenues.Sum(i => i.Total);
            ImportTotal = History.SelectMany(i => i.Historyconnectors).Sum(i => i.Export * i.Qty);
            ExpenseTotal = DailyExpenses.Sum(i => i.Total);
            Revenue = Total - ImportTotal;
            SoldTotal = (int)History.Sum(i => i.Historyconnectors.Sum(i => i.Qty));
        }

        private async Task AddExpense(){
            Dictionary<string, string> Expense = await DialogService.OpenAsync<NewExpense>("Expense");
            if(Expense is not null){
                await mydbService.CreateDailyexpense(
                    new Dailyexpense(){
                        Note=Expense.First(i => i.Key.Equals("Info")).Value, 
                        Expense = decimal.Parse(Expense.First(i => i.Key.Equals("Cost")).Value)
                });
            }
        }

        private async Task GetGraphData(){
            Dailyrevenues = [];
            DailyExpenses = [];
            foreach(var Record in History){
                var inList = Dailyrevenues.Where(i => i.Date == Record.CashoutDate.Split('(',2)[0]);
                if(inList.Any()){
                    inList.First().Total += Record.Total.GetValueOrDefault();
                }else{
                    DailyRevenue revenue = new(){
                        Date = Record.CashoutDate.Split('(',2)[0],
                        Total = Record.Total.GetValueOrDefault(),
                    };
                    Dailyrevenues.Add(revenue);
                }
            }

            foreach(var Expense in Dailyexpenses){
                var inList = DailyExpenses.Where(i => i.Date == Expense.Date);
                if(inList.Any()){
                    inList.First().Total += Expense.Expense;
                }else{
                    DailyRevenue revenue = new(){
                        Date = Expense.Date,
                        Total = Expense.Expense
                    };

                    DailyExpenses.Add(revenue);
                }
                
            }
            Dailyrevenues = Dailyrevenues.OrderByDescending(i => i.Date).ToList();
        }

        /*

        private async Task RegenerateKey(){
            foreach(var expense in Dailyexpenses.ToList()){
                var key = expense.Key;
                await mydbService.DeleteDailyexpense(key);
                Models.mydb.Dailyexpense NewExpense = new(){
                    Key = Guid.NewGuid(),
                    Note = expense.Note,
                    Date = expense.Date,
                    Expense=  expense.Expense,
                };
                await mydbService.CreateDailyexpense(NewExpense);
            }
        }
        
        private async Task GenerateDate(){
            foreach(var expense in Dailyexpenses.ToList()){
                expense.Date = expense.Key.Split(':',2)[0];
                await mydbService.UpdateDailyexpense(expense.Key, expense);
            }
        }*/

        protected async Task GetHistory(){
            char[] seperator = {':','('};
            List<Models.mydb.History> tempHis = [];
            var historylist = await mydbService.GetHistories();
            foreach(var i in historylist){
                string[] Temp = i.CashoutDate.Split(seperator);
                if(DateTime.ParseExact(Temp[0],"dd/MM/yyyy",null) == DateTime.Today){
                    if(tempHis.Any(i => i.Id == i.Id) == false){ // Temporay fix, Erro: Loop being called more then once
                        tempHis.Add(i);
                    }
                }
            }
            History = tempHis;
        }

        protected string TotalMinusExpense;
        protected async Task GetAllNumberForToday(){
            Total = decimal.Round(History.Sum(i => i.Total).GetValueOrDefault(),2);
            ExpenseTotal = decimal.Round(Dailyexpenses.Sum(i => i.Expense),2);
            TotalAba = decimal.Round(History.Sum(i => i.Bank.GetValueOrDefault()),2).ToString() + "$";
            TotalBaht = decimal.Round(History.Sum(i => i.Baht.GetValueOrDefault()),2).ToString();
            TotalRiel = decimal.Round(History.Sum(i => i.Riel.GetValueOrDefault()),2).ToString("#,##0");
            TotalMinusExpense = (decimal.Round(History.Sum(i => i.Total).GetValueOrDefault(),2) - decimal.Round(Dailyexpenses.Sum(i => i.Expense),2)).ToString() +  " $";
            
        }

        private string SubCardClass = "Statistic-Info-Overview-SubCard-Hide";
        private bool showCashInfo = false;
        private async Task ShowHideCashInfo(){
            if(showCashInfo == true){
                showCashInfo = false;
                SubCardClass = "Statistic-Info-Overview-SubCard-Hide";
            }else{
                showCashInfo=true;
                SubCardClass = "Statistic-Info-Overview-SubCard-Show";
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
            
        }

        protected async Task OpenHistory(CHKS.Models.mydb.History args)
        {
        }

        protected bool errorVisible;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid1.EditRow(data);
        }

         protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            grid1.CancelEditRow(data);
            await mydbService.CancelInventoryChanges(Inventories.FirstOrDefault());
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
                
            }finally{
                await GetAllNumberForToday();
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
                await GetAllNumberForToday();
                await grid2.Reload();
            }catch(Exception exc){
                if(exc.Message =="Item already available"){
                    await DialogService.Alert("Apology, This expense already listed. Please choose a different name or update the already listed expense.","Important");
                    await grid2.Reload();
                }
            }
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Dailyexpense args)
        {
            await mydbService.UpdateDailyexpense(args.Key, args);
            await GetAllNumberForToday();
        }

    }
}

public class DailyRevenue{
    public string Date {get; set;}
    public decimal Total {get; set;}
}