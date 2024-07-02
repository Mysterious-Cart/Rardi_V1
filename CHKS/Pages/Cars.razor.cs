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
    public partial class Cars
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

        protected IEnumerable<CHKS.Models.mydb.Car> cars;
        protected IEnumerable<CHKS.Models.mydb.Cart> Carts;

        protected RadzenDataGrid<CHKS.Models.mydb.Car> grid0;
        protected RadzenDataGrid<CHKS.Models.mydb.Cart> grid1;

        protected bool Mode = false;
        protected bool isEditing = false;

        protected override async Task OnInitializedAsync()
        {
            cars = await mydbService.GetCars();

            Carts = await mydbService.GetCarts();
        }

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEditing = true;
            await grid0.InsertRow(new CHKS.Models.mydb.Car());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Car car)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this record?") == true)
                {
                    var deleteResult = await mydbService.DeleteCar(car.Plate);

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
                    Detail = $"Unable to delete Car"
                });
            }
        }

        protected void switchMode(){
            if(Mode == true){
                Mode = false;
            }else{
                Mode = true;
            }
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Car args)
        {
            await mydbService.UpdateCar(args.Plate, args);
        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Car args)
        {
            args.Plate = args.Plate.ToUpper();
            await mydbService.CreateCar(args);
            await grid0.Reload();
            isEditing = false;
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Car data)
        {
            isEditing = true;
            await grid0.EditRow(data);
            
        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Car data)
        {
            await grid0.UpdateRow(data);
            
        }

        protected async Task CancelButtonClick(MouseEventArgs args, CHKS.Models.mydb.Car data)
        {
            isEditing = false;
            grid0.CancelEditRow(data);
            await mydbService.CancelCarChanges(data);
            
        }

        protected async Task SelectCar(CHKS.Models.mydb.Car Args){
            if(isEditing == false) {
                DialogService.Close(Args);
            }
        }
        
        protected async Task SelectCar(CHKS.Models.mydb.Cart Args){ //Overload with cart instead of Car
            if(isEditing == false) {
                DialogService.Close(Args);
            }
        }
    }
}