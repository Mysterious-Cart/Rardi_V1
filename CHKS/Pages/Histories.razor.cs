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
    public partial class Histories
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

        protected IEnumerable<CHKS.Models.mydb.History> histories;

        protected RadzenDataGrid<CHKS.Models.mydb.History> grid0;

        protected bool editMode = false;
        protected string date;

        protected string search = "";

        protected string ChosenDate;

        [Inject]
        protected SecurityService Security { get; set; }

        protected async Task Search(ChangeEventArgs args)
        {
            search = $"{args.Value}";

            await grid0.GoToPage(0);

            histories = await mydbService.GetHistories(new Query { Filter = $@"i => (i.CashoutDate.Contains(@0) || i.Plate.Contains(@0))", FilterParameters = new object[] { search }, Expand = "Car" });
        }

        protected override async Task OnInitializedAsync()
        {
            histories = await mydbService.GetHistories(new Query { Filter = $@"i => (i.CashoutDate.Contains(@0) || i.Plate.Contains(@0) )", FilterParameters = new object[] { search }, Expand = "Car" });
        }

        protected async Task OpenHistory(Models.mydb.History args){
            if(editMode == false){
            }
        }   



        protected async Task ExportClick(RadzenSplitButtonItem args)
        {
            if (args?.Value == "csv")
            {
                await mydbService.ExportHistoriesToCSV(new Query{
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Car",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Histories");
            }

            if (args == null || args.Value == "xlsx")
            {
                await mydbService.ExportHistoriesToExcel(new Query
                {
                    Filter = $@"{(string.IsNullOrEmpty(grid0.Query.Filter)? "true" : grid0.Query.Filter)}",
                    OrderBy = $"{grid0.Query.OrderBy}",
                    Expand = "Car",
                    Select = string.Join(",", grid0.ColumnsCollection.Where(c => c.GetVisible() && !string.IsNullOrEmpty(c.Property)).Select(c => c.Property.Contains(".") ? c.Property + " as " + c.Property.Replace(".", "") : c.Property))
                }, "Histories");
            }
        }

        protected async Task GridDeleteButtonClick( CHKS.Models.mydb.History history)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure?") == true)
                {
                    history.Info = "Deleted By:" + Security.User?.Name + "("+ DateTime.Now +")";
                    await mydbService.UpdateHistory(history.Id, history);
                    await grid0.Reload();
                }else{
                    editMode = false;
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete History"
                    
                });
            }
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.History data)
        {
            ChosenDate = data.CashoutDate;
            await grid0.EditRow(data);
            editMode = true;
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.History args)
        {
                
            await mydbService.UpdateHistory(args.Id,args);
            editMode = false;

        }


         protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.History data)
        {
            Console.WriteLine(ChosenDate);
            if(ChosenDate == "01/01/0001" ){
                await DialogService.Alert("No New Date given.","Important");
                await CancelButtonClick(args,data);
            }else{
                data.CashoutDate = ChosenDate;
                await grid0.UpdateRow(data);
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.History data)
        {
            grid0.CancelEditRow(data);
            await mydbService.CancelHistoryChanges(data);
            editMode = false;
        }
    }
}