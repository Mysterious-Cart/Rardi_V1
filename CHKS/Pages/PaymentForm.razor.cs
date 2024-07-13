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
    public partial class PaymentForm
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

        [Parameter]
        public decimal Total{get; set;}

        protected decimal? Bank = 0;
        protected decimal? Dollar = 0;
        protected decimal? Baht = 0;
        protected decimal? Riel = 0;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task CashOut(){
            DialogService.Close(new List<decimal?>{Bank,Dollar,Baht,Riel});
        }
    }
}