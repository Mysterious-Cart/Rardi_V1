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

        [Inject]
        protected mydbService mydbService {get; set;}

        protected int CartID;
        protected string[] Product = {"1", "0", ""};
        protected DateOnly ChosenDate;
        protected string GivenMoney;

        [Parameter]
        public string[] Info {get; set;}

        [Inject]
        protected SecurityService Security { get; set; }

        protected IEnumerable<Models.mydb.Inventory> ProductList;
        protected Models.mydb.Inventory ChosenProduct;

        protected async override Task OnInitializedAsync()
        {
            if(Info[0] == "Qty" ){
                Info[1] = Math.Round(decimal.Parse(Info[1]),2).ToString();
                Info[2] = Math.Round(decimal.Parse(Info[2]),2).ToString();
                Product[1] = Info[1]; 
            }else if(Info[0] == "EditItem"){
                Info[1] = Math.Round(decimal.Parse(Info[1]),2).ToString();
                Info[2] = Math.Round(decimal.Parse(Info[2]),2).ToString();
                Product[0] = Info[2];
                Product[1] = Info[1]; 
                Product[2] = Info[3];
            }else if(Info[0] == "Combine"){
                List<Models.mydb.Inventory> temp = new List<Models.mydb.Inventory>();
                foreach(string i in Info){ 
                    if(i != "Combine"){
                        Models.mydb.Inventory Product = await mydbService.GetInventoryByCode(i);
                        temp.Add(Product);
                    }
                }
                ProductList = temp;
            }
        }

        protected void Close(){
            if(Info[0]=="Cart ID"){
                DialogService.Close(CartID);
            }else if(Info[0]=="Choosing Date"){
                DialogService.Close(ChosenDate.ToString("dd/MM/yyyy"));
            }else if(Info[0] == "Qty" || Info[0] == "EditItem") {
                DialogService.Close(Product);
            }else if(Info[0] == "Combine"){
                DialogService.Close(ChosenProduct);
            }

        }
    }
}