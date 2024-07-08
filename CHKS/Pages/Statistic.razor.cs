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

        protected RadzenDataGrid<Models.mydb.History> grid1;

        protected static DateTime TimeEnd = DateTime.Now;
        protected static DateTime? TimeStart = DateTime.Now.AddDays(-DateTime.Today.Day + 1);

        protected TimeSpan DateSelected = TimeEnd.Subtract(TimeStart.GetValueOrDefault().AddDays(-DateTime.Today.Day + 1));

        protected string StringType = "C";
        protected string Culture = "us-US";

        protected decimal? Total;
        protected decimal? ProductTotal = 0;
        protected decimal? ServiceTotal = 0;

        protected override async Task OnInitializedAsync()
        {    
            await GetHistoryBaseOfChoosenDate();
        }

        protected async Task GetAllNumber(){
            Historyconnectors = await MydbService.GetHistoryconnectors();
            Total = Histories.Sum(i => i.Total);
            foreach(var history in Histories.ToList()){
                IEnumerable<Models.mydb.Historyconnector> TempHisCon;
                TempHisCon = Historyconnectors;
                TempHisCon = TempHisCon.Where(i => i.CartId == history.CashoutDate);
                ServiceTotal += TempHisCon.Sum(i => i.Product == "Service Charge"?i.Export:0);
                ProductTotal += TempHisCon.Sum(i => i.Product != "Service Charge"?i.Export * i.Qty:0 );
            }
        }

        protected async Task GetHistoryBaseOfChoosenDate(){
            List<Models.mydb.History> tempHis = new();
            Histories = Enumerable.Empty<Models.mydb.History>();
            TestHistories = await MydbService.GetHistories();
            foreach(var i in TestHistories.ToList()){
                char[] seperator = {':','('};
                string[] Temp = i.CashoutDate.Split(seperator,2);
                if( TimeStart <= DateTime.Parse(Temp[0]) && DateTime.Parse(Temp[0]) <= TimeEnd){
                    tempHis.Add(i);
                }
            }
            Histories = tempHis;
            await GetAllNumber();
        }

    }
}