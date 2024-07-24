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
    public partial class ReciptView
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

        protected IEnumerable<CHKS.Models.mydb.Historyconnector> Historyconnector;
        protected IEnumerable<CHKS.Models.mydb.Inventory> Inventory;
        protected CHKS.Models.mydb.History History;

        protected RadzenDataGrid<CHKS.Models.mydb.Historyconnector> grid0;

        protected string Date;
        protected string CustomerID;
        protected bool Isloading = true;

        [Parameter]
        public string ID {get; set;}

        [Inject]
        protected SecurityService Security { get; set; }

        protected override async Task OnInitializedAsync()
        { 
            Historyconnector = await MydbService.GetHistoryconnectors();
            History = await MydbService.GetHistoryByCashoutDate(ID);
            if(Historyconnector != null ){
                GettingHistory();
            }

        }

        protected async void GettingHistory(){
            Historyconnector = Historyconnector.Where(i => i.CartId == ID);
            Date =  History.CashoutDate;
            CustomerID = History.Plate;
            History.Total = Historyconnector!=Enumerable.Empty<Models.mydb.Historyconnector>()? Historyconnector.Sum(i => i.Export*i.Qty):0;
            await MydbService.UpdateHistory(ID,History);
            
        }

        protected async Task OpenInventory(){
            var product = await DialogService.OpenAsync<Inventories>("Choose", new Dictionary<string, object>{{"IsDialog", "true"}}, new DialogOptions{Width="80%"});
            IEnumerable<Models.mydb.Historyconnector> CheckForAlreadyExist = await MydbService.GetHistoryconnectors();
            CheckForAlreadyExist = CheckForAlreadyExist.Where(i => i.Product == product.Name);
            if(product != null && CheckForAlreadyExist.Any() == false ){
                Models.mydb.Historyconnector TempHiscon = new(){
                    Id = product.Name + DateTime.Now.ToString(),
                    Product = product.Name,
                    Qty = product.Stock,
                    Export = product.Export,
                    CartId = History.CashoutDate,
                    Note = "",
                };
                try{
                    await MydbService.CreateHistoryconnector(TempHiscon);
                }catch(Exception exc){
                }
            }else if(product != null) {
                Models.mydb.Historyconnector ThisProduct = CheckForAlreadyExist.FirstOrDefault();
                ThisProduct.Qty += product.Stock;
                try{
                    await MydbService.UpdateHistoryconnector(ThisProduct.Id, ThisProduct);
                }catch(Exception exc){
                }
            }
        }



        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid0.EditRow(data);
        }

         protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Historyconnector data)
        {
            grid0.CancelEditRow(data);
            await MydbService.CancelHistoryconnectorChanges(data);
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Historyconnector args)
        {
            await MydbService.UpdateHistoryconnector(args.Id, args);
            
        }

    }
}