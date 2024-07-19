using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using System.Globalization;

namespace CHKS.Pages
{
    public partial class Statistic
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

        protected IEnumerable<Models.mydb.History> Histories;
        protected IEnumerable<Models.mydb.History> TestHistories;
        protected IEnumerable<Models.mydb.Historyconnector> Historyconnectors;
        protected IEnumerable<Models.mydb.Historyconnector> ProductWithoutImport;
        protected IEnumerable<Models.mydb.Dailyexpense> MonthlyExpense;

        protected RadzenDataGrid<Models.mydb.History> grid1;
        protected RadzenDataGrid<Models.mydb.Dailyexpense> grid2;
        protected RadzenDataGrid<Models.mydb.Historyconnector> grid3;

        protected static DateTime TimeEnd = DateTime.Today;
        protected static DateTime? TimeStart = DateTime.Today.AddDays(-DateTime.Today.Day​ + 1);

        protected TimeSpan DateSelected = TimeEnd.Subtract(TimeStart.GetValueOrDefault().AddDays(-DateTime.Today.Day + 1));

        protected string StringType = "C";
        protected string Culture = "us-US";

        protected decimal? Total;
        protected decimal? ProductTotal = 0;
        protected decimal? ServiceTotal = 0;
        protected decimal? ImportTotal = 0;

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        {    
            await GetHistoryBaseOfChoosenDate();
            await GetMonthlyExpense();  
            await GetProductWithoutImport();

        }

        protected async Task GetMonthlyExpense(){
            MonthlyExpense = await MydbService.GetDailyexpenses();
            List<Models.mydb.Dailyexpense> TempList = new();
            foreach(var Expense in MonthlyExpense.ToList()){
                string[] temp = Expense.Key.Split(":");
                if(TimeStart < DateTime.Parse(temp[0]) && DateTime.Parse(temp[0]) <= TimeEnd){
                    TempList.Add(Expense);
                }
            }

            MonthlyExpense = TempList;

        }

        protected async Task OpenHistory(Models.mydb.History args){
            await DialogService.OpenAsync<ReciptView>("", new Dictionary<string, object>{{"ID",args.CashoutDate}}, new DialogOptions{Width="50%", Height="70%"});
        }  

        protected async Task GetAllNumber(){
            ServiceTotal = 0;
            ProductTotal = 0;
            ImportTotal = 0;
            Historyconnectors = await MydbService.GetHistoryconnectors();
            Total = Histories.Sum(i => i.Total);
            foreach(var history in Histories.ToList()){
                IEnumerable<Models.mydb.Historyconnector> TempHisCon;
                TempHisCon = Historyconnectors;
                TempHisCon = TempHisCon.Where(i => i.CartId == history.CashoutDate);
                ServiceTotal += TempHisCon.Sum(i => i.Product == "Service Charge"?i.Export:0);
                ProductTotal += TempHisCon.Sum(i => i.Product != "Service Charge"?i.Export * i.Qty:0 );
                ImportTotal += TempHisCon.Sum(i => i.Inventory.Import);
            }
        }

        protected async Task GetProductWithoutImport(){
            TimeStart = DateTime.Parse(TimeStart.GetValueOrDefault().ToString("dd/MM/yyyy"));
            TimeEnd = DateTime.Parse(TimeEnd.ToString("dd/MM/yyyy"));
            IEnumerable<Models.mydb.Historyconnector> TempHis = Historyconnectors;
            List<Models.mydb.Historyconnector> TempList = new(); 
            Console.WriteLine(TimeStart);
            foreach(var item in TempHis){
                char[] seperator = {':','('};
                string[] Temp = item.CartId.Split(seperator,2);
                
                if(TimeStart <= DateTime.ParseExact(Temp[0],"dd/MM/yyyy",null) && DateTime.ParseExact(Temp[0],"dd/MM/yyyy",null) <= TimeEnd && item.Inventory.Import == 0 && item.Product != "Service Charge"){
                    TempList.Add(item);
                }
                
            }
            ProductWithoutImport = TempList;
        }

        protected async Task GetHistoryBaseOfChoosenDate(){
            await GetMonthlyExpense();
            List<Models.mydb.History> tempHis = new();
            Histories = Enumerable.Empty<Models.mydb.History>();
            TestHistories = await MydbService.GetHistories();
            foreach(var i in TestHistories.ToList()){
                char[] seperator = {':','('};
                string[] Temp = i.CashoutDate.Split(seperator,2);

                if( TimeStart <= DateTime.ParseExact(Temp[0],"dd/MM/yyyy",null) && DateTime.ParseExact(Temp[0],"dd/MM/yyyy",null) <= TimeEnd){
                    tempHis.Add(i);
                }
            }
            Histories = tempHis;
            
            await GetAllNumber();
            await GetProductWithoutImport();
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid3.EditRow(data);
        }

         protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid3.UpdateRow(data);
            await GetProductWithoutImport();
            await grid3.Reload();
            
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            grid3.CancelEditRow(data);
            await MydbService.CancelHistoryconnectorChanges(data);
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Historyconnector args)
        {
            await MydbService.UpdateHistoryconnector(args.Id, args);
        }

    }
}