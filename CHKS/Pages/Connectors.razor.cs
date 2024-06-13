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
    public partial class Connectors
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

        protected IEnumerable<CHKS.Models.mydb.Connector> connectors;

        protected RadzenDataGrid<CHKS.Models.mydb.Connector> grid0;
        protected override async Task OnInitializedAsync()
        {
            connectors = await mydbService.GetConnectors();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            await grid0.InsertRow(new CHKS.Models.mydb.Connector());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector connector)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await mydbService.DeleteConnector(connector.GeneratedKey);

                    if (deleteResult != null)
                    {
                        await grid0.Reload();
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationService.Notify(new NotificationMessage
                {
                    Severity = NotificationSeverity.Error,
                    Summary = $"Error",
                    Detail = $"Unable to delete Connector"
                });
            }
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Connector args)
        {
            await mydbService.UpdateConnector(args.GeneratedKey, args);
        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Connector args)
        {
            await mydbService.CreateConnector(args);
            await grid0.Reload();
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector data)
        {
            await grid0.EditRow(data);
        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector data)
        {
            await grid0.UpdateRow(data);
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Connector data)
        {
            grid0.CancelEditRow(data);
            await mydbService.CancelConnectorChanges(data);
        }
    }
}