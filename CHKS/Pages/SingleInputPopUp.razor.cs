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

        protected decimal Input = 1;
        protected DateOnly ChosenDate;

        [Parameter]
        public string Title {get; set;}

        protected void Close(){
            if(Title=="Cart ID"){
                DialogService.Close((int)Input);
            }else if(Title=="Choosing Date"){
                DialogService.Close(ChosenDate.ToString());
            }else{
                DialogService.Close(Input);
            }

        }
    }
}