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
    public partial class SingleInputPopUp
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

        protected int CartID;
        protected Decimal[] Product;
        protected DateOnly ChosenDate;

        [Parameter]
        public string[] Info {get; set;}

        [Inject]
        protected SecurityService Security { get; set; }

        protected void Close(){
            if(Info[0]=="Cart ID"){
                DialogService.Close(CartID);
            }else if(Info[0]=="Choosing Date"){
                DialogService.Close(ChosenDate.ToString("dd/MM/yyyy"));
            }else if(Info[0] == "Qty") {
                DialogService.Close(Product);
            }

        }
    }
}