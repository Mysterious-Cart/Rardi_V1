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

        protected IEnumerable<CHKS.Models.mydb.History> History = [];
        protected IEnumerable<CHKS.Models.mydb.Historyconnector> Historyconnectors = [];
        protected IEnumerable<Models.mydb.Dailyexpense> Dailyexpenses = [];
        protected IEnumerable<Models.mydb.Inventory> Inventories = [] ;

        protected DateOnly dates = DateOnly.FromDateTime(DateTime.Today);
        protected bool NoEmptyImport = true;
        protected bool changeDataMode = false;

        protected decimal Revenue = 0;
        protected decimal Total = 0;
        protected decimal ImportTotal = 0;
        protected decimal ExpenseTotal = 0;

        protected string TotalBaht = "0";
        protected string TotalDollar="0";
        protected string TotalRiel= "0";
        protected string TotalAba = "0";

        protected RadzenDataGrid<CHKS.Models.mydb.Historyconnector> grid1;
        protected RadzenDataGrid<CHKS.Models.mydb.History> grid0;
        protected RadzenDataGrid<Models.mydb.Dailyexpense> grid2;

        List<DailyRevenue> Dailyrevenues = [];

        protected override async Task OnInitializedAsync()
        {
            await Task.Run(async () => {
                History = await mydbService.GetHistories();
            }).ContinueWith(async (i) => {
                Dailyexpenses = await mydbService.GetDailyexpenses();
                await GetGraphData();
            });
            Revenue = Total - (ImportTotal + ExpenseTotal);
            changeDataMode = false;
        }

        protected async Task GetGraphData(){
            char[] seperator = {':','('};
            History = History.Take(10);
            foreach(var Record in History){
                string[] cashoutdate = Record.CashoutDate.Split(seperator);
                var inList = Dailyrevenues.Where(i => i.Date == cashoutdate[0]);
                if(inList.Any()){
                    inList.First().Total += Record.Total.GetValueOrDefault();
                }else{
                    DailyRevenue revenue = new(){
                        Date = cashoutdate[0],
                        Total = Record.Total.GetValueOrDefault()
                    };

                    Dailyrevenues.Add(revenue);
                }
                
            }
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