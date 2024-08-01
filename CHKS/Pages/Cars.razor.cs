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

        [Inject]
        public PublicCommand PublicCommand {get; set;}

        protected IEnumerable<CHKS.Models.mydb.Car> cars;

        protected RadzenDataGrid<CHKS.Models.mydb.Car> grid0;

        protected bool isEditing = false;

        [Inject]
        protected SecurityService Security { get; set; }

        protected string search = "";

        protected async Task Search(ChangeEventArgs args)
        {
            
            search = $"{args.Value}";

            cars = cars.Where(i => i.Plate.Contains(search) && i.IsDeleted == 0);
        }

        protected override async Task OnInitializedAsync()
        {
            cars = await mydbService.GetCars(new Query{ Filter = $@"i =>i.IsDeleted==0", FilterParameters = new object[] { "1"} });
        }

        protected async Task<string> GetKey(){
            string GenKey = PublicCommand.GenerateRandomKey();
            IEnumerable<Models.mydb.Car> Temp = cars.Where(i => i.Key == GenKey);
            return Temp.Any() == false?GenKey:await GetKey();
        } 

        protected async Task AddButtonClick(MouseEventArgs args)
        {
            isEditing = true;
            await JSRuntime.InvokeVoidAsync("eval", "document.getElementsByClassName('rz-data-grid-data')[0].scrollTop = 0;");
            await grid0.InsertRow(new CHKS.Models.mydb.Car());
        }

        protected async Task GridDeleteButtonClick(MouseEventArgs args, CHKS.Models.mydb.Car car)
        {
            if (await DialogService.Confirm("តើអ្នកច្បាស់ដែរឫទេ?") == true)
            {
                car.IsDeleted = 1;
                car.Info = "Deleted By:" + Security.User?.Name + "("+DateTime.Now.ToString()+")";
                await mydbService.UpdateCar(car.Plate, car);
                await grid0.Reload();
            }
        }

        protected async Task GridRowUpdate(CHKS.Models.mydb.Car args)
        {
            await mydbService.UpdateCar(args.Plate, args);
        }

        protected async Task GridRowCreate(CHKS.Models.mydb.Car args)
        {
            try{
                args.Plate = args.Plate.ToUpper();
                args.Type = args.Type.ToUpper();
                args.IsDeleted = 0;
                await mydbService.CreateCar(args);
                await grid0.Reload();
                isEditing = false;
            }catch(Exception exc){
                if(exc.Message == "Item already available"){
                    await DialogService.Alert("Item already exist.","Note");
                    await grid0.Reload();
                }
            }
        }

        protected async Task EditButtonClick(MouseEventArgs args, CHKS.Models.mydb.Car data)
        {
            isEditing = true;
            await grid0.EditRow(data);
            
        }

        protected async Task SaveButtonClick(MouseEventArgs args, CHKS.Models.mydb.Car data)
        {
            data.Key = await GetKey();
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