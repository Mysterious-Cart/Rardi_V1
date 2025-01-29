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
using CHKS.Services;

namespace CHKS.Pages
{
    public partial class HandleService
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

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<Models.mydb.Historyconnector> HistoryProduct;

        protected string TotalService = "0";

        protected async override Task OnInitializedAsync()
        {
            await GetServiceOnly();
            await GetServiceTotal();
        }

        protected  async Task GetServiceOnly(){
            HistoryProduct = await MydbService.GetHistoryconnectors();
            HistoryProduct = HistoryProduct.Where(i => i.History.CashoutDate.Contains(DateTime.Now.ToString("MM/yyyy")));
        }

        protected async Task GetServiceTotal(){
            if(HistoryProduct != null || HistoryProduct != Enumerable.Empty<Models.mydb.Historyconnector>()){
                TotalService = Math.Round(HistoryProduct.Sum(i => i.Export * i.Qty),2).ToString() + " $";
            }
        }
    }

}